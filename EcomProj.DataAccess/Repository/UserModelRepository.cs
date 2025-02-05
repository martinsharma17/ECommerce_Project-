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
    public class UserModelRepository : Repository<UserModel>, IUserModelRepository
    {

        private ApplicationDbContext _db;

        public UserModelRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
