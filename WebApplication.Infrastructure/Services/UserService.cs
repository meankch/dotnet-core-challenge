using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication.Infrastructure.Contexts;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly InMemoryContext _dbContext;

        public UserService(ILogger<UserService> logger,
            InMemoryContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;

            // this is a hack to seed data into the in memory database. Do not use this in production.
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
            {
                _dbContext.Database.EnsureCreated();
            }
        }

        /// <inheritdoc />
        public async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            User? user = await _dbContext.Users
                    .Where(user => user.Id == id)
                    .Include(x => x.ContactDetail)
                    .FirstOrDefaultAsync(cancellationToken);
            return user;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> FindAsync(string? givenNames, string? lastName, CancellationToken cancellationToken = default)
        {
            IEnumerable<User> results = await _dbContext.Users
                    .Where(user =>
                        (givenNames != null && user.GivenNames.Contains(givenNames))
                        || (lastName != null && user.LastName.Contains(lastName)))
                    .Include(user => user.ContactDetail)
                    .ToListAsync(cancellationToken);
            return results;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetPaginatedAsync(int page, int count, CancellationToken cancellationToken = default)
        {
            IEnumerable<User> results = await _dbContext.Users
                    .Skip((page - 1) * count)
                    .Take(count)
                    .Include(user => user.ContactDetail)
                    .ToListAsync(cancellationToken);
            return results;
        }

        /// <inheritdoc />
        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            var addedUser = await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return addedUser.Entity;
        }

        /// <inheritdoc />
        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            var updatedUser = _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return updatedUser.Entity;
        }

        /// <inheritdoc />
        public async Task<User?> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            User? user = await GetAsync(id, cancellationToken);
            if (user is default(User))
            {
                _logger.LogError($"The user '{id}' could not be found.");
                return null;
            }

            var deletedUser = _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return deletedUser.Entity;
        }

        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.CountAsync(cancellationToken);
        }
    }
}
