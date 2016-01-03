using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Core;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImageSharingWithCloudStorage.DAL
{
    public class ImageStorage
    {
        public const bool USE_BLOB_STORAGE = true;

        public const string ACCOUNT = "imagegarage";
        public const string CONTAINER = "images";

        public static void SaveFile(HttpServerUtilityBase server,
                                    HttpPostedFileBase imageFile,
                                    int imageId)
        {
            if (USE_BLOB_STORAGE)
            {
                CloudStorageAccount account = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));
                CloudBlobClient client = account.CreateCloudBlobClient();
                CloudBlobContainer container = client.GetContainerReference(CONTAINER);
                CloudBlockBlob blob = container.GetBlockBlobReference(FilePath(server, imageId));
                imageFile.InputStream.Position = 0;
                blob.UploadFromStream(imageFile.InputStream);
            }
            else
            {
                string imgFileName = FilePath(server, imageId);
                imageFile.SaveAs(imgFileName);
            }

        }

        public static void DeleteFile(HttpServerUtilityBase server,
                                    int imageId)
        {
            if (USE_BLOB_STORAGE)
            {
                CloudStorageAccount account = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));
                CloudBlobClient client = account.CreateCloudBlobClient();
                CloudBlobContainer container = client.GetContainerReference(CONTAINER);
                CloudBlockBlob blob = container.GetBlockBlobReference(FilePath(server, imageId));

                // Delete the blob.
                blob.Delete();
            }
            else
            {
                System.IO.File.Delete(FilePath(server, imageId));
            }
        }

        public static string FilePath(HttpServerUtilityBase server,
                                      int imageId)
        {
            if (USE_BLOB_STORAGE)
            {
                return FilePath(imageId);
            }
            else
            {
                return server.MapPath("~/Content/Images/" + FilePath(imageId));
            }
        }

        public static string FilePath(int imageId)
        {
            return imageId + ".jpg";
        }

        public static string ImageURI(UrlHelper urlHelper, int imageId)
        {
            if (USE_BLOB_STORAGE)
            {
                return "http://" + ACCOUNT + ".blob.core.windows.net/" + CONTAINER + "/" + FilePath(imageId) ;
            }
            else
            {
                return urlHelper.Content("~/Content/Images/" + imageId.ToString() + ".jpg");
            }
        }
    }
}