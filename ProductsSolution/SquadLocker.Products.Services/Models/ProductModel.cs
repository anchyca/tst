using SquadLocker.Products.Data.Enums;
using System;
using System.Collections.Generic;

namespace SquadLocker.Products.Services.Models
{
    public class ProductModel
    {
        public ProductModel()
        {
            this.DecorationLocations = new List<ProductDecorationLocationModel>();
        }

        public int Id { get; set; }
        public string SKU { get; set; }
        public string BrandName { get; set; }
        public string ImageUrlBack { get; set; }
        public string ImageUrlFront { get; set; }
        public string ImageUrlName { get; set; }
        public string ImageUrlSide { get; set; }
        public string Size { get; set; }
        public DecorationMethodEnum DecorationMethod { get; set; }
        public string FloodColor { get; set; }

        public string ColorGroup { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Gender { get; set; }

        public PersonalizationVariantEnum PersonalizationVariant { get; set; }

        public List<ProductDecorationLocationModel> DecorationLocations { get; set; }
    }
}
