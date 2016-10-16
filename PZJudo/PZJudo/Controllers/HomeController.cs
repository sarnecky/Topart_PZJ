using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PZJudo.DAL;
using PZJudo.Extensions;
using PZJudo.Models;

namespace PZJudo.Controllers
{
    public class HomeController : Controller
    {
        MyContext db = new MyContext();
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                using (MyContext db = new MyContext())
                {
                    var id = User.Identity.GetUserId();
                    var name = User.Identity.Name;
                    var user = db.Users.Where(i => i.Id == id).First(); //user

                    var participations = db.Participations.Where(i => i.UserName == name).ToList(); //eventy

                    var inputs1 =
                        db.UserApplicationFormInputs.Where(i => i.ApplicationUserId == id)
                            .ToDictionary(i => i.FormInputId, i => i.InputValue);
                    Dictionary<string, string>inputs = new Dictionary<string, string>();

                    foreach (var e in inputs1)
                    {
                        var key = db.FormInputs.Where(i => i.FormInputId == e.Key).Select(i=>i.InputName).First();
                        inputs.Add(key,e.Value);
                    }
                    InfoAboutUser info = new InfoAboutUser()
                    {
                        User = user,
                        Participation = participations,
                        Inputs = inputs
                    };
                    return View(info);
                }
            }

            return View();
        }

        public ActionResult RolesTest()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            using (MyContext context = new MyContext())
            {
                context.Roles.AddOrUpdate(r => r.Name,
                new IdentityRole() { Name = "Admin" },
                new IdentityRole() { Name = "Klub" },
                new IdentityRole() { Name = "Zawodnik" }
                );
                context.SaveChanges();
            }
                ViewBag.Message = "Your contact page.";
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            userManager.AddToRole(User.Identity.GetUserId(), "Admin");
            var m = userManager.GetRoles(User.Identity.GetUserId());
            return View();
        }

        public ActionResult Events()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            if (User.Identity.IsAuthenticated)
            {
                var roles = userManager.GetRoles(User.Identity.GetUserId()).ToList();
                return View(roles);
            }
            return View(new List<string> {"noLogin"});

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