using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreAPI.Models
{
    public class User
    {
        [Key]
        public String userName { get; set; }
        [Required]
        public String email { get; set; }
        [Required]
        public String password { get; set; }
        [Required]
        public String fullName { get; set; }
        [Required]
        public String phoneNumber { get; set; }
        public String address { get; set; }
        public String picture { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updateAt { get; set; }
        public int userType { get; set; }

        public ICollection<Order> orders { get; set; }
        public ICollection<Feedback> feedbacks { get; set; }
        public ICollection<Cart> carts { get; set; }
    }
}