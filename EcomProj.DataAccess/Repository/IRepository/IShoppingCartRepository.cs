using EcomProj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomProj.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        int IncrementCount(ShoppingCart shoppingcart, int count);
        int DecrementCount(ShoppingCart shoppingcart, int count);
    }
}
