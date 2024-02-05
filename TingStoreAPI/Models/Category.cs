using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreAPI.Models
{
    public class Category
    {
        [Key]
        public int cateId { get; set; }
        [Required]
        public String cateName { get; set; }
        [Required]
        public String cateDescribe { get; set; }
        public bool cateStatus { get; set; }
    }
}