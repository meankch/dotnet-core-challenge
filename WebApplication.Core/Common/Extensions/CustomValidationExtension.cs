using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Infrastructure.Entities;

namespace WebApplication.Core.Common.Extensions
{
    public static class CustomValidationExtension
    {
        public static IRuleBuilderOptionsConditions<T, int> UserMustExistInDatabase<T>(this IRuleBuilder<T, int> ruleBuilder, DbSet<User> userDb)
        {
            return ruleBuilder.CustomAsync(async (id, context, cancellationToken) =>
            {
                User? user = await userDb
                    .Where(user => user.Id == id)
                    .Include(x => x.ContactDetail)
                    .FirstOrDefaultAsync(cancellationToken);
                if (user is default(User))
                {
                    throw new NotFoundException($"The user '{id}' could not be found.");
                }
            });
        }
    }
}
