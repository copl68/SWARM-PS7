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
    public class GradeTypeWeightController : BaseController<GradeTypeWeight>, iBaseController<GradeTypeWeight>
    {
        public GradeTypeWeightController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<GradeTypeWeight> lstGradeTypeWeight = await _context.GradeTypeWeights.OrderBy(x => x.SchoolId).ThenBy(x => x.SectionId).ThenBy(x => x.GradeTypeCode).ToListAsync();
            return Ok(lstGradeTypeWeight);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot select by single value");
        }

        [HttpGet]
        [Route("Get/{school}/{section}/{type}")]
        public async Task<IActionResult> RealGet(int school, int section, String type)
        {
            GradeTypeWeight itmGradeTypeWeight = await _context.GradeTypeWeights.Where(x => x.SchoolId == school && x.SectionId == section && x.GradeTypeCode == type).FirstOrDefaultAsync();
            return Ok(itmGradeTypeWeight);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot delete by single value");
        }

        [HttpDelete]
        [Route("Delete/{school}/{section}/{type}")]
        public async Task<IActionResult> RealDelete(int school, int section, String type)
        {
            GradeTypeWeight itmGradeTypeWeight = await _context.GradeTypeWeights.Where(x => x.SchoolId == school && x.SectionId == section && x.GradeTypeCode == type).FirstOrDefaultAsync();
            _context.Remove(itmGradeTypeWeight);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GradeTypeWeight input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grdtpwt = await _context.GradeTypeWeights.Where(x => x.SectionId == input.SectionId && x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode).FirstOrDefaultAsync();

                if (_grdtpwt != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "GradeTypeWeight Already Exists");
                }

                _grdtpwt = new GradeTypeWeight();
                _grdtpwt.NumberPerSection = input.NumberPerSection;
                _grdtpwt.PercentOfFinalGrade = input.PercentOfFinalGrade;
                _grdtpwt.DropLowest = input.DropLowest;
                _grdtpwt.SectionId = input.SectionId;
                _grdtpwt.GradeTypeCode = input.GradeTypeCode;
                _grdtpwt.SchoolId = input.SchoolId;
                _context.GradeTypeWeights.Add(_grdtpwt);
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
        public async Task<IActionResult> Put([FromBody] GradeTypeWeight input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var existGradeTypeWeight = await _context.GradeTypeWeights.Where(x => x.SectionId == input.SectionId && x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode).FirstOrDefaultAsync();

                if (existGradeTypeWeight == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok("Posted");
                }

                existGradeTypeWeight.NumberPerSection = input.NumberPerSection;
                existGradeTypeWeight.PercentOfFinalGrade = input.PercentOfFinalGrade;
                existGradeTypeWeight.DropLowest = input.DropLowest;
                existGradeTypeWeight.SectionId = input.SectionId;
                existGradeTypeWeight.GradeTypeCode = input.GradeTypeCode;
                existGradeTypeWeight.SchoolId = input.SchoolId;
                _context.GradeTypeWeights.Update(existGradeTypeWeight);
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
