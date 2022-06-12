using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Monolito01.Data;
using WebApplication.Monolito01.Models;

namespace WebApplication.Monolito01.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmployeesController
        public IActionResult Index()
        {
            // Consultando la colección de trabajadores.
            IEnumerable<Employee> colEmployees = _context.Employees.Where(s => s.Estate == true); // Código refactorizado para retornar sólo los empleados activos
            return View(colEmployees);
        }

        // GET: Empleadoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var empleado = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // GET: EmployeesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Employees.Add(employee);
                    _context.SaveChanges();
                    TempData["ResultOk"] = "Record added Successfully!";
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(employee); 
            }
        }

        // GET: EmployeesController/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }
            var employeefromdb = _context.Employees.Find(id);

            if (employeefromdb == null)
            {
                return NotFound();
            }
            return View(employeefromdb);
        }


        // POST: EmployeesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    _context.SaveChanges();
                    TempData["ResultOk"] = "Data Updated Successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: EmployeesController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }
            var employeefromdb = _context.Employees.Find(id);

            if (employeefromdb == null)
            {
                return NotFound();
            }
            return View(employeefromdb);
        }

        // POST: EmployeesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
               var employeefromdb = _context.Employees.Find(id);

                if (employeefromdb == null)
                {
                    return NotFound();
                }
                // _context.Employees.Remove(employeefromdb);   // Refactorizado para inactivar el usuario. No eliminarlo.
                employeefromdb.Estate = false;
                _context.Employees.Update(employeefromdb);
                _context.SaveChanges();
                TempData["ResultOk"] = "Data Deleted Successfully!";
                return RedirectToAction(nameof(Index));
           
        }

    }
}
