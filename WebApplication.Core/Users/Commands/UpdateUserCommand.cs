using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Core.Common.Extensions;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Commands
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public int Id { get; set; }
        public string GivenNames { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public class Validator : AbstractValidator<UpdateUserCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0)
                    .UserMustExistInDatabase().When(x => x.Id > 0, ApplyConditionTo.CurrentValidator);

                RuleFor(x => x.GivenNames)
                    .NotEmpty();

                RuleFor(x => x.LastName)
                    .NotEmpty();

                RuleFor(x => x.EmailAddress)
                    .NotEmpty();

                RuleFor(x => x.MobileNumber)
                    .NotEmpty();
            }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, UserDto>
        {
            private readonly ILogger<UpdateUserCommand> _logger;
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(ILogger<UpdateUserCommand> logger,
                IUserService userService, IMapper mapper)
            {
                _logger = logger;
                _userService = userService;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                User? user = await _userService.GetAsync(request.Id, cancellationToken);
                if (user is default(User))
                {
                    _logger.LogError($"The user '{request.Id}' could not be found.");
                    throw new NotFoundException($"The user '{request.Id}' could not be found.");
                }

                user.GivenNames = request.GivenNames;
                user.LastName = request.LastName;

                user.ContactDetail ??= new ContactDetail();
                user.ContactDetail.EmailAddress = request.EmailAddress;
                user.ContactDetail.MobileNumber = request.MobileNumber;

                User updatedUser = await _userService.UpdateAsync(user, cancellationToken);
                UserDto result = _mapper.Map<UserDto>(updatedUser);
                return result;
            }
        }
    }
}
