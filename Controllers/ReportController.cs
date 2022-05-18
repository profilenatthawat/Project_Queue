using System;
using System.Linq;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using project_2564.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Dynamic;
using System.Data;

namespace project_2564
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReportController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult Index()
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if(role.allow_report)
                {
                    return View();
                }
                return Redirect("~/Home/AccessDenied") ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Redirect("~/Account/Logout");
        }

        [HttpPost]
        public IActionResult report_1(String topic, String daterange)
        {
            Console.WriteLine(topic);
            // Console.WriteLine(daterange);
            string[] subs = daterange.Split(' ');
            // Console.WriteLine(subs[0]);
            // Console.WriteLine(subs[2]);
            var start_date = DateTime.ParseExact(subs[0], "dd/MM/yyyy", null);
            var end_date = DateTime.ParseExact(subs[2], "dd/MM/yyyy", null);

            Console.WriteLine("start_date : " + start_date);
            Console.WriteLine("end_daate : " + end_date);

            dynamic model = new ExpandoObject();
            List<Report> reports = new List<Report>();
            Report report = new Report();

            if (topic == "report1")
            {
                for (var st = start_date.Date; st <= end_date.Date; st = st.Date.AddDays(1))
                {
                    var count_A = _db.queue.Where(q =>
                        q.book_date.Date >= st &&
                        q.book_date.Month >= st.Month &&
                        q.book_date.Year >= st.Year &&
                        q.book_date.Date <= st.Date &&
                        q.book_date.Month <= st.Month &&
                        q.book_date.Year <= st.Year &&
                        q.status == "success" && q.type == "A"
                    ).ToList().Count();

                    var count_B = _db.queue.Where(q =>
                        q.book_date.Date >= st &&
                        q.book_date.Month >= st.Month &&
                        q.book_date.Year >= st.Year &&
                        q.book_date.Date <= st.Date &&
                        q.book_date.Month <= st.Month &&
                        q.book_date.Year <= st.Year &&
                        q.status == "success" && q.type == "B"
                    ).ToList().Count();

                    var count_C = _db.queue.Where(q =>
                        q.book_date.Date >= st &&
                        q.book_date.Month >= st.Month &&
                        q.book_date.Year >= st.Year &&
                        q.book_date.Date <= st.Date &&
                        q.book_date.Month <= st.Month &&
                        q.book_date.Year <= st.Year &&
                        q.status == "success" && q.type == "C"
                    ).ToList().Count();

                    var total = count_A + count_B + count_C;

                    report.bookdate = st;
                    report.sum_a = count_A;
                    report.sum_b = count_B;
                    report.sum_c = count_C;
                    report.total = total;
                    reports.Add(report);
                    report = new Report();
                }

                model.reports = reports;

                return View(model);
            }
            return Redirect("~/Account/Logout");
        }
    }
}