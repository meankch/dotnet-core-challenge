using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Commands;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Core.Users.Queries;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ActionName("GetUserAsync")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync(
                       [FromQuery] GetUserQuery query,
                       CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("find")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FindUsersAsync(
                       [FromQuery] FindUsersQuery query,
                       CancellationToken cancellationToken)
        {
            IEnumerable<UserDto> result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(PaginatedDto<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListUsersAsync(
                        [FromQuery] ListUsersQuery query,
                        CancellationToken cancellationToken)
        {
            PaginatedDto<IEnumerable<UserDto>> result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUserAsync(
                        [FromBody] CreateUserCommand command,
                        CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(command, cancellationToken);
            return Created($"/Users?id={result.UserId}", result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync(
                        [FromBody] UpdateUserCommand command,
                        CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserAsync(
                        [FromQuery] DeleteUserCommand command,
                        CancellationToken cancellationToken)
        {
            UserDto result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
