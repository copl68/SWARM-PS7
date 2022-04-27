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
    public class SchoolController : BaseController<School>, iBaseController<School>
    {
        public SchoolController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<School> lstSchool = await _context.Schools.OrderBy(x => x.SchoolId).ToListAsync();
            return Ok(lstSchool);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            School itmSchool = await _context.Schools.Where(x => x.SchoolId == input).FirstOrDefaultAsync();
            return Ok(itmSchool);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            School itmSchool = await _context.Schools.Where(x => x.SchoolId == input).FirstOrDefaultAsync();
            _context.Remove(itmSchool);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] School input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _schl = await _context.Schools.Where(x => x.SchoolId == input.SchoolId).FirstOrDefaultAsync();

                if (_schl != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "School Already Exists");
                }

                _schl = new School();
                _schl.SchoolId = input.SchoolId;
                _schl.SchoolName = input.SchoolName;

                _context.Schools.Add(_schl);
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
        public async Task<IActionResult> Put([FromBody] School input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var existSchool = await _context.Schools.Where(x => x.SchoolId == input.SchoolId).FirstOrDefaultAsync();

                if (existSchool == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok("Posted");
                }

                existSchool.SchoolId = input.SchoolId;
                existSchool.SchoolName = input.SchoolName;
                _context.Schools.Update(existSchool);
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
