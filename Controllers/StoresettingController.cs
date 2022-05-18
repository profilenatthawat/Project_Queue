using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using project_2564.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace project_2564
{

    public class StoresettingController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StoresettingController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult Index()
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_view_settings)
                {
                    dynamic model = new ExpandoObject();
                    List<Table> tables = _db.table.Where(t => t.is_delete.Equals(false)).Select(t => new Table
                    {
                        id = t.id,
                        name = t.name,
                        size = t.size
                    }).ToList();

                    return View(tables);
                }
                return Redirect("~/Home/AccessDenied") ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Redirect("~/Account/Logout");
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult AddTable()
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_add_settings)
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Redirect("~/Account/Logout");
        }

        [HttpPost]
        public IActionResult AddTable(Table table)
        {
            table.is_delete = false;
            table.create_date = DateTime.Now;
            table.update_date = DateTime.Now;

            _db.table.Add(table);
            _db.SaveChanges();

            return Redirect("~/Storesetting/Index");
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult EditTable(int id)
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_edit_settings)
                {
                    var table = _db.table.FirstOrDefault(t => t.id == id);
                    return View(table);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Redirect("~/Account/Logout");
        }

        [HttpPost]
        public IActionResult EditTable(Table table)
        {
            table.update_date = DateTime.Now;

            _db.table.Update(table);
            _db.SaveChanges();

            return Redirect("~/Storesetting/Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var table = await _db.table.FirstOrDefaultAsync(t => t.id == id);
            if (table == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            table.is_delete = true;
            _db.table.Attach(table).Property(x => x.is_delete).IsModified = true;
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Delete successful" });
        }

        [HttpPost]
        public IActionResult change_cal(int calculate_set)
        {
            Console.WriteLine(calculate_set) ;

            var data = _db.setting.Where(x => x.id == 1).FirstOrDefault();

            data.calculate_set = calculate_set ;

            _db.setting.Attach(data).Property(d => d.calculate_set).IsModified = true;
            _db.SaveChanges();

            return Redirect("~/Home/Index") ;
        }
    }
}