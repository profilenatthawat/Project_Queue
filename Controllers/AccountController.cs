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
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult Index()
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_view_user)
                {
                    List<User> users = _db.user.Where(u => u.is_delete.Equals(false)).Select(x => new User
                    {
                        id = x.id,
                        username = x.username
                    }).ToList();
                    return View(users);
                }
                return Redirect("~/Home/AccessDenied") ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Redirect("~/Account/Logout");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            try
            {
                var login = _db.user.FirstOrDefault(x => x.username.Equals(user.username));

                if (BCrypt.Net.BCrypt.Verify(user.password, login.password) && login.is_delete == false)
                {
                    ClaimsIdentity identity = null;
                    bool isAuthenticate = false;

                    if (login.role_id != 0)
                    {
                        identity = new ClaimsIdentity(new[] {
                            new Claim(ClaimTypes.Name, user.username),
                            new Claim(ClaimTypes.Role, login.role_id.ToString()),
                        }, CookieAuthenticationDefaults.AuthenticationScheme);
                        isAuthenticate = true;
                    }

                    if (isAuthenticate)
                    {
                        var principal = new ClaimsPrincipal(identity);
                        var IsLogin = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        HttpContext.Session.SetString("username", user.username);
                        HttpContext.Session.SetInt32("user_id", login.id);
                        HttpContext.Session.SetInt32("role_id", login.role_id);

                        return Redirect("~/Home/Index");
                    }
                }
                else
                {
                    ViewBag.message = "รหัสผ่านไม่ถูกต้อง";
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = "ชื่อผู้ใช้งานไม่ถูกต้อง";
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie == ".Application.Session")
                {
                    Response.Cookies.Delete(cookie);
                }
                else
                {
                    Response.Cookies.Delete(cookie);
                }
            }
            await HttpContext.SignOutAsync();
            return Redirect("~/Account/login");
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult AddAccount()
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_add_user)
                {
                    ViewData["role"] = new SelectList(_db.role.Where(r => r.is_delete.Equals(false)), "id", "name");
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
        [ValidateAntiForgeryToken]
        public IActionResult AddAccount(User user)
        {
            try
            {
                ViewData["role"] = new SelectList(_db.role, "id", "name", user.role_id);
                if (!checkUsername(user.username))
                {
                    user.create_date = DateTime.Now;
                    user.update_date = DateTime.Now;
                    user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
                    _db.user.Add(user);
                    _db.SaveChanges();
                    return Redirect("~/Account/Index");
                }
                else
                {
                    ViewBag.message = "ชื่อผู้ใช้นี้ถูกใช้งานไปแล้ว";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Redirect("~/Account/Logout");
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult EditUser(int id)
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_edit_user)
                {
                    ViewData["role"] = new SelectList(_db.role.Where(r => r.is_delete.Equals(false)), "id", "name");
                    var user = _db.user.FirstOrDefault(u => u.id == id);
                    return View(user);
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
        public IActionResult EditUser(User user)
        {
            user.update_date = DateTime.Now;
            _db.user.Update(user);
            _db.SaveChanges();
            return Redirect("~/Account/Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var role = _db.role.FirstOrDefault(r => r.id == HttpContext.Session.GetInt32("role_id"));
                if (role.allow_delete_role)
                {
                    var user = await _db.user.FirstOrDefaultAsync(u => u.id == id);
                    if (user == null)
                    {
                        return Json(new { success = false, message = "ไม่สามารถลบข้อมูลได้ !" });
                    }
                    user.is_delete = true;
                    _db.user.Attach(user).Property(x => x.is_delete).IsModified = true;
                    await _db.SaveChangesAsync();
                    return Json(new { success = true, message = "ลบข้อมูลสำเร็จ" });
                }
                return Json(new { success = false, message = "ไม่สามารถลบข้อมูลได้ !" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(new { success = false, message = "ไม่สามารถลบข้อมูลได้ !" });
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult ChangePass(int id)
        {
            var user = _db.user.FirstOrDefault(u => u.id == id);
            return View(user);
        }

        [HttpPost]
        public IActionResult ChangePass(User user, string check_newpassword)
        {
            if (checkPassword(user.password, check_newpassword))
            {
                user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
                _db.user.Attach(user).Property(x => x.password).IsModified = true;
                _db.SaveChanges();
                return Redirect("~/Account/Index");
            }
            else
            {
                ViewBag.Message = "รหัสผ่านไม่ตรงกัน";
            }
            return View(user);
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult MyCliams()
        {
            return View();
        }

        private bool checkUsername(string username)
        {
            if (_db.user.Any(x => x.username == username))
                return true;
            return false;
        }

        private bool checkPassword(string password, string check_password)
        {
            if (password == check_password)
                return true;
            return false;
        }
    }
}