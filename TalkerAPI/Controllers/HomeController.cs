using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceStack.ServiceClient.Web;
using TalkerAPI.API;
//using ServiceStack.Common.ServiceClient.Web;
using ServiceStack.ServiceInterface.Auth;
using System.IO;

namespace TalkerAPI.Controllers
{
    public class HomeController : Controller
    {

        JsonServiceClient client;
        AuthResponse authResponse;

        public HomeController()
        {
            
            client = new JsonServiceClient("http://localhost:55034/api")
            {
                UserName = "admin",
                Password = "qwerty"
            };
            client.AlwaysSendBasicAuthHeader = true;

        }

        public ActionResult Index()
        {
            UserRecordsResponse res = null;
            try
            {
                res = client.Get(new UserRecords { UserName = client.UserName });
            }
            catch (Exception e)
            {

            }
            return View(res);
        }

        public ActionResult NewRecord()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PostRecord(HttpPostedFileBase[] aaa)
        {
            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                aaa[0].InputStream.CopyTo(ms);
                buff = ms.ToArray();
            }
            SendRecord a = new SendRecord { UserName = client.UserName, Message = "New record", Value = buff };
            var b = client.Post(a);
            return Index();
        }

    }
}
