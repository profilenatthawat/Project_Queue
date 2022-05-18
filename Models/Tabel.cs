using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_2564.Models
{
    public class Table
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public bool status { get; set; }
        public bool is_delete { get; set; }
        public DateTime create_date { get; set; }
        public DateTime update_date { get; set; }

        // Setting

        public DateTime yesterday { get; set; }
        public int counter_today { get; set; }

    }
}