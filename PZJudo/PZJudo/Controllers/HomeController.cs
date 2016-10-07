using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using PZJudo.DAL;
using PZJudo.Extensions;
using PZJudo.Models;

namespace PZJudo.Controllers
{
    public class HomeController : Controller
    {
        MyContext db = new MyContext();
        public ActionResult Index()
        {/*
            var ev = new Event();
            ev.Name = "wycieczka";
            db.Events.Add(ev);
            db.SaveChanges();
            */
            return View();
        }

        public ActionResult RolesTest()
        {
           
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            var a = db.Users.Include("Roles").Select(i => i.Roles);
        
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Events()
        {
            if (User.Identity.IsAuthenticated)
            {
               // var a = db.Users.Select(i => i.Roles).ToList();
               // db.Roles;
              //  return View(new {userRoles = a});
            }
            return View();
        }

        public void MakeXlsFile(string name, GridView grid)
        {
            Response.ClearContent();
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", name));
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }
        public void ExportToExcel()
        {
            var grid = new GridView();

            grid.DataSource = db.Users.Select(i => new {UserId = i.Id, UserName = i.UserName, UserPasswordHash = i.PasswordHash}).ToList();
            grid.DataBind();

            MakeXlsFile("Users", grid);

            /*
            grid.DataSource = db.Users.Select(i => new { UserId = i.Id, UserName = i.UserName, UserPasswordHash = i.PasswordHash }).ToList();
            grid.DataBind();

            MakeXlsFile("Users", grid);
            
            var grid2 = new GridView();
            grid2.DataSource = db.Events.Include("ApplicationUser").Select(i => new {EventId = i.EventId, EventName = i.Name, Organizer = i.ApplicationUser.UserName}).ToList();
            grid2.DataBind();

            MakeXlsFile("Events", grid2);
            */
        }
    }
}