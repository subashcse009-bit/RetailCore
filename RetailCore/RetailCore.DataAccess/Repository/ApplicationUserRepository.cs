using RetailCore.DataAccess.Data;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailCore.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>,IApplicationUser
    {
        private readonly ApplicationDBContext _dbContext;
        public ApplicationUserRepository(ApplicationDBContext dbContext):base (dbContext)
        {
            _dbContext= dbContext;
        }
    }
}
