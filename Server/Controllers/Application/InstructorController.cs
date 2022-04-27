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
    public class InstructorController : BaseController<Instructor>, iBaseController<Instructor>
    {
        public InstructorController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Instructor> lstInstructor = await _context.Instructors.OrderBy(x => x.SchoolId).ThenBy(x => x.InstructorId).ToListAsync();
            return Ok(lstInstructor);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot select by single value");
        }

        [HttpGet]
        [Route("Get/{school}/{instructor}")]
        public async Task<IActionResult> RealGet(int school, int instructor)
        {
            Instructor itmInstructor = await _context.Instructors.Where(x => x.SchoolId == school && x.InstructorId == instructor).FirstOrDefaultAsync();
            return Ok(itmInstructor);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot delete by single value");
        }

        [HttpDelete]
        [Route("Delete/{school}/{instructor}")]
        public async Task<IActionResult> RealDelete(int school, int instructor)
        {
            Instructor itmInstructor = await _context.Instructors.Where(x => x.SchoolId == school && x.InstructorId == instructor).FirstOrDefaultAsync();
            _context.Remove(itmInstructor);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Instructor input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _instr = await _context.Instructors.Where(x => x.SchoolId == input.SchoolId && x.InstructorId == input.InstructorId).FirstOrDefaultAsync();

                if (_instr != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Instructor Already Exists");
                }

                _instr = new Instructor();
                _instr.SchoolId = input.SchoolId;
                _instr.InstructorId = input.InstructorId;
                _instr.FirstName = input.FirstName;
                _instr.LastName = input.LastName;
                _instr.StreetAddress = input.StreetAddress;
                _instr.Zip = input.Zip;

                _context.Instructors.Add(_instr);
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
        public async Task<IActionResult> Put([FromBody] Instructor input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var existInstructor = await _context.Instructors.Where(x => x.SchoolId == input.SchoolId && x.InstructorId == input.InstructorId).FirstOrDefaultAsync();

                if (existInstructor == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok("Posted");
                }

                existInstructor.SchoolId = input.SchoolId;
                existInstructor.InstructorId = input.InstructorId;
                existInstructor.FirstName = input.FirstName;
                existInstructor.LastName = input.LastName;
                existInstructor.StreetAddress = input.StreetAddress;
                existInstructor.Zip = input.Zip;
                _context.Instructors.Update(existInstructor);
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
