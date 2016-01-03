using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Core;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Drawing;

namespace ImageSharingWebRole.DAL
{
    public class ImageStorage
    {
        public const bool USE_BLOB_STORAGE = true;

        public const string ACCOUNT = "imagegarage";
        public const string CONTAINER = "images";

        public static CloudBlobContainer getContainer()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient client = account.CreateCloudBlobClient();
            return client.GetContainerReference(CONTAINER);
        }

        public static void SaveFile(HttpServerUtilityBase server,
                                    HttpPostedFileBase imageFile,
                                    int imageId)
        {
            if (USE_BLOB_STORAGE)
            {
                CloudBlobContainer container = getContainer();
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
                CloudBlobContainer container = getContainer();
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
                return "http://" + ACCOUNT + ".blob.core.windows.net/" + CONTAINER + "/" + FilePath(imageId);
            }
            else
            {
                return urlHelper.Content("~/Content/Images/" + imageId.ToString() + ".jpg");
            }
        }

        public static Stream GetImageFromBlob(int imageId)
        {

            String imageName = FilePath(imageId);
            CloudBlobContainer container = getContainer();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageName);
            MemoryStream image = new MemoryStream();
            blockBlob.DownloadToStream(image);
            return image;
        }

        public static Boolean Validate(int id)
        {
            if (USE_BLOB_STORAGE)
            {
                CloudBlobContainer container = getContainer();
                CloudBlockBlob blob = container.GetBlockBlobReference(FilePath(null, id));

                //Get image stream
                MemoryStream imageStream = new MemoryStream();
                blob.DownloadToStream(imageStream);

                Image image = Image.FromStream(imageStream);

                if (image.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //string imgFileName = FilePath(server, imageId);
                //try
                //{
                //    using (var file = System.IO.File.OpenRead(@"C:\Users\joshi\Documents\Visual Studio 2015\Projects\ImageSharingWithCloudStorage\ImageSharingWithCloudStorage\Images\PinkFloyd.jpg"))
                //    {
                //        file.DownloadTo
                //    }
                //}
                //catch (Exception ex)
                //{

                //}

                //imageFile.SaveAs(imgFileName);
                return true;
            }
        }
    }
}