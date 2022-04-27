using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWARM.EF.Data;
using SWARM.EF.Models;
using SWARM.Server.Controllers.Base;
using SWARM.Server.Models;
using SWARM.Shared;
using SWARM.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
//using Telerik.DataSource.Trial;
//using Telerik.DataSource.Extensions.Trial;

namespace SWARM.Server.Controllers.Application
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : BaseController<Course>, iBaseController<Course>
    {

        public CourseController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor) 
            :base (context, httpContextAccessor) 
        {
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get()
        {
            List<Course> lstCourses = await _context.Courses.OrderBy(x => x.CourseNo).ToListAsync();
            return Ok(lstCourses);
        }

        [HttpGet]
        [Route("Get/{input}")]
        public async Task<IActionResult> Get(int input)
        {
            Course itmCourse = await _context.Courses.Where(x => x.CourseNo == input).FirstOrDefaultAsync();
            return Ok(itmCourse);
        }

        [HttpDelete]
        [Route("Delete/{input}")]
        public async Task<IActionResult> Delete(int input)
        {
            Course itmCourse = await _context.Courses.Where(x => x.CourseNo == input).FirstOrDefaultAsync();
            _context.Remove(itmCourse);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Course input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _crse = await _context.Courses.Where(x => x.CourseNo == input.CourseNo).FirstOrDefaultAsync();

                if(_crse != null)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Course Already Exists");
                }

                _crse = new Course();
                _crse.Cost = input.Cost;
                _crse.Description = input.Description;
                _crse.Prerequisite = input.Prerequisite;
                _crse.PrerequisiteSchoolId = input.PrerequisiteSchoolId;
                _crse.SchoolId = input.SchoolId;
                _context.Courses.Add(_crse);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(input.CourseNo);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Course input)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var existCourse = await _context.Courses.Where(x => x.CourseNo == input.CourseNo).FirstOrDefaultAsync();

                if (existCourse == null)
                {
                    trans.Rollback();
                    await Post(input);
                    return Ok(input.CourseNo);
                }

                existCourse.Cost = input.Cost;
                existCourse.Description = input.Description;
                existCourse.Prerequisite = input.Prerequisite;
                existCourse.PrerequisiteSchoolId = input.PrerequisiteSchoolId;
                existCourse.SchoolId = input.SchoolId;
                _context.Courses.Update(existCourse);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(input.CourseNo);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[HttpPost]
        //[Route("GetCourses")]
        //public async Task<DataEnvelope<CourseDTO>> GetPost([FromBody] DataSourceRequest gridRequest)
        //{
        //    DataEnvelope<CourseDTO> dataToReturn = null;
        //    IQueryable<CourseDTO> queriableStates = _context.Courses
        //            .Select(sp => new CourseDTO
        //            {
        //                Cost = sp.Cost,
        //                CourseNo = sp.CourseNo,
        //                CreatedBy = sp.CreatedBy,
        //                CreatedDate = sp.CreatedDate,
        //                Description = sp.Description,
        //                ModifiedBy = sp.ModifiedBy,
        //                ModifiedDate = sp.ModifiedDate,
        //                Prerequisite = sp.Prerequisite,
        //                PrerequisiteSchoolId = sp.PrerequisiteSchoolId,
        //                SchoolId = sp.SchoolId
        //            });

        //    // use the Telerik DataSource Extensions to perform the query on the data
        //    // the Telerik extension methods can also work on "regular" collections like List<T> and IQueriable<T>
        //    try
        //    {

        //        DataSourceResult processedData = await queriableStates.ToDataSourceResultAsync(gridRequest);

        //        if (gridRequest.Groups.Count > 0)
        //        {
        //            // If there is grouping, use the field for grouped data
        //            // The app must be able to serialize and deserialize it
        //            // Example helper methods for this are available in this project
        //            // See the GroupDataHelper.DeserializeGroups and JsonExtensions.Deserialize methods
        //            dataToReturn = new DataEnvelope<CourseDTO>
        //            {
        //                GroupedData = processedData.Data.Cast<AggregateFunctionsGroup>().ToList(),
        //                TotalItemCount = processedData.Total
        //            };
        //        }
        //        else
        //        {
        //            // When there is no grouping, the simplistic approach of 
        //            // just serializing and deserializing the flat data is enough
        //            dataToReturn = new DataEnvelope<CourseDTO>
        //            {
        //                CurrentPageData = processedData.Data.Cast<CourseDTO>().ToList(),
        //                TotalItemCount = processedData.Total
        //            };
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //fixme add decent exception handling
        //    }
        //    return dataToReturn;
        //}

    }
}
