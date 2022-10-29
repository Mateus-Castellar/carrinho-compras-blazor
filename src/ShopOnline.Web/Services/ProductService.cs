using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;
using System.Net.Http.Json;

namespace ShopOnline.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProductDto>> GetItems()
        {
            try
            {
                var products = await _httpClient
                    .GetFromJsonAsync<IEnumerable<ProductDto>>("api/products");

                return products;
            }
            catch (Exception)
            {
                //log exceptions
                throw;
            }
        }
    }
}
