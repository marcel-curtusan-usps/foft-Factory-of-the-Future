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
            /*
                id =	String	Unique id of this BackgroundImage
                metersPerPixelX	 = Number	X scale of the image. Denotes how many meters each pixel (in x dir) of the image represents.
                metersPerPixelY	 = Number	Y scale of the image. Denotes how many meters each pixel (in y dir) of the image represents.
                origoX =	Number	Origo x location of the coordinate system (ie. pixels from topleft corner of the image)
                origoY =	Number	Origo y location of the coordinate system (ie. pixels from topleft corner of the image)
                xMeter =	Number	X coordinate for the bottom left corner of the background image (in meters)
                yMeter =	Number	Y coordinate for the bottom left corner of the background image (in meters)
                widthMeter =	Number	Width of the background image (in meters)
                heightMeter =	Number	Height of the background image (in meters)
                rotation =	Number	Rotation of the image in degrees
                alpha =	Number	Alpha value used in rendering of this image
                visible =	Boolean	Boolean indicating if this image is set to be visible or not
                otherCoordSys =	String	Comma separated list of other coordinate systems that use the same background image
                base64 =	String	Image bytes as base64 encoded string.
             */
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
                             ["id"] = "",
                             ["metersPerPixelY"] = 0,
                             ["metersPerPixelX"] = 0,
                          }
                       }
                    }
                }
            };
            //Fetch the File.
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];
            double metersPerPixelY = Convert.ToDouble(HttpContext.Current.Request.Form["metersPerPixelY"]);
            double metersPerPixelX = Convert.ToDouble(HttpContext.Current.Request.Form["metersPerPixelX"]);
            int fileLen;
            fileLen = postedFile.ContentLength;
            byte[] input = new byte[fileLen];
            string imageBase64 = "";
            using (Image OSLImage = Image.FromStream(postedFile.InputStream))
            {
                temp1["coordinateSystems"][0]["backgroundImages"][0]["id"] = Path.GetFileNameWithoutExtension(postedFile.FileName);
                temp1["coordinateSystems"][0]["backgroundImages"][0]["origoX"] = OSLImage.Width;
                temp1["coordinateSystems"][0]["backgroundImages"][0]["origoY"] = OSLImage.Height;
                temp1["coordinateSystems"][0]["backgroundImages"][0]["metersPerPixelY"] = metersPerPixelY;
                temp1["coordinateSystems"][0]["backgroundImages"][0]["metersPerPixelX"] = metersPerPixelX;
                temp1["coordinateSystems"][0]["backgroundImages"][0]["widthMeter"] = OSLImage.Width * metersPerPixelY;
                temp1["coordinateSystems"][0]["backgroundImages"][0]["heightMeter"] = OSLImage.Height * metersPerPixelX;
                temp1["coordinateSystems"][0]["name"] = Path.GetFileNameWithoutExtension(postedFile.FileName);
                input = AppParameters.ImageToByteArray(OSLImage);
                imageBase64 = Convert.ToBase64String(input);
                temp1["coordinateSystems"][0]["backgroundImages"][0]["base64"] = string.Concat("data:image/png;base64,", imageBase64);

            }
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
    }
   
}