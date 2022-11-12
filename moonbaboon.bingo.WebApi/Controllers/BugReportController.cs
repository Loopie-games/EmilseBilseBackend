using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BugReportController: ControllerBase
    {
        private IBugReportService _bugReportService;

        public BugReportController(IBugReportService bugReportService)
        {
            _bugReportService = bugReportService;
        }
        
        [HttpGet]
        public ActionResult<List<BugReport>> GetAll()
        {
            try
            {
                return Ok(_bugReportService.GetAll());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
        
        [HttpPost(nameof(Create))]
        public ActionResult<BugReport> Create(BugReportEntity pt)
        {
            try
            {
                var created = _bugReportService.Create(pt);
                return CreatedAtAction(nameof(GetById), new {created.Id}, created);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BugReport))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<BugReport> GetById(string id)
        {
            try
            {
                return Ok(_bugReportService.GetById(id));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}