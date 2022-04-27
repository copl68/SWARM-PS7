using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWARM.EF.Data;
using SWARM.EF.Models;
using SWARM.Server.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWARM.Server.Controllers.Application
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : BaseController<Student>, iBaseController<Student>
    {
        public StudentController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Student> lstStudent = await _context.Students.OrderBy(x => x.StudentId).ToListAsync();
            return Ok(lstStudent);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            Student itmStudent = await _context.Students.Where(x => x.StudentId == input).FirstOrDefaultAsync();
            return Ok(itmStudent);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            Student itmStudent = await _context.Students.Where(x => x.StudentId == input).FirstOrDefaultAsync();
            _context.Remove(itmStudent);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _stdnt = await _context.Students.Where(x => x.StudentId == input.StudentId).FirstOrDefaultAsync();

                if (_stdnt != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Student Already Exists");
                }

                _stdnt = new Student();
                _stdnt.StudentId = input.StudentId;
                _stdnt.SchoolId = input.SchoolId;
                _stdnt.LastName = input.LastName;
                _stdnt.Zip = input.Zip;
                _stdnt.RegistrationDate = input.RegistrationDate;


                _context.Students.Add(_stdnt);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok("Done");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Student input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var existStudent = await _context.Students.Where(x => x.StudentId == input.StudentId).FirstOrDefaultAsync();

                if (existStudent == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok("Posted");
                }

                existStudent.StudentId = input.StudentId;
                existStudent.SchoolId = input.SchoolId;
                existStudent.LastName = input.LastName;
                existStudent.Zip = input.Zip;
                existStudent.RegistrationDate = input.RegistrationDate;
                _context.Students.Update(existStudent);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok("Done");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
