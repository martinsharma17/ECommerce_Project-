using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomProj.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IShoppingCartRepository ShoppingCart { get; }
        IUserModelRepository UserModel { get; }

        IOrderDetailRepository OrderDetail { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IProductRepository Product { get; }

        void Save();

    }
}
