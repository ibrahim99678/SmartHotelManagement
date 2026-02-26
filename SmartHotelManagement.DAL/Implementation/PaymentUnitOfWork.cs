using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Implementation
{
    public class PaymentUnitOfWork: UnitOfWork, IPaymentUnitOfWork
    {
        public IPaymentRepository PaymentRepository { get; }

        public PaymentUnitOfWork(SmartHotelDbContext context, IPaymentRepository paymentRepository) : base(context)
        {
            PaymentRepository = paymentRepository;
        }
    }
}
