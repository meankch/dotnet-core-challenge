using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Queries
{
    public class ListUsersQuery : IRequest<PaginatedDto<IEnumerable<UserDto>>>
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; } = 10;

        public class Validator : AbstractValidator<ListUsersQuery>
        {
            public Validator()
            {
                RuleFor(query => query.PageNumber)
                    .Must(value => value > 0)
                    .WithMessage("'Page Number' must be greater than '0'.");
            }
        }

        public class Handler : IRequestHandler<ListUsersQuery, PaginatedDto<IEnumerable<UserDto>>>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public Handler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            /// <inheritdoc />
            public async Task<PaginatedDto<IEnumerable<UserDto>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
            {
                var totalUsers = await _userService.CountAsync(cancellationToken);
                var totalPages = Math.Ceiling(totalUsers / (double)request.ItemsPerPage);

                IEnumerable<User>? users = await _userService.GetPaginatedAsync(request.PageNumber, request.ItemsPerPage, cancellationToken);

                return new PaginatedDto<IEnumerable<UserDto>>
                {
                    Data = _mapper.Map<IEnumerable<UserDto>>(users),
                    HasNextPage = request.PageNumber < totalPages,
                };
            }
        }
    }
}
