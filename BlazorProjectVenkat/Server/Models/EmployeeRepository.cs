using BlazorProjectVenkat.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorProjectVenkat.Server.Models
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext appDbContext;

        public EmployeeRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task<Employee> AddEmployee(Employee employee)
        {
            if (employee.Department != null)
            {
                appDbContext.Entry(employee.Department).State = EntityState.Unchanged;
            }

            var result = await appDbContext.Employees.AddAsync(employee);
            await appDbContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task DeleteEmployee(int employeeId)
        {
            var result = await appDbContext.Employees
                .Where(x => x.EmployeeId == employeeId)
                .FirstOrDefaultAsync();

            if (result != null)
            {
                appDbContext.Employees.Remove(result);
                await appDbContext.SaveChangesAsync();
            }
        }

        public async Task<Employee> GetEmployee(int employeeId)
        {
            return await appDbContext.Employees
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            return await appDbContext.Employees
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await appDbContext.Employees.ToListAsync();
        }

        public async Task<IEnumerable<Employee>> Search(string name, Gender? gender)
        {
            IQueryable<Employee> query = appDbContext.Employees;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.FirstName.Contains(name) || x.LastName.Contains(name));
            }

            if (gender != null)
            {
                query = query.Where(x => x.Gender == gender);
            }
            return await query.ToListAsync();

        }

        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            var result = await appDbContext.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employee.EmployeeId);

            if (result != null)
            {
                result.DateOfBrith = employee.DateOfBrith;
                result.Department = employee.Department;
                result.DepartmentId = employee.DepartmentId;
                result.Email = employee.Email;
                result.EmployeeId = employee.EmployeeId;
                result.FirstName = employee.FirstName;
                result.Gender = result.Gender;
                result.LastName = result.LastName;
                result.PhotoPath = result.PhotoPath;

                await appDbContext.SaveChangesAsync();

                return result;
            }

            return null;
        }
    }
}
