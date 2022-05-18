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
    public class Report
    {
        public DateTime bookdate { get; set; }
        public int sum_a { get; set; }
        public int sum_b { get; set; }
        public int sum_c { get; set; }
        public int total { get; set; }
        public string formatdate() {
            return bookdate.ToString("dd/MM/yyyy");
        }

    }

}