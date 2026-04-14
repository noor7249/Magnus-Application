using Magnus.API.Data;
using Magnus.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Magnus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Department>>> GetAll()
    {
        var departments = await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Department>> GetById(int id)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department is null)
        {
            return NotFound(new { message = $"Department with id {id} was not found." });
        }

        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<Department>> Create([FromBody] Department department)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        department.CreatedAt = DateTime.UtcNow;
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Department>> Update(int id, [FromBody] Department request)
    {
        if (id != request.Id)
        {
            return BadRequest(new { message = "Route id and request id must match." });
        }

        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        if (department is null)
        {
            return NotFound(new { message = $"Department with id {id} was not found." });
        }

        department.Name = request.Name;
        department.Description = request.Description;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(department);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
        if (department is null)
        {
            return NotFound(new { message = $"Department with id {id} was not found." });
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
