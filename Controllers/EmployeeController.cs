using crud_operation.Data;
using crud_operation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace crud_operation.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Employee
        public IActionResult Index()
        {
            var employees = _context.Employees.ToList();
            return View(employees);
        }

        // GET: Employee/Details/5
        public IActionResult Details(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee, IFormFile? resumeFile)
        {
            if (ModelState.IsValid)
            {
                // Check if a file is uploaded
                if (resumeFile != null && resumeFile.Length > 0)
                {
                    // Set the path to save the file
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", resumeFile.FileName);

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await resumeFile.CopyToAsync(fileStream);
                    }

                    // Save the file path in the database
                    employee.ResumePath = filePath;
                }

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employee/Edit/5
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
                    // If a new file is uploaded
                    if (resumeFile != null && resumeFile.Length > 0)
                    {
                        // Delete the old file if it exists
                        if (!string.IsNullOrEmpty(employee.ResumePath) && System.IO.File.Exists(employee.ResumePath))
                        {
                            System.IO.File.Delete(employee.ResumePath);
                        }

                        // Set the path to save the new file
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", resumeFile.FileName);

                        // Save the new file to the server
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await resumeFile.CopyToAsync(fileStream);
                        }

                        // Save the new file path in the database
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

        // GET: Employee/Delete/5
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound();

            // Delete the employee and save the changes
            _context.Employees.Remove(employee);
            _context.SaveChanges();

            // Redirect to the Index page after deletion
            return RedirectToAction("Index");
        }

    }
}
