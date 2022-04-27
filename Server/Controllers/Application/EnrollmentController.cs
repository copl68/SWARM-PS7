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
    public class EnrollmentController : BaseController<Enrollment>, iBaseController<Enrollment>
    {
        public EnrollmentController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Enrollment> lstEnrollment = await _context.Enrollments.OrderBy(x => x.SectionId).ThenBy(x => x.StudentId).ToListAsync();
            return Ok(lstEnrollment);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot select by single value");
        }

        [HttpGet]
        [Route("Get/{section}/{student}")]
        public async Task<IActionResult> RealGet(int section, int student)
        { 
            Enrollment itmEnrollment = await _context.Enrollments.Where(x => x.SectionId == section && x.StudentId == student).FirstOrDefaultAsync();
            return Ok(itmEnrollment);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot delete by single value");
        }

        [HttpDelete]
        [Route("Delete/{section}/{student}")]
        public async Task<IActionResult> RealDelete(int section, int student)
        {
            Enrollment itmEnrollment = await _context.Enrollments.Where(x => x.SectionId == section && x.StudentId == student).FirstOrDefaultAsync();
            _context.Remove(itmEnrollment);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Enrollment input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _enr = await _context.Enrollments.Where(x => x.SectionId == input.SectionId && x.StudentId == input.StudentId).FirstOrDefaultAsync();

                if (_enr != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment Already Exists");
                }

                _enr = new Enrollment();
                _enr.StudentId = input.StudentId;
                _enr.SectionId = input.SectionId;
                _enr.EnrollDate = input.EnrollDate;
                _enr.SchoolId = input.SchoolId;
                _context.Enrollments.Add(_enr);
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
        public async Task<IActionResult> Put([FromBody] Enrollment input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var existEnrollment = await _context.Enrollments.Where(x => x.SectionId == input.SectionId && x.StudentId == input.StudentId).FirstOrDefaultAsync();

                if (existEnrollment == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok("Posted");
                }

                existEnrollment.StudentId = input.StudentId;
                existEnrollment.SectionId = input.SectionId;
                existEnrollment.EnrollDate = input.EnrollDate;
                existEnrollment.SchoolId = input.SchoolId;
                _context.Enrollments.Update(existEnrollment);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(input.SectionId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
