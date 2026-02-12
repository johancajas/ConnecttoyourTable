using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/employees")]
public class EmployeeController : ControllerBase
{
    private readonly EmployeeService _employeeService;

    public EmployeeController(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<List<EmployeeDTO>>> GetEmployees()
    {
        var employees = await _employeeService.GetEmployeesAsync();
        return Ok(employees);
    }

    // GET: api/employees/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDTO>> GetEmployee(Guid id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);

        if (employee == null)
            return NotFound(new { message = $"Employee with ID {id} not found" });

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<EmployeeDTO>> CreateEmployee([FromBody] EmployeeDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _employeeService.CreateEmployeeAsync(dto);
        return CreatedAtAction(nameof(GetEmployee), new { id = created.Id }, created);
    }

    // PUT: api/employees/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeDTO>> UpdateEmployee(Guid id, [FromBody] EmployeeDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _employeeService.UpdateEmployeeAsync(id, dto);

        if (updated == null)
            return NotFound(new { message = $"Employee with ID {id} not found" });

        return Ok(updated);
    }

    // DELETE: api/employees/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee(Guid id)
    {
        var success = await _employeeService.DeleteEmployeeAsync(id);

        if (!success)
            return NotFound(new { message = $"Employee with ID {id} not found" });

        return NoContent();
    }
}
