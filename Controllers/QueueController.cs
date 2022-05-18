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
    public class QueueController : Controller
    {

        private readonly ApplicationDbContext _db;
        public QueueController(ApplicationDbContext db)
        {
            _db = db; // การเชื่อมต่อกับฐานข้อมูล
        }

        [Authorize(AuthenticationSchemes = "Cookies")] // การยืนยันตัวก่อนเข้าทำงานเมธอดนี้
        public IActionResult Index()
        {
            dynamic model = new ExpandoObject(); // สร้างตัวแปรอ็อบเจ็คขึ้นมาใหม่
            var rawSql = "SELECT * FROM Queue WHERE is_delete = false and status = 'wait' and Date(book_date) = Date(now()) order by book_date ASC"; // คำสั่งในการคิวรี่ข้อมูล
            var queue = _db.queue.FromSqlRaw(rawSql).ToList(); // นำข้อมูลที่คิวรี่มาเก็บไว้ในตัวแปร

            model.queue = queue; //นำข้อมูลเก็บในตัวแปร

            return View(model); // คืนค่าตัวแปรไปยังหน้าเว็บไซต์
        }//จบเมธอด

        public IActionResult AddQ()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQ(Queue queue)
        {
            if (queue.phone_number == null) //เช็คข้อมูลหากไม่ได่มีการกรอกเบอร์โทรศัพท์
            {
                queue.phone_number = "ไม่ได้ระบุ";
            }

            var data = _db.setting.Where(x => x.id == 1).FirstOrDefault();

            if (data.yesterday.Date != DateTime.Now.Date)
            {
                data.counter_today = 1;
                data.yesterday = DateTime.Now;

                _db.setting.Attach(data).Property(d => d.counter_today).IsModified = true;
                _db.setting.Attach(data).Property(d => d.yesterday).IsModified = true;
                _db.SaveChanges();

                queue.user_id = 1; // กำหนดให้ ผู้จองคิวมีรหัสพนักงานเป็น 1 กรณีจองจากเครื่องกดแถวคอย
                queue.queue_no = data.counter_today;
                queue.create_date = DateTime.Now;
                queue.book_date = DateTime.Now;
                queue.update_date = DateTime.Now;
                queue.status = "wait";
                queue.is_delete = false;
                queue.wait_time = 0;

                // ตรวจสอบจำนวนที่จองเพื่อแยกประเภทของคิว
                if (queue.value < 3)
                {
                    queue.type = "A";
                } // จบเงื่อนไขที่ 1
                else if (queue.value < 5)
                {
                    queue.type = "B";
                } // จบเงื่อนไขที่ 2
                else
                {
                    queue.type = "C";
                }// จบเงื่อนไขที่ 3

                data.counter_today = data.counter_today + 1;
                _db.setting.Attach(data).Property(d => d.counter_today).IsModified = true;
                _db.SaveChanges();

                _db.queue.Add(queue); // คำสั่งในการเพิ่มข้อมูล
                _db.SaveChanges(); // ทำการอัพเดทข้อมูลในฐานข้อมูล
            }
            else
            {
                // กำหนดข้อมูลในการเริ่มจองคิว
                queue.user_id = 1; // กำหนดให้ ผู้จองคิวมีรหัสพนักงานเป็น 1 กรณีจองจากเครื่องกดแถวคอย
                queue.queue_no = data.counter_today;
                queue.create_date = DateTime.Now;
                queue.book_date = DateTime.Now;
                queue.update_date = DateTime.Now;
                queue.status = "wait";
                queue.is_delete = false;
                queue.wait_time = 0;

                // ตรวจสอบจำนวนที่จองเพื่อแยกประเภทของคิว
                if (queue.value < 3)
                {
                    queue.type = "A";
                } // จบเงื่อนไขที่ 1
                else if (queue.value < 5)
                {
                    queue.type = "B";
                } // จบเงื่อนไขที่ 2
                else
                {
                    queue.type = "C";
                }// จบเงื่อนไขที่ 3

                data.counter_today = data.counter_today + 1;
                _db.setting.Attach(data).Property(d => d.counter_today).IsModified = true;
                _db.SaveChanges();

                _db.queue.Add(queue); // คำสั่งในการเพิ่มข้อมูล
                _db.SaveChanges(); // ทำการอัพเดทข้อมูลในฐานข้อมูล

            }
            return Redirect("~/queue/QRcode/" + queue.id); // คืนค่าข้อมูลการจองคิวไปยังผู้จอง
        }// จบเมธอด

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult AddQueue()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQueue(Queue queue)
        {
            var user = _db.user.FirstOrDefault(u => u.id == HttpContext.Session.GetInt32("user_id")); // ดึงข้อมูลรหัสผู้จัดการ
            var data = _db.setting.Where(x => x.id == 1).FirstOrDefault();

            if (data.yesterday.Date != DateTime.Now.Date)
            {
                data.counter_today = 1;
                data.yesterday = DateTime.Now;

                _db.setting.Attach(data).Property(d => d.counter_today).IsModified = true;
                _db.setting.Attach(data).Property(d => d.yesterday).IsModified = true;
                _db.SaveChanges();

                queue.user_id = user.id; //กำหนดรหัสผู้จอง
                // กำหนดข้อมูลในการเริ่มจองคิว
                queue.queue_no = data.counter_today ;
                queue.create_date = DateTime.Now;
                queue.update_date = DateTime.Now;
                queue.is_delete = false;
                queue.status = "wait";
                queue.wait_time = 0;
                // ตรวจสอบจำนวนที่จองเพื่อแยกประเภทของคิว
                if (queue.value < 3)
                {
                    queue.type = "A";
                } // จบเงื่อนไขที่ 1
                else if (queue.value < 5)
                {
                    queue.type = "B";
                } // จบเงื่อนไขที่ 2
                else
                {
                    queue.type = "C";
                } // จบเงื่อนไขที่ 3

                data.counter_today = data.counter_today + 1;
                _db.setting.Attach(data).Property(d => d.counter_today).IsModified = true;
                _db.SaveChanges();

                _db.queue.Add(queue); // คำสั่งในการเพิ่มข้อมูล
                _db.SaveChanges(); //ทำการอัพเดทข้อมูลในฐานข้อมูล
            }
            else
            {
                queue.user_id = user.id; //กำหนดรหัสผู้จอง
                // กำหนดข้อมูลในการเริ่มจองคิว
                queue.queue_no = data.counter_today ;
                queue.create_date = DateTime.Now;
                queue.update_date = DateTime.Now;
                queue.is_delete = false;
                queue.status = "wait";
                queue.wait_time = 0;
                // ตรวจสอบจำนวนที่จองเพื่อแยกประเภทของคิว
                if (queue.value < 3)
                {
                    queue.type = "A";
                } // จบเงื่อนไขที่ 1
                else if (queue.value < 5)
                {
                    queue.type = "B";
                } // จบเงื่อนไขที่ 2
                else
                {
                    queue.type = "C";
                } // จบเงื่อนไขที่ 3

                data.counter_today = data.counter_today + 1;
                _db.setting.Attach(data).Property(d => d.counter_today).IsModified = true;
                _db.SaveChanges();

                _db.queue.Add(queue); // คำสั่งในการเพิ่มข้อมูล
                _db.SaveChanges(); //ทำการอัพเดทข้อมูลในฐานข้อมูล
            }
            return Redirect("~/queue/Index"); // คืนไปหน้าเว็บเพจ
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        public IActionResult EditQueue(int id)
        {
            var queue = _db.queue.FirstOrDefault(q => q.id == id); // คิวรี่ข้อมููลจากฐานข้อมูล
            return View(queue); // คือค่าข้อมูลไปยังหน้าแก้ไขข้อมูล
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditQueue(Queue queue)
        {
            // กำหนดข้อมูลในแก้ไข
            queue.update_date = DateTime.Now;
            queue.is_delete = false;

            // ตรวจสอบจำนวนที่แก้ไขเพื่อระบุประเภทให้ถูกต้อง
            if (queue.value < 3)
            {
                queue.type = "A";
            }// จบเงื่อนไขที่ 1
            else if (queue.value < 5)
            {
                queue.type = "B";
            }// จบเงื่อนไขที่ 2
            else
            {
                queue.type = "C";
            }// จบเง่อนไขที่ 3

            _db.queue.Update(queue); //คำสั่งในการอัพเดทข้อมูล
            _db.SaveChanges(); // ทำการอัพเดทข้อมูล
            return Redirect("~/queue/Index"); // คืนค่าไปยังเว็บเพจ
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var queue = await _db.queue.FirstOrDefaultAsync(q => q.id == id); // ทำการเช็คข้อมูลและดึงข้อมูลจากฐานข้อมูล
            if (queue == null) // ตรวจสอบข้อมูลว่ามีค่าเป็น null หรือไม่
            {
                return Json(new { success = false, message = "Error while Deleting" }); // คืนค่าข้อความไปยังผู้ใช้งาน
            }// จบเงื่อนไข

            // เปลี่ยนสถานะการลบ
            queue.is_delete = true;

            _db.queue.Attach(queue).Property(x => x.is_delete).IsModified = true; //คำสั่งคิวรี่ในการนำข้อมูลไปอัพเดทในฐานข้อมูล
            await _db.SaveChangesAsync(); //ทำการอัพเดทข้อมูล

            return Json(new { success = true, message = "Delete successful" }); // คืนค่าข้อความไปยังผู้ใช้งานระบบ
        }

        public IActionResult QRcode(int id)
        {
            dynamic model = new ExpandoObject(); // สร้างตัวแปรอ็อบเจ็ค
            var queue = _db.queue.FirstOrDefault(q => q.id == id); // ทำการเช็คข้อมูลและดึงข้อมูลจากฐานข้อมูล
            model.queue = queue; // นำข้อมูลใส่ในตัวแปรอ็อบเจ็ค
            model.wait_time = CalculateWaitTime(queue.type); // ทำการคำนวนหาเวลารอ
            return View(model); // คืนค่าไปยังหน้าเว็บ
        }

        private String CalculateWaitTime(String type)
        {
            var setting = _db.setting.Where(x => x.id == 1).FirstOrDefault();

            var End_date = DateTime.Now.AddMonths(setting.calculate_set*-1) ;

            // Console.WriteLine(End_date) ;

            var count = _db.queue.Where(q => 
            q.type == type && 
            q.status == "success" && 
            q.book_date >= End_date  
            ).Count(); // คืวรี่ข้อมูลคิวประเภทนั้นๆ และนับจำนวนคิวที่สำเร็จ

            if (count > 0)
            {
                var avg_time = _db.queue.Where(q => q.type == type 
                && q.status == "success" &&
                q.book_date >= End_date
                ).Average(q => q.wait_time); // คิวรี่ข้อมูลและทำการหาค่าเฉลี่ย
                if (avg_time < 60)
                {
                    return Math.Round(avg_time).ToString() + " นาที"; // คืนค่าข้อมูลไปยังผู้ใช้งาน
                }// จบเงื่อนไข
                var hours = Math.Floor(avg_time / 60); //ทำการแปลงข้อมูลเป็นหน่วยชั่วโมง
                var minutes = avg_time % 60; // ทำการแปลงข้อมูลเป็นหน่วยนาที
                return Math.Round(hours).ToString() + " ชั่วโมง " + Math.Round(minutes).ToString() + " นาที"; // คืนค่าข้อมูลที่คำนวนไปยังผู้ใช้งาน

            }//  จบเงื่อนไขที่ 1
            else
            {
                return "10 นาที";
            } // จบเงื่อนไขที่ 2
        }// จบเมธอด

    }
}