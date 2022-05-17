using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        // GET: api/IncomingData
        [HttpPost]
        public HttpResponseMessage UploadFiles()
        {
            string path = string.Concat(AppParameters.Logdirpath, AppParameters.Images);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            JObject temp1 = new JObject
            {
                ["coordinateSystems"] = new JArray { new JObject{
                ["backgroundImages"] = new JArray { new JObject{
                             ["widthMeter"] = 0,
                             ["xMeter"] = 0,
                             ["visible"] = true,
                             ["otherCoordSys"] = "",
                             ["rotation"] = 0,
                             ["base64"] = "",
                             ["origoY"] = 0,
                             ["origoX"] = 0,
                             ["heightMeter"] = 0,
                             ["yMeter"] = 0,
                             ["alpha"] = 0,
                             ["id"] = Guid.NewGuid().ToString(),
                             ["metersPerPixelY"] = 0,
                             ["metersPerPixelX"] = 0,
                          }
                       }
                    }
                }
            };
            //Fetch the File.
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];
            byte[] imagebyte = new byte[postedFile.ContentLength];
            using (Image OSLImage = Image.FromStream(postedFile.InputStream))
            {
                temp1["coordinateSystems"][0]["backgroundImages"][0]["origoX"] = OSLImage.Width;
                temp1["coordinateSystems"][0]["backgroundImages"][0]["origoY"] = OSLImage.Height;
                temp1["coordinateSystems"][0]["backgroundImages"][0]["widthMeter"] = OSLImage.Width;
                temp1["coordinateSystems"][0]["backgroundImages"][0]["heightMeter"] = OSLImage.Height;
            }
            temp1["coordinateSystems"][0]["backgroundImages"][0]["base64"] = string.Concat("data:image/png;base64", Convert.ToBase64String(imagebyte));
            //Fetch the File Name.
            string fileName = postedFile.FileName;
            string fullFilePath = string.Concat(path, @"\", fileName);
            FileInfo fileToupload = new FileInfo(fullFilePath);
            if (fileToupload.Exists)
            {
                //move file to archive
                File.Move(fileToupload.FullName, fileToupload.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(fileToupload.Name) + "_" + fileToupload.LastWriteTime.ToString("yyyy_MM_dd_HH_mm_ss_fff") + fileToupload.Extension);
                postedFile.SaveAs(fullFilePath);

                Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(temp1, Formatting.None), "getProjectInfo", "0"));
            }
            else
            {
                //Save the File.
                postedFile.SaveAs(fullFilePath);
                Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(temp1, Formatting.None), "getProjectInfo", "0"));
            }
            //Send OK Response to Client.
            return Request.CreateResponse(HttpStatusCode.OK, fileName);
        }
        //public Task Post()
        //{
        //    List<string> savedFilePath = new List<string>();
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }
        //    string rootPath = HttpContext.Current.Server.MapPath("~/UploadedFiles");
        //    var provider = new MultipartFileStreamProvider(rootPath);
        //    var task = Request.Content.ReadAsMultipartAsync(provider).
        //    ContinueWith(t =>
        //    {
        //        if (t.IsCanceled || t.IsFaulted)
        //        {
        //            Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
        //        }
        //        foreach (MultipartFileData item in provider.FileData)
        //        {
        //            try
        //            {
        //                string name = item.Headers.ContentDisposition.FileName.Replace("\"", "");
        //                string newFileName = Guid.NewGuid() + Path.GetExtension(name);
        //                File.Move(item.LocalFileName, Path.Combine(rootPath, newFileName));
        //                Uri baseuri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, string.Empty));
        //                string fileRelativePath = "~/UploadedFiles/" + newFileName;
        //                Uri fileFullPath = new Uri(baseuri, VirtualPathUtility.ToAbsolute(fileRelativePath));
        //                savedFilePath.Add(fileFullPath.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                string message = ex.Message;
        //            }
        //        }
        //        return Request.CreateResponse(HttpStatusCode.Created, savedFilePath);
        //    });
        //    return task;
        //}
    }
}