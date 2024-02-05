using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class Product
    {
        [Key]
        [DisplayName("ID")]
        public int proId { get; set; }

        [Required(ErrorMessage = ("Please enter product name!"))]
        [DisplayName("Product Name")]
        public String proName { get; set; }

        [Required(ErrorMessage = ("Please enter description!"))]
        [DisplayName("Description")]
        public String proDescription { get; set; }


        [Required(ErrorMessage = ("Please enter price!"))]
        [DisplayName("Price")]
        [Column(TypeName = "Money")]          // ánh xạ thuộc tính Price, kiểu decimal vào cột Price,  kiểu Money của bảng
        public long Price { set; get; }

        [Required(ErrorMessage = ("Please enter quantity!"))]
        [DisplayName("Quantity")]
        public int proQuantity { get; set; }

        [Required(ErrorMessage = ("Please enter product image!"))]
        [DisplayName("Image")]
        public String proImage { get; set; }
        [DisplayName("Status")]
        public bool proStatus { get; set; }

        [Required(ErrorMessage = ("Please enter cate Id!"))]
        [DisplayName("Category ID")]
        public int cateId { get; set; }
        [ForeignKey("cateId")]
        public Category category { get; set; }

        public ICollection<ProductImage> productImages { get; set; }
        public ICollection<DiscountPercent> discountPercents { get; set; }
        public ICollection<Feedback> feedbacks { get; set; }
        public ICollection<OrderDetail> orderDetails { get; set; }
        public ICollection<Cart> carts { get; set; }
    }
}