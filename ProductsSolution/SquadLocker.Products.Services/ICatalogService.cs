using SquadLocker.Products.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SquadLocker.Products.Services
{
    public interface ICatalogService
    {
        Task<ProductModel> GetBySkuAsync(string sku);
        ICollection<ProductModel> SearchBySku(string sku);
    }
}
