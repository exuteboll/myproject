using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.DAL;
using WebApplication3.DAL.Storage;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL.Storage
{
    public class UserStorage : BaseStorage<UserDb>
    {
        public UserStorage(ApplicationDbContext db) : base(db) { }

        public async Task<UserDb> GetByEmail(string email)
        {
            return await _db.UserDb.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}