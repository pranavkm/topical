using System;
using Topical.Models;
using Topical.Repository;

namespace Topical.Services
{
    public class UserService : IUserService
    {
        private readonly LuceneProvider _dbProvider;

        public UserService(LuceneProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }


        public User Create(string id, string password)
        {
            var user = _dbProvider.GetRecord<User>(id);
            if (user != null)
            {
                throw new InvalidOperationException("Id {0} is taken.");
            }

            user = new User
            {
                Id = id,
                PasswordHash = CryptoProvider.HashPassword(password),
                CreatedOn = DateTimeOffset.UtcNow
            };

            _dbProvider.AddRecord(user);

            return user;
        }

        public User Get(string id, string password)
        {
            var user = _dbProvider.GetRecord<User>(id);
            if (user != null && CryptoProvider.VerifyHashedPassword(user.PasswordHash, password))
            {
                return user;
            }
            return null;
        }
    }
}