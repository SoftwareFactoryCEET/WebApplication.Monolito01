using System.ComponentModel.DataAnnotations;

namespace WebApplication.Monolito01.Models.ViewModels
{
    public class VerificarAutenticadorViewModel
    {
        [Required]
        [Display(Name = "Código del autenticador")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Recordar datos?")]
        public bool RecordarDatos { get; set; }
    }
}
