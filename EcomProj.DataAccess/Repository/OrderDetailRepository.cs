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
    public class OrderDetailRepository : Repository<OrderDetails>, IOrderDetailRepository
    {

        private ApplicationDbContext _db;

        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(OrderDetails obj)
        {
            _db.OrderDetails.Update(obj);
        }
    }
}
