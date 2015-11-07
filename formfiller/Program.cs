/*
 * Form Filler: fills in a Word Doc whose bookmarks correspond to property names in a
 * JSON blob retrieved from Azure blob storage.
 *
 * Written for the 2015 Seattle Social Justice Hackathon #SSJH
 *
 * Author: Dan Liebling <dan@liebling.org>
 * Date:   2015-11-07
 *
 */

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
    static class Program
    {
        static void Main(string[] args)
        {
            WriteLine("opening word");
            var app = new Application();
            try
            {
                object docname = @"c:\src\legalvoice\formfiller\dissowithmarkup.docx";
                var doc = app.Documents.Open(ref docname);

                WriteLine("Press ENTER to read the posted data.");
                ReadLine();

                WriteLine("getting latest blob");
                var latest = TTask.Run(async () => await GetLatestBlobAsync()).Result;

                if (latest == null)
                {
                    WriteLine("!! no blobs to get");
                }
                else
                {
                    WriteLine("filling in form");
                    FillForm(latest, doc);

                    WriteLine("exporting PDF");
                    var pdf = ExportPDF(doc);
                    WriteLine("opening PDF");
                    OpenPDF(pdf);
                }

                object saveChanges = false;
                doc.Close(ref saveChanges);
            }
            finally
            {
                app.Quit();
            }

            /*var latest = TTask.Run(async () => await GetLatestBlobAsync()).Result;
            Console.WriteLine(latest.ToString(Formatting.Indented));
            Console.ReadLine();*/
        }



        private static void FillForm(JObject latest, Document doc)
        {
            foreach (var bm in doc.Bookmarks.Cast<Bookmark>()) {
                //var prop = latest.Property(bm.Name);
                var value = BookmarkLookup.Value(latest, bm.Name);

                if (value != null)
                {
                    WriteLine($"set {bm.Name} to \"{value}\"");
                    bm.Range.Text = value;
                    bm.Range.Font.Color = WdColor.wdColorBlue;
                }
            }
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


        static async Task<JObject> GetLatestBlobAsync()
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
                return (JObject) JObject.ReadFrom(new JsonTextReader(reader));
            }
        }
    }
}
