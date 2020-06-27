using AutoMapper;
using Ecommerce.Api.Customers.Db;
using Ecommerce.Api.Customers.Interfaces;
using Ecommerce.Api.Customers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Api.Customers.Providers
{
    public class CustomersProvider : ICustomersProvider
    {
        private readonly CustomersDbContext dbContext;
        private readonly ILogger<CustomersProvider> logger;
        private readonly IMapper mapper;

        public CustomersProvider(CustomersDbContext dbContext, ILogger<CustomersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
            SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Customers.Any())
            {
                dbContext.Customers.Add(new Db.Customer() { Id = 1, Name = "Obuli Mani", Address = "Vibha Orchids, Panathur, Bangalore" });
                dbContext.Customers.Add(new Db.Customer() { Id = 2, Name = "Janarthanan", Address = "Some Apartments, AECS Layout, Bangalore" });
                dbContext.Customers.Add(new Db.Customer() { Id = 3, Name = "Pushpa", Address = "Some Individual House, Moodalapalya, Bangalore" });
                dbContext.Customers.Add(new Db.Customer() { Id = 4, Name = "Mageswari", Address = "Own House, Pathinam thitta, Trivandrum" });
                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Customer> Customers, string ErrorMessage)> GetCustomersAsync()
        {
            try
            {
                var customers = await dbContext.Customers.ToListAsync();
                if (customers != null && customers.Any())
                {
                    var results = mapper.Map<IEnumerable<Db.Customer>, IEnumerable<Models.Customer>> (customers);
                    return (true, results, null);
                }
                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {

                logger?.LogError(ex.Message);
                return  (false, null, ex.Message);
            }
        }


        public async Task<(bool IsSuccess, Models.Customer Customer, string ErrorMessage)> GetCustomerAsync(int id)
        {
            try
            {
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

                if (customer != null)
                {
                    var result = mapper.Map<Db.Customer, Models.Customer>(customer);
                    return (true, result, null);
                }
                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {

                logger?.LogError(ex.Message);
                return (false, null, ex.Message);
            }

        }
    }
}
