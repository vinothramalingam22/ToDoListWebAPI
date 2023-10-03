﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListMicroService.Filter;
using ToDoListMicroService.Models;
using ToDoListMicroService.Services;
using ToDoListMicroService.Util;
using ToDoListMicroService.Queries;
using ToDoListMicroService.Commands;

namespace ToDoListMicroService.Controllers
{    
    /// <summary>
    /// ToDoList
    /// </summary>
    /// <response code="200">All good</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="204">No content</response>
    [ApiController]
    [Route("todolist/api/v1/user")]
    [Authorize]
    public class ToDoListController : ControllerBase
    {
        #region Private declarations
        private readonly ISender _mediator;
        private readonly ITokenService _tokenService;
        private readonly ILogger<ToDoListController> _logger;
        #endregion

        #region Public Methods
        public ToDoListController(ITokenService tokenService, ISender mediator, ILogger<ToDoListController> logger)
        {            
            _mediator = mediator;
            _tokenService= tokenService;
            _logger = logger;
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("list/all")]
        public async Task<ActionResult<List<ToDoListResponseModel>>> Get([FromQuery] PaginationFilter filter)
        {
            try
            {
                var queryRequest = new GetAllToDoListQuery()
                {
                    SearchTerm= filter.SearchTerm,
                    PageNumber= filter.PageNumber,
                    PageSize = filter.PageSize,
                    UserId = _tokenService.GetName()
                };

                var toDoList = await _mediator.Send(queryRequest);
                
                return Ok(toDoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("list/{taskName}")]
        public async Task<ActionResult<ToDoListResponseModel>> Get(string taskName)
        {
            try
            {
                var userId =  _tokenService.GetName();

                var request = new GetToDoListDetailsQuery()
                {
                    UserId = userId,
                    TaskName = taskName
                };

                var taskList = await _mediator.Send(request);

                return Ok(taskList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
               
        [HttpPost]
        [Route("add-list")]
        public async Task<ActionResult> Post([FromBody] ToDoListRequestModel model)
        {
            try
            {
                var userId = _tokenService.GetName();

                var createRequest = new CreateToDoListRequest()
                {
                    Name= model.Name,
                    Description = model.Description,
                    Status= model.Status,
                    TotalEffort = model.TotalEffort,
                    StartDate= model.StartDate,
                    EndDate= model.EndDate,
                    UserId = userId
                };

                await _mediator.Send(createRequest);

                return Ok(StatusCodes.Status201Created);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }
       
        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] string taskStatus, string id)
        {
            try
            {
                if(!Utilities.isValidStatus(taskStatus))
                {
                    _logger.LogError("Invalid Status", taskStatus);
                    return StatusCode(StatusCodes.Status400BadRequest, "Invalid status");
                }

                var updateToDoListRequest = new UpdateToDoListRequest()
                {
                    Id = id,
                    Status = taskStatus
                };

                var response = await _mediator.Send(updateToDoListRequest); 

                if (response != null)
                {
                    return Ok(StatusCodes.Status204NoContent);
                }
                _logger.LogError("Not Found", id);
                return StatusCode(StatusCodes.Status404NotFound, "Not Found");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex?.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }
        #endregion Public Methods
    }
}
