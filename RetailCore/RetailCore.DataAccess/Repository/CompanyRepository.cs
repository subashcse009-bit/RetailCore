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
    public class CompanyRepository : Repository<Company>, ICompany
    {
        private readonly ApplicationDBContext _dbContext;
        public CompanyRepository(ApplicationDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Update(Company company)
        {
           _dbContext.Companies.Update(company);
        }
    }
}
