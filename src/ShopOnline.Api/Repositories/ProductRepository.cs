using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;

namespace ShopOnline.Api.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetItems();
        Task<IEnumerable<ProductCategory>> GetCategories();
        Task<Product?> GetItem(int id);
        Task<ProductCategory?> GetCategory(int id);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly ShopOnlineDbContext _context;

        public ProductRepository(ShopOnlineDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductCategory>> GetCategories()
        {
            return await _context.ProductCategories.AsNoTracking().ToListAsync();
        }

        public async Task<ProductCategory?> GetCategory(int id)
        {
            return await _context.ProductCategories.FirstOrDefaultAsync(lbda => lbda.Id == id);
        }

        public async Task<Product?> GetItem(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetItems()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }
    }
}
