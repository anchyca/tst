using SquadLocker.Products.Data;
using SquadLocker.Products.Data.Entities;
using SquadLocker.Products.Data.Enums;
using SquadLocker.Products.Services.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using SquadLocker.Products.Data.Extensions;

namespace SquadLocker.Products.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CatalogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductModel> GetBySkuAsync(string sku)
        {
            var product = await _unitOfWork.Repository<Product>().Table
                            .Where(x => x.SKU == sku)
                            .Select(x => new ProductModel
                            {
                                Id = x.Id,
                                SKU = x.SKU,
                                BrandName = x.BrandName,
                                Size = x.Size,
                                ImageUrlBack = x.ImageUrlBack,
                                ImageUrlFront = x.ImageUrlFront,
                                ImageUrlName = x.ImageUrlName,
                                ImageUrlSide = x.ImageUrlSide,
                                DecorationMethod = x.DecorationMethod,
                                FloodColor = x.FloodColor,
                                ColorGroup = x.ColorGroup,
                                Gender = x.Gender,
                                Price = x.Price,
                                PersonalizationVariant = x.PersonalizationVariant,
                                DecorationLocations = x.ProductDecorationLocations.Select(y => new ProductDecorationLocationModel
                                {
                                    ProductId = y.ProductId,
                                    DecorationLocation = y.DecorationLocation
                                }).ToList()
                            })
                            .OrderBy(x => x.SKU.IndexOf(sku))
                            .FirstOrDefaultAsync();

            return product;
        }

        public ICollection<ProductModel> SearchBySku(string sku)
        {
            var products = _unitOfWork.Repository<Product>().Table
                            .Where(x => x.SKU.Contains(sku))
                            .Select(x => new ProductModel
                            {
                                Id = x.Id,
                                SKU = x.SKU,
                                BrandName = x.BrandName,
                                Size = x.Size,
                                ImageUrlBack = x.ImageUrlBack,
                                ImageUrlFront = x.ImageUrlFront,
                                ImageUrlName = x.ImageUrlName,
                                ImageUrlSide = x.ImageUrlSide,
                                DecorationMethod = x.DecorationMethod,
                                FloodColor = x.FloodColor,
                                ColorGroup = x.ColorGroup,
                                Gender = x.Gender,
                                Price = x.Price,
                                PersonalizationVariant = x.PersonalizationVariant,
                                DecorationLocations = x.ProductDecorationLocations.Select(y => new ProductDecorationLocationModel
                                {
                                    ProductId = y.ProductId,
                                    DecorationLocation = y.DecorationLocation
                                }).ToList()
                            })
                            .OrderBy(x => x.SKU.IndexOf(sku))
                            .ToList();

            return products;
        }
    }
}
