using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project_2564.Models
{
    public class Queue
    {
        public int id { get; set; }
        public int queue_no { get; set; }
        public int user_id { get; set; }
        public String customer_name { get; set; }
        public String phone_number { get; set; }
        public int value { get; set; }
        public String type { get; set; }
        public String status { get; set; }
        public int wait_time { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime success_time { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime book_date { get; set; }
        [DataType(DataType.Date)]
        public DateTime create_date { get; set; }
        [DataType(DataType.Date)]
        public DateTime update_date { get; set; }
        public bool is_delete { get; set; }
         public string formatdatetime(DateTime date) {
            return date.ToString("dd/MM/yyyy HH:mm");
        }
        public string formatdate(DateTime date) {
            return date.ToString("dd/MM/yyyy");
        }

         public string formattime(DateTime date) {
            return date.ToString("HH:mm");
        }
    }
}