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
    public class GradeController : BaseController<Grade>, iBaseController<Grade>
    {
        public GradeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Grade> lstGrade = await _context.Grades.OrderBy(x => x.SectionId).ThenBy(x => x.StudentId).ThenBy(x => x.SchoolId).ThenBy(x => x.GradeTypeCode).ThenBy(x => x.GradeCodeOccurrence).ToListAsync();
            return Ok(lstGrade);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot select by single value");
        }

        [HttpGet]
        [Route("Get/{section}/{student}/{school}/{code}/{occurrence}")]
        public async Task<IActionResult> RealGet(int section, int student, int school, string code, int occurrence)
        {
            Grade itmGrade = await _context.Grades.Where(x => x.SectionId == section && x.StudentId == student && x.SchoolId == school && x.GradeCodeOccurrence == occurrence && x.GradeTypeCode == code).FirstOrDefaultAsync();
            return Ok(itmGrade);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot delete by single value");
        }

        [HttpDelete]
        [Route("Get/{section}/{student}/{school}/{code}/{occurrence}")]
        public async Task<IActionResult> RealDelete(int section, int student, int school, string code, int occurrence)
        {
            Grade itmGrade = await _context.Grades.Where(x => x.SectionId == section && x.StudentId == student && x.SchoolId == school && x.GradeCodeOccurrence == occurrence && x.GradeTypeCode == code).FirstOrDefaultAsync();
            _context.Remove(itmGrade);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Grade input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grd = await _context.Grades.Where(x => x.SectionId == input.SectionId && x.StudentId == input.StudentId && x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode && x.GradeCodeOccurrence == input.GradeCodeOccurrence).FirstOrDefaultAsync();

                if (_grd != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Grade Already Exists");
                }

                _grd = new Grade();
                _grd.StudentId = input.StudentId;
                _grd.SectionId = input.SectionId;
                _grd.GradeTypeCode = input.GradeTypeCode;
                _grd.GradeCodeOccurrence = input.GradeCodeOccurrence;
                _grd.SchoolId = input.SchoolId;
                _context.Grades.Add(_grd);
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
        public async Task<IActionResult> Put([FromBody] Grade input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var existGrade = await _context.Grades.Where(x => x.SectionId == input.SectionId && x.StudentId == input.StudentId && x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode && x.GradeCodeOccurrence == input.GradeCodeOccurrence).FirstOrDefaultAsync();

                if (existGrade == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok("Posted");
                }

                existGrade.StudentId = input.StudentId;
                existGrade.SectionId = input.SectionId;
                existGrade.GradeTypeCode = input.GradeTypeCode;
                existGrade.GradeCodeOccurrence = input.GradeCodeOccurrence;
                existGrade.SchoolId = input.SchoolId;
                _context.Grades.Update(existGrade);
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
