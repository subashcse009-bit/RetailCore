using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailCore.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        public ICategory Category { get; }
        public IProduct Product { get; }
        public ICompany Company { get; }
        public IShoppingCart ShoppingCart { get; }
        public IApplicationUser ApplicationUser { get; }
        public IOrderHeader OrderHeader { get; }
        public IOrderDetail OrderDetail { get; }
        void Save();
    }
}
