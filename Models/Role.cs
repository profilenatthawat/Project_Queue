using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project_2564.Models
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required(ErrorMessage = "กรุณากรอกชื่อสิทธิการใช้งาน")]
        public String name { get; set; }

        // Authorize Function Queue
        public bool allow_view_queue { get; set; }
        public bool allow_add_queue { get; set; }
        public bool allow_edit_queue { get; set; }
        public bool allow_delete_queue { get; set; }

        //  Authorize Function Username
        public bool allow_view_user { get; set; }
        public bool allow_add_user  { get; set; }
        public bool allow_edit_user { get; set; }
        public bool allow_delete_user { get; set; }

        //  Authorize Function Settingstore
        public bool allow_view_settings { get; set; }
        public bool allow_add_settings { get; set; }
        public bool allow_edit_settings { get; set; }
        public bool allow_delete_settings { get; set; }

        // Authorize Function Role 
        public bool allow_view_role { get; set; }
        public bool allow_add_role { get; set; }
        public bool allow_edit_role { get; set; }
        public bool allow_delete_role { get; set; }

        // Authorize Function Report
        public bool allow_report { get; set; }

        public DateTime create_date { get; set; }
        public DateTime update_date { get; set; }
        public bool is_delete { get; set; }
    }
}