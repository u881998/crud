using crud_operation.Data;
using crud_operation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace crud_operation.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

       
        public IActionResult Index()
        {
            var employees = _context.Employees.ToList();
            return View(employees);
        }

      
        public IActionResult Details(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

     
        public IActionResult Create()
        {
            return View();
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee, IFormFile? resumeFile)
        {
            if (ModelState.IsValid)
            {
              
                if (resumeFile != null && resumeFile.Length > 0)
                {
                   
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", resumeFile.FileName);

                   
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await resumeFile.CopyToAsync(fileStream);
                    }

                    
                    employee.ResumePath = filePath;
                }

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

      
        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee, IFormFile? resumeFile)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   
                    if (resumeFile != null && resumeFile.Length > 0)
                    {
                        
                        if (!string.IsNullOrEmpty(employee.ResumePath) && System.IO.File.Exists(employee.ResumePath))
                        {
                            System.IO.File.Delete(employee.ResumePath);
                        }

                        
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", resumeFile.FileName);

                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await resumeFile.CopyToAsync(fileStream);
                        }

                       
                        employee.ResumePath = filePath;
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
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
            return _context.Employees.Any(e => e.Id == id);
        }

     
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound();

         
            _context.Employees.Remove(employee);
            _context.SaveChanges();

          
            return RedirectToAction("Index");
        }

    }
}
