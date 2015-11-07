using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;

/// <summary>
/// Summary description for FromRepository
/// </summary>
public class FormRepository
{
    /// <summary>get a client that can connect to the blob store</summary>
    /// <returns></returns>
    private static CloudBlobClient GetClient()
    {
        return CloudStorageAccount.Parse(WebConfigurationManager.AppSettings["connectionString"]).CreateCloudBlobClient();
    }


    /// <summary>
    /// initializes blob storage, ensuring the container exists
    /// </summary>
    public void Init()
    {
        var client = GetClient();
        var cont = client.GetContainerReference("disso");
        cont.CreateIfNotExists();
    }


    /// <summary>Stores a form in the block store</summary>
    /// <param name="form"></param>
    /// <returns>URI of new blob</returns>
    public Uri StoreForm(JObject form)
    {
        var client = GetClient();
        var cont = client.GetContainerReference("disso");
        var blob = cont.GetBlockBlobReference(Guid.NewGuid().ToString());
        blob.UploadText(form.ToString(Newtonsoft.Json.Formatting.None));
        return blob.Uri;
    }
}