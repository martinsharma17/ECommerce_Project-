using EcomProj.DataAccess.Data;
using EcomProj.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomProj.DataAccess.Repository
{

    public class UnitOfWork : IUnitOfWork
    {

        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCart = new ShoppingCartRepository(_db);
            UserModel = new UserModelRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            Product = new ProductRepository(_db);



        }

        public IShoppingCartRepository ShoppingCart { get;private set; }
        public IUserModelRepository UserModel { get; private set; }

        public IOrderHeaderRepository OrderHeader { get; private set; }

        public IOrderDetailRepository OrderDetail { get; private set; }

        public IProductRepository Product { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }

    }
}
