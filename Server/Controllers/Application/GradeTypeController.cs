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
    public class GradeTypeController : BaseController<GradeType>, iBaseController<GradeType>
    {
        public GradeTypeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<GradeType> lstGradeType = await _context.GradeTypes.OrderBy(x => x.SchoolId).ThenBy(x => x.GradeTypeCode).ToListAsync();
            return Ok(lstGradeType);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot select by single value");
        }

        [HttpGet]
        [Route("Get/{school}/{type}")]
        public async Task<IActionResult> RealGet(int school, String type)
        {
            GradeType itmGradeType = await _context.GradeTypes.Where(x => x.SchoolId == school && x.GradeTypeCode == type).FirstOrDefaultAsync();
            return Ok(itmGradeType);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Composite Primary Key - Cannot delete by single value");
        }

        [HttpDelete]
        [Route("Delete/{school}/{type}")]
        public async Task<IActionResult> RealDelete(int school, String type)
        {
            GradeType itmGradeType = await _context.GradeTypes.Where(x => x.SchoolId == school && x.GradeTypeCode == type).FirstOrDefaultAsync();
            _context.Remove(itmGradeType);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GradeType input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grdtp = await _context.GradeTypes.Where(x => x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode).FirstOrDefaultAsync();

                if (_grdtp != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "GradeType Already Exists");
                }

                _grdtp = new GradeType();
                _grdtp.SchoolId = input.SchoolId;
                _grdtp.Description = input.Description;
                _grdtp.GradeTypeCode = input.GradeTypeCode;
                _context.GradeTypes.Add(_grdtp);
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
        public async Task<IActionResult> Put([FromBody] GradeType input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var existGradeType = await _context.GradeTypes.Where(x => x.SchoolId == input.SchoolId && x.GradeTypeCode == input.GradeTypeCode).FirstOrDefaultAsync();

                if (existGradeType == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok("Posted");
                }

                existGradeType.SchoolId = input.SchoolId;
                existGradeType.Description = input.Description;
                existGradeType.GradeTypeCode = input.GradeTypeCode;
                _context.GradeTypes.Update(existGradeType);
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
