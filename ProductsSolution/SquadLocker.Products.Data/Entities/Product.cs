//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SquadLocker.Products.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.ProductDecorationLocations = new HashSet<ProductDecorationLocation>();
        }
    
        public int Id { get; set; }
        public System.DateTime DateCreatedUtc { get; set; }
        public System.DateTime DateEditedUtc { get; set; }
        public string SKU { get; set; }
        public string Size { get; set; }
        public Enums.DecorationMethodEnum DecorationMethod { get; set; }
        public string FloodColor { get; set; }
        public string BrandName { get; set; }
        public string ColorGroup { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Gender { get; set; }
        public string ImageUrlBack { get; set; }
        public string ImageUrlFront { get; set; }
        public string ImageUrlName { get; set; }
        public string ImageUrlSide { get; set; }
        public Enums.PersonalizationVariantEnum PersonalizationVariant { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductDecorationLocation> ProductDecorationLocations { get; set; }
    }
}