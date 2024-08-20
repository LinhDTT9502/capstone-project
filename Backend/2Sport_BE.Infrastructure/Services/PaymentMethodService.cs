using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IPaymentMethodService
    {
        Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync();
        Task<PaymentMethod> GetPaymentMethodAsync(int id);
        Task<PaymentMethod> AddPaymentMethodAsync(PaymentMethod paymentMethod);
        Task<bool> UpdatePaymentMethodAsync(PaymentMethod paymentMethod);
        Task<bool> DeletePaymentMethodAsync(int id);
    }
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly TwoSportDBContext _context;

        public PaymentMethodService(TwoSportDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return await _context.PaymentMethods.ToListAsync();
        }

        public async Task<PaymentMethod> GetPaymentMethodAsync(int id)
        {
            return await _context.PaymentMethods.FindAsync(id);
        }

        public async Task<PaymentMethod> AddPaymentMethodAsync(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Add(paymentMethod);
            await _context.SaveChangesAsync();
            return paymentMethod;
        }

        public async Task<bool> UpdatePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            _context.Entry(paymentMethod).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PaymentMethodExistsAsync(paymentMethod.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeletePaymentMethodAsync(int id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null)
            {
                return false;
            }

            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> PaymentMethodExistsAsync(int id)
        {
            return await _context.PaymentMethods.AnyAsync(e => e.Id == id);
        }
    }

}
