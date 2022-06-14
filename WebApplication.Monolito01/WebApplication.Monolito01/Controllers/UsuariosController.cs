using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Monolito01.Data;
using WebApplication.Monolito01.Models;

namespace WebApplication.Monolito01.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;


        public UsuariosController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles ="Administrador")]
        public async  Task<IActionResult> Index()
        {

            var usuarios = _context.UsuarioRegistrado.ToList();
            var rolesUsuario = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();

            foreach (var item in usuarios)
            {
                var rol = rolesUsuario.FirstOrDefault(u => u.UserId == item.Id);
                if (rol == null)
                {
                    item.Rol = "Ninguno";
                }
                else
                {
                    item.Rol = roles.FirstOrDefault(u => u.Id == rol.RoleId).Name;
                }
            }
            return View(usuarios);
        }



        //Editar usuario (Asignación de rol)
        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult Editar(string id)
        {
            var usuarioBD = 
                _context.UsuarioRegistrado.FirstOrDefault(u => u.Id == id);

            if (usuarioBD == null)
            {
                return NotFound();
            }
            //Obtner los roles actuales del usuario 
            var rolUsuario = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            var rol = rolUsuario.FirstOrDefault(u => u.UserId == usuarioBD.Id);


            if (rol != null)
            {
                usuarioBD.IdRol = roles.FirstOrDefault(u => u.Id == rol.RoleId).Id;
            }
            usuarioBD.ListaRoles = _context.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });

            return View(usuarioBD);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(UsuarioRegistrado usuario)
        {
            if (!ModelState.IsValid)
            {
                var usuarioBD = _context.UsuarioRegistrado.FirstOrDefault(u => u.Id == usuario.Id);
                if (usuarioBD == null)
                {
                    return NotFound();
                }

                var rolUsuario = _context.UserRoles.FirstOrDefault(u => u.UserId == usuarioBD.Id);
                if (rolUsuario != null)
                {
                    //Obtener el rol actual
                    var rolActual = _context.Roles.Where(u => u.Id == rolUsuario.RoleId).Select(e => e.Name).FirstOrDefault();
                    //Eliminar el rol actual
                    await _userManager.RemoveFromRoleAsync(usuarioBD, rolActual);
                }

                //Agregar usuario al nuevo rol seleccionado
                await _userManager.AddToRoleAsync(usuarioBD, _context.Roles.FirstOrDefault(u => u.Id == usuario.IdRol).Name);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            usuario.ListaRoles = _context.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });

            return View(usuario);
        }
    }
}
