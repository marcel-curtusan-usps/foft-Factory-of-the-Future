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
            
            //Fetch the File.
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];
            double metersPerPixelY = Convert.ToDouble(HttpContext.Current.Request.Form["metersPerPixel"]);
            double metersPerPixelX = Convert.ToDouble(HttpContext.Current.Request.Form["metersPerPixel"]);
            string name = HttpContext.Current.Request.Form["name"];
            int fileLen;
            fileLen = postedFile.ContentLength;
           
            CoordinateSystem CSystem = new CoordinateSystem();
            CSystem.Name = Path.GetFileNameWithoutExtension(postedFile.FileName);
            CSystem.Id = Guid.NewGuid().ToString();
            using (Image OSLImage = Image.FromStream(postedFile.InputStream))
            {
                byte[] input = AppParameters.ImageToByteArray(OSLImage);
                string imageBase64 = Convert.ToBase64String(input);
                BackgroundImage temp = new BackgroundImage()
                {
                    Id = Path.GetFileNameWithoutExtension(postedFile.FileName),
                    Name = string.IsNullOrEmpty(name) ? "Main Floor" : name,
                    OrigoX = OSLImage.Width,
                    OrigoY = OSLImage.Height,
                    MetersPerPixelX = metersPerPixelX,
                    MetersPerPixelY = metersPerPixelY,
                    WidthMeter = OSLImage.Width * metersPerPixelY,
                    HeightMeter = OSLImage.Height * metersPerPixelX,
                    Base64 = string.Concat("data:image/png;base64,", imageBase64) ,
                    FacilityName = !string.IsNullOrEmpty(AppParameters.AppSettings["FACILITY_NAME"].ToString()) ? AppParameters.AppSettings["FACILITY_NAME"].ToString() : "Site Not Configured",
                    ApplicationFullName = AppParameters.AppSettings["APPLICATION_FULLNAME"].ToString(),
                    ApplicationAbbr = AppParameters.AppSettings["APPLICATION_NAME"].ToString(),
                };
                CSystem.BackgroundImage = temp;
                AppParameters.CoordinateSystem.TryAdd(CSystem.Id, CSystem);
            }
            ////Fetch the File Name.
            string fileName = postedFile.FileName;
            //string fullFilePath = string.Concat(path, @"\", fileName);
            //FileInfo fileToupload = new FileInfo(fullFilePath);
            //if (fileToupload.Exists)
            //{
            //    //move old image to archive
            //    File.Move(fileToupload.FullName, fileToupload.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(fileToupload.Name) + "_" + fileToupload.LastWriteTime.ToString("yyyy_MM_dd_HH_mm_ss_fff") + fileToupload.Extension);
            //    postedFile.SaveAs(fullFilePath);

            //    //Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(temp1, Formatting.None), "getProjectInfo", "0"));
            //}
            //else
            //{
            //    //Save the image.
            //    postedFile.SaveAs(fullFilePath);
            //    //Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(temp1, Formatting.None), "getProjectInfo", "0"));
            //}

            //Send OK Response to Client.
            return Request.CreateResponse(HttpStatusCode.OK, fileName);
        }
    }
   
}