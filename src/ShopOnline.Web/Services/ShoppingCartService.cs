﻿using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;
using System.Net;
using System.Net.Http.Json;

namespace ShopOnline.Web.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly HttpClient _httpClient;

        public ShoppingCartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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

        public async Task<IEnumerable<CartItemDto>> GetItems(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/shoppingCarts/{userId}/GetItems");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        return Enumerable.Empty<CartItemDto>();

                    return await response.Content.ReadFromJsonAsync<IEnumerable<CartItemDto>>();
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
    }
}
