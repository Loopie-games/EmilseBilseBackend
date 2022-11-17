using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BugReportController: ControllerBase
    {
        private readonly IBugReportService _bugReportService;

        public BugReportController(IBugReportService bugReportService)
        {
            _bugReportService = bugReportService;
        }
        
        [Authorize(Roles = nameof(Admin))]
        [HttpGet]
        public ActionResult<List<BugReport>> GetAll()
        {
            try
            {
                return _bugReportService.GetAll(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpPost(nameof(Create))]
        public ActionResult<BugReport> Create(UserBugReportDto pt)
        {
            try
            {
                var created = _bugReportService.Create(new BugReportEntity(null, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, pt.Title, pt.Description));
                return CreatedAtAction(nameof(GetById), new {created.Id}, created);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return BadRequest(e.Message);
            }
        }
        [Authorize(Roles = nameof(Admin))]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BugReport))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<BugReport> GetById(string id)
        {
            try
            {
                return Ok(_bugReportService.GetById(id, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}