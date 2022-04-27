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
    public class ZipcodeController : BaseController<Zipcode>, iBaseController<Zipcode>
    {
        public ZipcodeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Zipcode> lstStudent = await _context.Zipcodes.OrderBy(x => x.Zip).ToListAsync();
            return Ok(lstStudent);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            String _input = input.ToString();
            Zipcode itmZipcode = await _context.Zipcodes.Where(x => x.Zip == _input).FirstOrDefaultAsync();
            return Ok(itmZipcode);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            String _input = input.ToString();
            Zipcode itmZipcode = await _context.Zipcodes.Where(x => x.Zip == _input).FirstOrDefaultAsync();
            _context.Remove(itmZipcode);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Zipcode input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _zp = await _context.Zipcodes.Where(x => x.Zip == input.Zip).FirstOrDefaultAsync();

                if (_zp != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Student Already Exists");
                }

                _zp = new Zipcode();
                _zp.Zip = input.Zip;


                _context.Zipcodes.Add(_zp);
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
        public async Task<IActionResult> Put([FromBody] Zipcode input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var existZip = await _context.Zipcodes.Where(x => x.Zip == input.Zip).FirstOrDefaultAsync();

                if (existZip == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok("Posted");
                }

                existZip.Zip = input.Zip;
                _context.Zipcodes.Update(existZip);
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
