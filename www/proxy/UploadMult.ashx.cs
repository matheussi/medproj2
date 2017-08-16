namespace MedProj.www.proxy
{
    using System;
    using System.Web;
    using System.Linq;
    using System.Configuration;
    using System.Collections.Generic;

    public class UploadMult : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Server.ScriptTimeout = Int32.MaxValue;

            HttpPostedFile file = context.Request.Files["Filedata"];

            String fileName = file.FileName;

            Random Number = new Random();
            Int32 NewNumber = Number.Next(10000) * Number.Next(1000);
            String newFileName = String.Concat(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, "_",
                NewNumber, "_", Util.Geral.RetiraAcentos(file.FileName));

            file.SaveAs(context.Server.MapPath(ConfigurationManager.AppSettings["AnexosTemp"] + newFileName));

            context.Response.ContentType = "text/plain";
            context.Response.Write(newFileName + "|" + fileName);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}