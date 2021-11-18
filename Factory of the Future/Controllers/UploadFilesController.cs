using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class UploadFilesController : ApiController
    {
        public Task Post()
        {
            List<string> savedFilePath = new List<string>();
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string rootPath = HttpContext.Current.Server.MapPath("~/UploadedFiles");
            var provider = new MultipartFileStreamProvider(rootPath);
            var task = Request.Content.ReadAsMultipartAsync(provider).
            ContinueWith(t =>
            {
                if (t.IsCanceled || t.IsFaulted)
                {
                    Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                }
                foreach (MultipartFileData item in provider.FileData)
                {
                    try
                    {
                        string name = item.Headers.ContentDisposition.FileName.Replace("\"", "");
                        string newFileName = Guid.NewGuid() + Path.GetExtension(name);
                        File.Move(item.LocalFileName, Path.Combine(rootPath, newFileName));
                        Uri baseuri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, string.Empty));
                        string fileRelativePath = "~/UploadedFiles/" + newFileName;
                        Uri fileFullPath = new Uri(baseuri, VirtualPathUtility.ToAbsolute(fileRelativePath));
                        savedFilePath.Add(fileFullPath.ToString());
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                    }
                }
                return Request.CreateResponse(HttpStatusCode.Created, savedFilePath);
            });
            return task;
        }
    }
}