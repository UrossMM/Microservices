using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connecion = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            // Coupon je tip koji zelimo da fja QueryFirstOrDefaultAsync vrati
            var coupon = await connecion.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

            if(coupon == null)
            {
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };
            }
            
            return coupon;  
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connecion = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected= await connecion.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)", 
                new { ProductName =coupon.ProductName, Description= coupon.Description, Amount=coupon.Amount });
            if (affected == 0)
                return false;
            return true;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connecion = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        
            var affected= await connecion.ExecuteAsync("UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amount=@Amount WHERE Id=@Id",
                new {ProductName=coupon.ProductName,Description=coupon.Description, Amount=coupon.Amount, Id=coupon.Id });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connecion = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

            var affected = await connecion.ExecuteAsync("DELETE FROM Coupon WHERE ProductName=@ProductName", new { ProductName = productName });

            if (affected == 0)
                return false;

            return true;
        }
    }
}
