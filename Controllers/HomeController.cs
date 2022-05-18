using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using project_2564.Models;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Microsoft.AspNetCore.Authorization;

namespace project_2564
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db; // การเชื่อมต่อกับฐานข้อมูล
        }

        [Authorize(AuthenticationSchemes = "Cookies")] // การยืนยันตัวก่อนเข้าทำงานเมธอดนี้
        public IActionResult Index()
        {
            dynamic model = new ExpandoObject(); // สร้างตัวแปรอ็อบเจ็คขึ้นมาใหม่

            var rawsql_a = "SELECT * FROM Queue WHERE is_delete = false and type = 'A' and status = 'wait' and Date(book_date) = Date(now()) order by book_date ASC"; // คำสั่งในการคิวรี่ข้อมูล
            var queue_a = _db.queue.FromSqlRaw(rawsql_a).ToList(); // ดึงข้อมูลจากฐานข้อมูล
            model.queue_a = queue_a; // นำข้อมูลที่ดึงมาจากฐานข้อมูลมาเก็บไว้ในตัวแปร

            var rawsql_b = "SELECT * FROM Queue WHERE is_delete = false and type = 'B' and status = 'wait' and Date(book_date) = Date(now()) order by book_date ASC"; // คำสั่งในการคิวรี่ข้อมูล
            var queue_b = _db.queue.FromSqlRaw(rawsql_b).ToList(); // ดึงข้อมูลจากฐานข้อมูล
            model.queue_b = queue_b; // นำข้อมูลที่ดึงมากจากฐานข้อมูลมาเก็บไว้ในตัวแปร

            var rawsql_c = "SELECT * FROM Queue WHERE is_delete = false and type = 'C' and status = 'wait' and Date(book_date) = Date(now()) order by book_date ASC"; // คำสั่งในการคิวรี่ข้อมูล
            var queue_c = _db.queue.FromSqlRaw(rawsql_c).ToList(); // ดึงข้อมูลจากฐานข้อมูล
            model.queue_c = queue_c; // นำข้อมูลที่ดึงมากจากฐานข้อมูลมาเก็บไว้ในตัวแปร

            var rawsql_stop = "SELECT * FROM Queue WHERE is_delete = false and status = 'stop' and Date(book_date) = Date(now()) order by book_date ASC"; // คำสั่งในการคิวรี่ข้อมูล
            var queue_stop = _db.queue.FromSqlRaw(rawsql_stop).ToList(); // ดึงข้อมูลจากฐานข้อมูล
            model.queue_stop = queue_stop; // นำข้อมูลที่ดึงมากจากฐานข้อมูลมาเก็บไว้ในตัวแปร

            model.Type_A_Today = _db.queue.Where(q =>
              q.type == "A" &&
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year
            ).ToList().Count(); // นำข้อมูลที่ดึงมากจากฐานข้อมูลมาเก็บไว้ในตัวแปร

            model.Type_B_Today = _db.queue.Where(q =>
              q.type == "B" &&
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year
            ).ToList().Count(); // นำข้อมูลที่ดึงมากจากฐานข้อมูลมาเก็บไว้ในตัวแปร

            model.Type_C_Today = _db.queue.Where(q =>
              q.type == "C" &&
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year
            ).ToList().Count(); // นำข้อมูลที่ดึงมากจากฐานข้อมูลมาเก็บไว้ในตัวแปร

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 8:00
            model.Value_8am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 8 && q.book_date.Hour <= 8
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 9:00
            model.Value_9am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 9 && q.book_date.Hour <= 9
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา  10:00
            model.Value_10am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 10 && q.book_date.Hour <= 10
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา  11:00
            model.Value_11am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 11 && q.book_date.Hour <= 11
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 12:00
            model.Value_12am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour == 12 && q.book_date.Hour <= 12
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 13:00
            model.Value_13am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 13 && q.book_date.Hour <= 13
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 14:00
            model.Value_14am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 14 && q.book_date.Hour <= 14
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 15:00
            model.Value_15am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 15 && q.book_date.Hour <= 15
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 16:00
            model.Value_16am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 16 && q.book_date.Hour <= 16
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 17:00
            model.Value_17am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 17 && q.book_date.Hour <= 17
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 18:00
            model.Value_18am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 18 && q.book_date.Hour <= 18
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 19:00
            model.Value_19am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 19 && q.book_date.Hour <= 19
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 20:00
            model.Value_20am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 20 && q.book_date.Hour <= 20
            ).ToList().Count();

            //ดึงข้อมูลจำนวนผู้ใช้บริการในช่วงเวลา 21:00
            model.Value_21am = _db.queue.Where(q =>
              q.book_date.Date == DateTime.Now.Date &&
              q.book_date.Month == DateTime.Now.Month &&
              q.book_date.Year == DateTime.Now.Year &&
              q.book_date.Hour >= 21 && q.book_date.Hour <= 21
            ).ToList().Count();

            // model.tbls = _db.table.OrderBy(t => t.name).ToList();

            ViewData["table_A"] = new SelectList(_db.table.Where(r => r.is_delete.Equals(false) && r.size == "2" && r.status.Equals(true)).OrderBy(t => t.name), "name", "name"); //คิวรี่ข้อมูลจากตารางมาสร้างตัวเลือกในการระบุโต๊ะ
            ViewData["table_B"] = new SelectList(_db.table.Where(r => r.is_delete.Equals(false) && r.size == "4" && r.status.Equals(true)).OrderBy(t => t.name), "name", "name"); //คิวรี่ข้อมูลจากตารางมาสร้างตัวเลือกในการระบุโต๊ะ
            ViewData["table_C"] = new SelectList(_db.table.Where(r => r.is_delete.Equals(false) && r.size != "2" && r.size != "4" && r.status.Equals(true)).OrderBy(t => t.name), "name", "name"); //คิวรี่ข้อมูลจากตารางมาสร้างตัวเลือกในการระบุโต๊ะ

            return View(model); // คืนค่าไปยังหน้าเว็บเพจ
        } // จบเมธอด

        [HttpPost]
        public async Task<IActionResult> Success(int id)
        {
            var queue = await _db.queue.FirstOrDefaultAsync(q => q.id == id); // ทำการเช็คข้อมูลและดึงข้อมูลจากฐานข้อมูล
            if (queue == null)
            {
                return Json(new { success = false, message = "Error while Successful" });
            }// จบเงื่อนไข

            // ทำการอัพเดทข้อมูล
            queue.success_time = DateTime.Now;
            var success_hour = queue.success_time.Hour;
            var success_minute = queue.success_time.Minute;
            var book_hour = queue.book_date.Hour;
            var book_minute = queue.book_date.Minute;
            var wait_hour = success_hour - book_hour;
            var wait_minute = success_minute - book_minute;
            var wait_hour_minute = wait_hour * 60;
            wait_minute = wait_hour_minute + wait_minute;
            queue.wait_time = wait_minute;
            queue.status = "success";

            _db.queue.Attach(queue).Property(x => x.status).IsModified = true; //คำสั่งคิวรี่อัพเดทข้อมูลในฐานข้อมูล
            await _db.SaveChangesAsync(); //ทำการอัพเดทข้อมูล

            return Json(new { success = true, message = "Status successful" }); // คืนค่าข้อความไปยังผู้ใช้งานระบบ
        }//จบเมธอด

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var queue = await _db.queue.FirstOrDefaultAsync(q => q.id == id); // ทำการเช็คข้อมูลและดึงข้อมูลจากฐานข้อมูล
            if (queue == null)
            {
                return Json(new { success = false, message = "Error while Cancel" });
            }// จบเงื่อนไข

            // ทำการอัพเดทข้อมูล
            queue.status = "cancel";
            queue.is_delete = true;

            _db.queue.Attach(queue).Property(x => x.is_delete).IsModified = true; // คำสั่งคิวรี่อัพเดทข้อมูลในฐานข้อมูล
            await _db.SaveChangesAsync(); //ทำการอัพเดทข้อมูล
            return Json(new { success = true, message = "Status cancel successful" }); // คืนค่าข้อความไปยังผู้ใช้งานระบบ
        }// จบเมธอด

        [HttpPost]
        public async Task<IActionResult> Stopping(int id)
        {
            var queue = await _db.queue.FirstOrDefaultAsync(q => q.id == id); // ทำการเช็คข้อมูลและดึงข้อมูลจากฐานข้อมูล
            if (queue == null)
            {
                return Json(new { success = false, message = "Error while Stopping" });
            }// จบเงื่อนไช

            // เปลี่ยนสถานะให้เป็นพักคิว
            queue.status = "stop";

            _db.queue.Attach(queue).Property(x => x.is_delete).IsModified = true; // คำสั่งคิวรี่อัพเดทข้อมูลในฐานข้อมูล
            await _db.SaveChangesAsync(); //ทำการอัพเดทข้อมูล

            return Json(new { success = true, message = "Status stopping successful" }); // คืนค่าข้อความไปยังผู้ใช้งานระบบ
        }// จบเมธอด

        [HttpPost]
        public async Task<IActionResult> Waiting(int id)
        {
            var queue = await _db.queue.FirstOrDefaultAsync(q => q.id == id); // ทำการเช็คข้อมูลและดึงข้อมูลจากฐานข้อมูล
            if (queue == null)
            {
                return Json(new { success = false, message = "Error while Waiting" });
            }// จบเงื่อนไข

            //ทำการอัพเดทข้อมูล
            queue.status = "wait";

            _db.queue.Attach(queue).Property(x => x.is_delete).IsModified = true; // คำสั่งคิวรี่อัพเดทข้อมูลในฐานข้อมูล
            await _db.SaveChangesAsync(); //ทำการอัพเดทข้อมูล

            return Json(new { success = true, message = "Status waiting successful" }); // คืนค่าข้อความไปยังผู้ใช้งานระบบ
        }// จบเมธอด

        private void LineNotify(string lineToken, string message)
        {
            try
            {
                message = System.Web.HttpUtility.UrlEncode(message, Encoding.UTF8);
                var request = (HttpWebRequest)WebRequest.Create("https://notify-api.line.me/api/notify"); // ตั้งค่าพาทสำหรับส่งข้อมูลไปยัง api ของไลน์
                var postData = string.Format("message={0}", message); 
                var data = Encoding.UTF8.GetBytes(postData); // เข้ารหัสข้อมูล
                // ตั้งค่าวิธีการส่งข้อมูลไปยัง api 
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Headers.Add("Authorization", "Bearer " + lineToken);
                var stream = request.GetRequestStream();
                stream.Write(data, 0, data.Length);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); // แสดงข้อความการทำงานผิดพลาด
            }
        }// จบเมธอด

        [HttpPost]
        public IActionResult CallInLineNotify(String Queue_no, String tbl)
        {
            string token = "D6kQOieF6gV8LfwN19UM6SBaTU8tu7VR3ZNBoEHanUD"; // โทเคนสำหรับส่งข้อความไปยังกลุ่มไลน์
            string message = "ขอเชิญคิว " + Queue_no + " เข้ารับบริการด้วยค่ะ."; // ข้อความที่ต้องการส่ง
            tbl = "0" ;
            LineNotify(token, message); // นำข้อมูลส่งเข้าไปยังเมธอดเพื่อส่งข้อความไปยังกลุ่มไลน์
            Main(Queue_no+","+tbl); // ส่งข้อมูลไปยัง Mqtt broker 
            
            return Redirect("~/Home/Index"); // กลับไปทำงานที่หน้าเว็บเพจ
        }// จบเมธอด

        [HttpPost]
        public IActionResult ChangeStatusTbl(int id)
        {
          
          var table = _db.table.FirstOrDefault( t => t.id == id) ;
          table.status = !table.status ;

          _db.table.Update(table);
          _db.SaveChanges();

          return Redirect("~/Home/Index");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public static async Task Main(string Queue_no)
        {
            String Queue = Queue_no ;

            // ตั้งการค่าเชื่อมต่อกับ Mqtt broker
            var mqttFactory = new MqttFactory();
            IMqttClient client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                            .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer("broker.hivemq.com", 1883)
                            .WithCleanSession()
                            .Build();

            client.UseConnectedHandler( e => 
            {
                Console.WriteLine("Connected to broker successfully");
            });

            client.UseDisconnectedHandler( e=> 
            {
                Console.WriteLine("Disconnect from broker successfully");
            });

            await client.ConnectAsync(options); // เริ่มการเชื่อต่อกับ Broker

            await PublishMessageAsync(client, Queue) ; // ส่งข้อมูลไปยังตัว Broker

            await client.DisconnectAsync(); // ตัดการเชื่อมต่อกับ Broker
        }// จบเมธอด

        private static async Task PublishMessageAsync(IMqttClient client,string message_from_web)
        {
            string messagepayload = message_from_web ;

            // ตั้งค่าพาทสำหรับการส่งข้มูลไปยังตัว Broker
            var message = new MqttApplicationMessageBuilder()
                            .WithTopic("rmutl/myres/project")
                            .WithPayload(messagepayload)
                            .WithAtLeastOnceQoS()
                            .Build();
            
            if(client.IsConnected)
            {
                await client.PublishAsync(message); // ทำการส่งข้อมูลไปยัง Broker
            }// จบเงื่อนไข
        }//จบเมธอด
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
