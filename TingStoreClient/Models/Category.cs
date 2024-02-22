using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class Category
    {
        [Key]
        [DisplayName("ID")]
        public int cateId { get; set; }

        [Required(ErrorMessage = ("Please enter category name!"))]
        [DisplayName("Category Name")]
        public String cateName { get; set; }

        [Required(ErrorMessage = ("Please enter description!"))]
        [DisplayName("Description")]
        public String cateDescribe { get; set; }

        [DisplayName("Status")]
        public bool cateStatus { get; set; }

        public ICollection<Product> Products { get; set; }

        public Category(String cateName, String cateDescribe, bool cateStatus = true)
        {
            this.cateName = cateName;
            this.cateDescribe = cateDescribe;
            this.cateStatus = cateStatus;

        }

        public Category(int cateId, String cateName, String cateDescribe, bool cateStatus = true)
        {
            this.cateId = cateId;
            this.cateName = cateName;
            this.cateDescribe = cateDescribe;
            this.cateStatus = cateStatus;

        }
        public Category()
        {
        }

    }
}