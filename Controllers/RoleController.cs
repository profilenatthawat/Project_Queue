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

namespace project_2564
{
    [Authorize(AuthenticationSchemes = "Cookies")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RoleController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_view_role)
                {
                    List<Role> roles = _db.role.Where(r => r.is_delete.Equals(false)).Select(x => new Role
                    {
                        id = x.id,
                        name = x.name
                    }).OrderBy( r => r.id).ToList();
                    return View(roles);
                }
                return Redirect("~/Home/AccessDenied") ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Redirect("~/Account/Logout");
        }

        public IActionResult AddRole()
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_add_role)
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
        public IActionResult AddRole(Role role)
        {
            role.create_date = DateTime.Now;
            role.update_date = DateTime.Now;
            role.is_delete = false;

            _db.role.Add(role);
            _db.SaveChanges();

            return Redirect("~/Role/Index");
        }

        public IActionResult EditRole(int id)
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_edit_role)
                {
                    var role_to_edit = _db.role.FirstOrDefault(r => r.id == id);
                    return View(role_to_edit);
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
        public IActionResult EditRole(Role role)
        {
            role.update_date = DateTime.Now;
            role.is_delete = false;

            _db.role.Update(role);
            _db.SaveChanges();

            return Redirect("~/Role/Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_delete_role)
                {
                    var role_to_delete = await _db.role.FirstOrDefaultAsync(r => r.id == id);
                    if (role_to_delete == null)
                    {
                        return Json(new { success = true , message = "ลบข้อมูลสำเร็จ" });
                    }
                    role_to_delete.is_delete = true;
                    _db.role.Attach(role_to_delete).Property(x => x.is_delete).IsModified = true;
                    await _db.SaveChangesAsync();
                }
                return Json(new { success = false, message = "ลบข้อมูลไม่สำเร็จ" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(new { success = false, message = "ลบข้อมูลไม่สำเร็จ" });
        }
    }
}