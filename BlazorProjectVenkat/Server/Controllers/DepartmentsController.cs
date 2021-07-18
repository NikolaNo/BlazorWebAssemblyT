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
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository departmentRepository;

        public DepartmentsController(IDepartmentRepository departmentRepository)
        {
            this.departmentRepository = departmentRepository;
        }
        [HttpGet]
        public async Task<ActionResult> GetDepartmentsAsync()
        {
            try
            {
                IEnumerable<Department> departments = await departmentRepository.GetDepartments();

                return Ok(departments);
            }
            catch (Exception x)
            {
                return NotFound(x);
            }
        
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            try
            {
                return Ok(await departmentRepository.GetDepartment(id));
            }
            catch (Exception)
            {

                return NotFound();
            }
        }


    }
}
