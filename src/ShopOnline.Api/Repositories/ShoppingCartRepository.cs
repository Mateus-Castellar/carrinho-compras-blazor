using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Repositories
{
    public interface IShoppingCartRepository
    {
        Task<CartItem?> AddItem(CartItemtoAddDto cartItemtoAddDto);
        Task<CartItem?> UpdateQty(int id, CartItemQtyUpdateDto cartItemQtyUpdateDto);
        Task<CartItem?> DeleteItem(int id);
        Task<CartItem?> GetItem(int id);
        Task<IEnumerable<CartItem>> GetItems(int userId);
    }

    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ShopOnlineDbContext _context;

        public ShoppingCartRepository(ShopOnlineDbContext context)
        {
            _context = context;
        }

        private async Task<bool> CartItemExists(int cartId, int productId)
        {
            return await _context.CartItems
                .AnyAsync(c => c.CartId == cartId && c.ProductId == productId);
        }

        public async Task<CartItem?> AddItem(CartItemtoAddDto cartItemtoAddDto)
        {
            if (await CartItemExists(cartItemtoAddDto.CartId, cartItemtoAddDto.ProductId) == false)
            {
                var item = await (from product in _context.Products
                                  where product.Id == cartItemtoAddDto.ProductId
                                  select new CartItem
                                  {
                                      CartId = cartItemtoAddDto.CartId,
                                      ProductId = product.Id,
                                      Qty = cartItemtoAddDto.Qty
                                  }).SingleOrDefaultAsync();

                if (item is not null)
                {
                    var result = await _context.CartItems.AddAsync(item);
                    await _context.SaveChangesAsync();
                    return result.Entity;
                }
            }
            return null;
        }

        public async Task<CartItem?> DeleteItem(int id)
        {
            var item = await _context.CartItems.FindAsync(id);

            if (item is not null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                return item;
            }

            return null;
        }

        public async Task<CartItem?> GetItem(int id)
        {
            return await (from cart in _context.Carts
                          join cartItem in _context.CartItems
                          on cart.Id equals cartItem.CartId
                          where cartItem.Id == id
                          select new CartItem
                          {
                              Id = cartItem.Id,
                              ProductId = cartItem.ProductId,
                              Qty = cartItem.Qty,
                              CartId = cartItem.CartId
                          }).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<CartItem>> GetItems(int userId)
        {
            return await (from cart in _context.Carts
                          join cartItem in _context.CartItems
                          on cart.Id equals cartItem.CartId
                          where cart.UserId == userId
                          select new CartItem
                          {
                              Id = cartItem.Id,
                              ProductId = cartItem.ProductId,
                              Qty = cartItem.Qty,
                              CartId = cartItem.CartId
                          }).ToListAsync();
        }

        public async Task<CartItem?> UpdateQty(int id, CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            var item = await _context.CartItems.FindAsync(id);

            if (item is not null)
            {
                item.Qty = cartItemQtyUpdateDto.Qty;
                await _context.SaveChangesAsync();
                return item;
            }

            return null;
        }
    }
}
