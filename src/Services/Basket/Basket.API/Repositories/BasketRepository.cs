using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories
{
    public class BasketRepository: IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task DeleteBasket(string userName)
        {
            await _redisCache.RemoveAsync(userName); 
        }

        public async Task<ShoppingCart> GetBasket(string userName)
        {
            var basket= await _redisCache.GetStringAsync(userName);
            if(String.IsNullOrEmpty(basket)) //vratio json kao string
                return null;

            //koristimo json konvertor da deserijalizujemo string u shoppingcart object
            return JsonConvert.DeserializeObject<ShoppingCart>(basket); //pretvara iz json u shoppingcart
        }
         
        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket)); //pretvara u json, u bazi je zapamceno kao json

            return await GetBasket(basket.UserName);
        }
    }
}
