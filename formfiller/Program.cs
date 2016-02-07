#define WORD
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
using Microsoft.ServiceBus.Messaging;

namespace formfiller
{
    static class Program
    {
        static object docname = @"c:\src\legalvoice\formfiller\dissowithmarkup.docx";

        static void Main(string[] args)
        {
            var qclient = QueueClient.CreateFromConnectionString(ConfigurationManager.AppSettings["serviceBus"]);
            qclient.OnMessage(ProcessMessage);

            Console.WriteLine("press enter to exit.");
            Console.ReadLine();
        }


        static void ProcessMessage(BrokeredMessage msg)
        {
            var app = new Application();
            object filename = null;

            WriteLine("queue received message");
            var blobID = msg.GetBody<string>();
            msg.Complete();

            WriteLine($"getting blob {blobID}");
            var latest = GetBlob(blobID);
            WriteLine(latest);

            WriteLine("opening word");
#if WORD
            try
            {
                var doc = app.Documents.Open(ref docname);

                WriteLine("filling in form");
                var fields =
                    latest["fields"]
                    .Where(o => o.Value<string>("response") != null)
                    .ToDictionary(o => o.Value<string>("id"), o => o.Value<string>("response"));
                FillForm(fields, doc);

                /*WriteLine("exporting PDF");
                var pdf = ExportPDF(doc);
                WriteLine("opening PDF");
                OpenPDF(pdf);*/

                filename = Path.ChangeExtension(Path.GetTempFileName(), ".docx");
                WriteLine($"saving to {filename}");
                doc.SaveAs2(ref filename);
                object saveChanges = false;
                doc.Close(ref saveChanges);
            }
            catch (Exception e)
            {
                WriteLine(e.ToString());
            }
            finally
            {
                app.Quit();
            }

            if (filename != null)
                Process.Start(new ProcessStartInfo((string)filename) { UseShellExecute = true });
#endif
        }



        private static void FillForm(Dictionary<string, string> fields, Document doc)
        {
            var bmc = doc.Bookmarks.Cast<Bookmark>().Count();
            foreach (var bm in doc.Bookmarks.Cast<Bookmark>()) {
                //var prop = latest.Property(bm.Name);
                var value = BookmarkLookup.Value(fields, bm.Name);

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


        static  JObject GetLatestBlob()
        {
            var client = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["connectionString"])
                .CreateCloudBlobClient();

            // extremely inefficient. good enough for the hackathon.
            var cont = client.GetContainerReference("disso");
            var blobs = cont.ListBlobsSegmented(null);
            var latest = blobs.Results.OfType<CloudBlob>().OrderByDescending(b => b.Properties.LastModified).FirstOrDefault();
            if (latest == null)
                return null;

            var stream = latest.OpenRead();
            using (var reader = new StreamReader(stream))
            {
                return (JObject) JObject.ReadFrom(new JsonTextReader(reader));
            }
        }


        static JObject GetBlob(string id)
        {
            var client = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["connectionString"])
                .CreateCloudBlobClient();
            //var cont = client.GetContainerReference("disso");
            //var blob = cont.GetBlobReference(id);
            var blob = client.GetBlobReferenceFromServer(new StorageUri(new Uri(id)));
            using (var ms = new MemoryStream())
            {
                blob.DownloadToStream(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return JObject.Load(new JsonTextReader(new StreamReader(ms)));
            }
        }
    }
}
