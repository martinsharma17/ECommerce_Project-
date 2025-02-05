using EcomProj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomProj.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);

        void UpdateStatus(int id, string orderStatus, string? paymentstatus = null);
        void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);

        //save must be done in Repository ma but this is not the good practice 
    }
}
