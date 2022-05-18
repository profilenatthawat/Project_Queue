using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_2564.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required(ErrorMessage = "กรุณาเลือกสิทธิการใช้งาน")]
        public int role_id {get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้งาน")]
        public String username { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        public String password { get; set; }
        public DateTime create_date { get; set; }
        public DateTime update_date { get; set; }
        public bool is_delete { get; set; }
    }
}

