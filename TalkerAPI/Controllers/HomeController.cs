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

        public ActionResult Index()
        {
            if (Session["name"] == null)
            {
                return View();
            }
            return RedirectToAction("Info");
        }

        [HttpPost]
        public ActionResult Index(string name, string password)
        {
            Session["name"] = name;
            Session["password"] = password;
            return RedirectToAction("Info");
        }

        public ActionResult Info()
        {
            var t = Session["name"];
            if (Session["name"] == null)
            {
                return RedirectToAction("Index");
            }
            client = new JsonServiceClient("http://coursemanage.apphb.com//api")
            {
                UserName = Session["name"].ToString(),
                Password = Session["password"].ToString()
            };
            client.AlwaysSendBasicAuthHeader = true;

            UserRecordsResponse res = null;
            try
            {
                res = client.Get(new UserRecords { UserName = client.UserName });
            }
            catch
            {

            }
            return View(res);
        }

        public ActionResult NewRecord()
        {
            ViewBag.Er = "NoError";
            if (Session["errora"] != null && Session["errora"] == "1")
            {
                ViewBag.Er = "Error";
            }
            return View();
        }

        [HttpPost]
        public ActionResult PostRecord(HttpPostedFileBase[] aaa)
        {
            if (client == null)
            {
                client = new JsonServiceClient("http://coursemanage.apphb.com//api")
                {
                    UserName = Session["name"].ToString(),
                    Password = Session["password"].ToString()
                };
                client.AlwaysSendBasicAuthHeader = true;
            }

            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                aaa[0].InputStream.CopyTo(ms);
                buff = ms.ToArray();
            }
            SendRecord a = new SendRecord { UserName = client.UserName, Message = "New record", Value = buff };
            var b = client.Post(a);
            return RedirectToAction("View");
        }

    }
}
