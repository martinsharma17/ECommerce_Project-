using EcomProj.DataAccess.Data;
using EcomProj.DataAccess.Repository.IRepository;
using EcomProj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomProj.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Product obj)
        {

            //_db can be used directy here but bahira paudaina ...
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.FixedPrice = obj.FixedPrice;
                objFromDb.DiscountPrice = obj.DiscountPrice;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Quantity = obj.Quantity;
                objFromDb.SubCategoryId = obj.SubCategoryId;
                if (obj.Image != null)
                {
                    objFromDb.Image = obj.Image;
                }

            }
        }
    }
}
