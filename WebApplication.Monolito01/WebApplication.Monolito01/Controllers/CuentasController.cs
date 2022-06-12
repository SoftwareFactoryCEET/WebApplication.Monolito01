using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Monolito01.Models;
using WebApplication.Monolito01.Models.ViewModels;

namespace WebApplication.Monolito01.Controllers
{
    public class CuentasController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;        // Manejador de Usuarios
        private readonly IEmailSender _emailSender;                     // Interfaz para manejo de Email
        private readonly SignInManager<IdentityUser> _signInManager;    // Manejador de autenticación


        // Crear un constructor
        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Registro(string? returnurl = null)
        {
            //Pendiente implementar código para crear roles.

            ViewData["ReturnUrl"] = returnurl;
            var registroViewModel = new RegisterViewModel();
            return View(registroViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegisterViewModel registroViewModel, string? returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");


            if (ModelState.IsValid)
            {

                var usuario = new UsuarioRegistrado
                {
                    Nombres = registroViewModel.Nombres,
                    Apellidos = registroViewModel.Apellidos,
                    UserName = registroViewModel.Email,
                    Email = registroViewModel.Email,
                    URL = registroViewModel.URL,
                    CodigoPais = registroViewModel.CodigoPais,
                    Pais = registroViewModel.Pais,
                    PhoneNumber = registroViewModel.PhoneNumber,
                    Ciudad = registroViewModel.Ciudad,
                    Direccion = registroViewModel.Direccion,
                    FechaNacimiento = registroViewModel.FechaNacimiento,
                    Estado = registroViewModel.Estado
                };

                var resultado = await _userManager.CreateAsync(usuario, registroViewModel.Password); // Manejador de usuarios del Framework de Identity

                if (resultado.Succeeded)
                {
                    //Pendiente agregar el usuario a rol por defecto


                    // Confirmación por email...
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
                    var urlRetorno = Url.Action("ConfirmarEmail", "Cuentas", new { userId = usuario.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(
                                                        registroViewModel.Email,
                                                        "Confirmar su cuenta - WebApplicationsEmployess",
                                                        "Por favor confirme su cuenta dando click aquí: <a href=\"" + urlRetorno + "\">enlace</a>"
                                                    );
                    await _signInManager.SignInAsync(usuario, isPersistent: false);  // Manejador de autenticación del Framework de Identidad (Identity)
                    //return LocalRedirect(returnurl);
                    return RedirectToAction("Index", "Home");
                }
                ValidarErrores(resultado);
            }
            return View(registroViewModel);
        }

        [AllowAnonymous]
        private void ValidarErrores(IdentityResult resultado)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                return View("Error");
            }
            var resultado = await _userManager.ConfirmEmailAsync(usuario, code);
            return View(resultado.Succeeded ? "ConfirmarEmail" : "Error");
        }

      
    }
}
