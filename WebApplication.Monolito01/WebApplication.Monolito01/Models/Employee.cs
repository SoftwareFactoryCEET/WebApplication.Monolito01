using System.ComponentModel.DataAnnotations;

namespace WebApplication.Monolito01.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Nombre Empleado")]
        public string Name { get; set; }
        [Display(Name = "Cargo")]
        public string Designation { get; set; }
        [DataType(DataType.MultilineText)]
        public string Adress { get; set; }
        public DateTime? RecordCreation { get; set; }
        public DateTime? RecordUpdateOn { get; set; }
        public bool Estate { get; set; } = true;
    }
}
