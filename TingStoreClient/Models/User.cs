using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class User
    {
        [Key]
        [Required(ErrorMessage = "Please enter your username!")]
        [DisplayName("Username")]
        public String userName { get; set; }

        [Required(ErrorMessage = "Please enter your email!")]
        [DisplayName("Email")]
        public String email { get; set; }

        [Required(ErrorMessage =("Please enter your password!"))]
        [DisplayName("Password")]
        public String password { get; set; }

        [Required(ErrorMessage =("Please enter your Fullname!"))]
        [DisplayName("Fullname")]
        public String fullName { get; set; }

        [Required(ErrorMessage =("Please enter your Phonenumber!"))]
        [DisplayName("Phone number")]
        public String phoneNumber { get; set; }

        [DisplayName("Address")]
        public String address { get; set; }

        [DisplayName("Picture")]
        public String picture { get; set; }

        [DisplayName("Date Add")]
        public DateTime createdAt { get; set; }

        [DisplayName("Date Update")]
        public DateTime updateAt { get; set; }

        [DisplayName("User Type")]
        public int userType { get; set; }

        public ICollection<Order> orders { get; set; }
        public ICollection<Feedback> feedbacks { get; set; }
        public ICollection<Cart> carts { get; set; }
    }
}