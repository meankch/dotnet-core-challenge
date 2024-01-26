using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Commands
{
    public class DeleteUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }

        public class Validator : AbstractValidator<DeleteUserCommand>
        {
            private readonly ILogger<DeleteUserCommand> _logger;
            private readonly IUserService _userService;

            public Validator(ILogger<DeleteUserCommand> logger, IUserService userService)
            {
                _logger = logger;
                _userService = userService;

                RuleFor(x => x.Id)
                    .GreaterThan(0);

                When(x => x.Id > 0, () =>
                {
                    RuleFor(x => x.Id)
                        .CustomAsync(async (id, context, cancellationToken) =>
                        {
                            User? result = await _userService.GetAsync(id, cancellationToken);
                            if (result is default(User))
                            {
                                _logger.LogError($"The user '{id}' could not be found.");
                                throw new NotFoundException($"The user '{id}' could not be found.");
                            }
                        });
                });
            }
        }

        public class Handler : IRequestHandler<DeleteUserCommand, UserDto>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<UserDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                User? deletedUser = await _userService.DeleteAsync(request.Id, cancellationToken);
                return _mapper.Map<UserDto>(deletedUser);
            }
        }
    }
}
