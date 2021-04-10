using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using TenisApp.DataAccess.Repository;
using TenisApp.Shared.ViewModel;

namespace TenisApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CourtController : ControllerBase
    {
        private readonly ILogger<CourtController> _logger;
        private readonly ICourtRepository _repository;

        public CourtController(ILogger<CourtController> logger, ICourtRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _repository.GetCourts();
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database problem occured!", statusCode: 500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([Required] int id)
        {
            try
            {
                var result = await _repository.GetCourt(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database problem occured!", statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([Required] CourtViewModel model)
        {
            try
            {
                var result = await _repository.AddCourt(model.Court);
                model.Court = result;
                return Ok(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database problem occured!", statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([Required] CourtViewModel model)
        {
            try
            {
                await _repository.UpdateCourt(model.Court);
                return Ok(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database problem occured!", statusCode: 500);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] CourtViewModel model)
        {
            try
            {
                await _repository.DeleteCourt(model.Court);
                return Ok(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database problem occured!", statusCode: 500);
            }
        }
    }
}
