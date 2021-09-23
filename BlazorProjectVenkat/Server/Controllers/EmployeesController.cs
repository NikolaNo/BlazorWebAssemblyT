using BlazorProjectVenkat.Server.Models;
using BlazorProjectVenkat.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorProjectVenkat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }
        [HttpGet("all")]
        public async Task<ActionResult> GetAllEmployees()
        {
            try
            {
                return Ok(await employeeRepository.GetAllEmployees());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from database");
            }
        }

        [HttpGet("{search}")]
        public async Task<ActionResult<IEnumerable<Employee>>> Search(string name, Gender? gender) 
        {
            try
            {
                var result = await employeeRepository.Search(name, gender);

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriveing data from the database");
            }
        
        }

        [HttpGet]
        public async Task<ActionResult> GetEmployees(int skip = 0, int take = 5, string orderBy = "EmployeeId")
        {
            try
            {
                return Ok(await employeeRepository.GetEmployees(skip,take, orderBy));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriveing data from the database");
            }            
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            try
            {
                var result = await employeeRepository.GetEmployee(id);

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriveing data from the database");
            }

        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {

            try
            {
                if (employee == null) return BadRequest();

                var emp = await employeeRepository.GetEmployeeByEmail(employee.Email);

                if (emp != null)
                {
                    ModelState.AddModelError("Email", "Employee email is already in use");
                    return BadRequest(ModelState);
                }

                var createdEmployee = await employeeRepository.AddEmployee(employee);

                return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.EmployeeId }, createdEmployee );

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new employee record");
            }

        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, Employee employee)
        {

            try
            {
                if (id != employee.EmployeeId) return BadRequest("Employee id mismatch");

                var employeeToUpdate = await employeeRepository.GetEmployee(id);

                if (employeeToUpdate == null)
                {
                    return NotFound($"Employee with id = {id} not found");
                }

                return await employeeRepository.UpdateEmployee(employee);                

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating employee record");
            }

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {

            try
            {
             

                var employeeToDelete = await employeeRepository.GetEmployee(id);
                if(employeeToDelete == null)
                {
                    return NotFound($"Employee with id = {id} not found");
                }

                await employeeRepository.DeleteEmployee(id);

                return Ok($"Employee with id = {id} deleted");

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleteing employee record");
            }

        }
    }
}
