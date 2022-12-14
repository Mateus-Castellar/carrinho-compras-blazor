using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ShopOnline.Web.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly HttpClient _httpClient;

        public ShoppingCartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public event Action<int> OnShoppingCartChanged;

        public async Task<CartItemDto> AddItem(CartItemtoAddDto cartItemtoAddDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<CartItemtoAddDto>
                    ("api/shoppingCarts", cartItemtoAddDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        return default(CartItemDto);

                    return await response.Content.ReadFromJsonAsync<CartItemDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"http status {response.StatusCode} Message {message}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CartItemDto> DeleteItem(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/shoppingCarts/{id}");

                if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<CartItemDto>();

                return default(CartItemDto);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CartItemDto>> GetItems(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/shoppingCarts/{userId}/GetItems");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        return Enumerable.Empty<CartItemDto>().ToList();

                    return await response.Content.ReadFromJsonAsync<List<CartItemDto>>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"http status {response.StatusCode} Message {message}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RaiseEventOnShoppingCartChanged(int totalQty)
        {
            if (OnShoppingCartChanged is not null)
            {
                OnShoppingCartChanged.Invoke(totalQty);
            }
        }

        public async Task<CartItemDto> UpdateQty(CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            try
            {
                var jsonRequest = JsonSerializer.Serialize(cartItemQtyUpdateDto);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

                var response = await _httpClient.PatchAsync($"api/shoppingCarts/{cartItemQtyUpdateDto.CartItemId}", content);

                if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<CartItemDto>();

                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
