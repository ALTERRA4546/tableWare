//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tableware
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.OrderProduct = new HashSet<OrderProduct>();
        }
    
        public string ProductArticleNumber { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<int> ProductCategory { get; set; }
        public byte[] ProductPhoto { get; set; }
        public Nullable<int> ProductManufacturer { get; set; }
        public Nullable<decimal> ProductCost { get; set; }
        public Nullable<byte> ProductDiscountAmount { get; set; }
        public Nullable<byte> ProductMaxDiscountAmount { get; set; }
        public Nullable<int> ProductQuantityInStock { get; set; }
        public Nullable<int> ProductSupplayer { get; set; }
        public Nullable<int> MeasurementUnit { get; set; }
    
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual MeasurementUnit MeasurementUnit1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
        public virtual ProductCategory ProductCategory1 { get; set; }
        public virtual ProductSupplayer ProductSupplayer1 { get; set; }
    }
}
