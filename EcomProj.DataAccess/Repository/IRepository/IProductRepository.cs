using EcomProj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomProj.DataAccess.Repository.IRepository
{ 
    public interface IProductRepository : IRepository<Product>
{
    void Update(Product obj);

    //save must be done in Repository ma but this is not the good practice 
}
}
