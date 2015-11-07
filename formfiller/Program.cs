using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTask = System.Threading.Tasks.Task;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using static System.Console;

namespace formfiller
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("opening word");
            var app = new Application();
            try
            {
                object docname = @"c:\src\legalvoice\formfiller\dissowithmarkup.docx";
                var doc = app.Documents.Open(ref docname);

                WriteLine("getting latest blob");
                var latest = TTask.Run(async () => await GetLatestBlobAsync()).Result;

                if (latest == null)
                {
                    WriteLine("!! no blobs to get");
                }
                else
                {
                    WriteLine("filling in form");
                    FillForm((JObject) latest, doc);

                    WriteLine("exporting PDF");
                    var pdf = ExportPDF(doc);
                    WriteLine("opening PDF");
                    OpenPDF(pdf);
                }

                object saveChanges = false;
                doc.Close(ref saveChanges);
            }
            finally {
                app.Quit();
            }
        }


        private static void FillForm(JObject latest, Document doc)
        {
            foreach (var bm in doc.Bookmarks.Cast<Bookmark>()) {
                var prop = latest.Property(bm.Name);
                if (prop != null)
                {
                    string value = null;
                    switch (prop.Value.Type)
                    {
                        case JTokenType.String:
                            value = prop.Value<string>();
                            break;
                        case JTokenType.Boolean:
                            value = prop.Value<bool>() ? "√" : null;
                            break;
                        default:
                            WriteLine($"unexpected property type: {prop.Value.Type}");
                            break;
                    }
                }
            }
        }

        
        /// <summary>Makes a convenient dictionary for accessing the Word bookmarks</summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static Dictionary<string, Bookmark> MapBookmarks(Document doc)
        {
            return doc.Bookmarks.Cast<Bookmark>().ToDictionary(bm => bm.Name, bm => bm);
        }


        private static string ExportPDF(Document doc)
        {
            var tempfile = Path.ChangeExtension(Path.GetTempFileName(), ".pdf");
            doc.ExportAsFixedFormat(tempfile, WdExportFormat.wdExportFormatPDF);
            return tempfile;
        }


        private static void OpenPDF(string filename)
        {
            Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
        }


        static async Task<JToken> GetLatestBlobAsync()
        {
            var client = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["connectionString"])
                .CreateCloudBlobClient();

            // extremely inefficient. good enough for the hackathon.
            var cont = client.GetContainerReference("disso");
            var blobs = await cont.ListBlobsSegmentedAsync(null);
            var latest = blobs.Results.OfType<CloudBlob>().OrderByDescending(b => b.Properties.LastModified).FirstOrDefault();
            if (latest == null)
                return null;

            var stream = await latest.OpenReadAsync();
            using (var reader = new StreamReader(stream))
            {
                return JObject.ReadFrom(new JsonTextReader(reader));
            }
        }
    }
}
