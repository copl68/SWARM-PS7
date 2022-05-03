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
    public class GradeConversionController : BaseController<GradeConversion>, iBaseController<GradeConversion>
    {
        public GradeConversionController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<GradeConversion> lstGradeConversion = await _context.GradeConversions.OrderBy(x => x.SchoolId).ThenBy(x => x.LetterGrade).ToListAsync();
            return Ok(lstGradeConversion);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot select by single value");
        }

        [HttpGet]
        [Route("Get/{school}/{grade}")]
        public async Task<IActionResult> RealGet(int school, String grade)
        {
            GradeConversion itmGradeConversion = await _context.GradeConversions.Where(x => x.SchoolId == school && x.LetterGrade == grade).FirstOrDefaultAsync();
            return Ok(itmGradeConversion);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot delete by single value");
        }

        [HttpDelete]
        [Route("Delete/{school}/{grade}")]
        public async Task<IActionResult> RealDelete(int school, String grade)
        {
            GradeConversion itmGradeConversion = await _context.GradeConversions.Where(x => x.SchoolId == school && x.LetterGrade == grade).FirstOrDefaultAsync();
            _context.Remove(itmGradeConversion);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GradeConversion input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grdcv = await _context.GradeConversions.Where(x => x.SchoolId == input.SchoolId && x.LetterGrade == input.LetterGrade).FirstOrDefaultAsync();

                if (_grdcv != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "GradeConversion Already Exists");
                }

                _grdcv = new GradeConversion();
                _grdcv.SchoolId = input.SchoolId;
                _grdcv.LetterGrade = input.LetterGrade;
                _grdcv.GradePoint = input.GradePoint;
                _grdcv.MinGrade = input.MinGrade;
                _grdcv.MaxGrade = input.MaxGrade;
                _context.GradeConversions.Add(_grdcv);
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
        public async Task<IActionResult> Put([FromBody] GradeConversion input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var existGradeConversion = await _context.GradeConversions.Where(x => x.SchoolId == input.SchoolId && x.LetterGrade == input.LetterGrade).FirstOrDefaultAsync();

                if (existGradeConversion == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok("Posted");
                }

                existGradeConversion.SchoolId = input.SchoolId;
                existGradeConversion.LetterGrade = input.LetterGrade;
                existGradeConversion.GradePoint = input.GradePoint;
                existGradeConversion.MinGrade = input.MinGrade;
                existGradeConversion.MaxGrade = input.MaxGrade;
                _context.GradeConversions.Update(existGradeConversion);
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
