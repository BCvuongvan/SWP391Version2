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
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Please enter a valid email address")]
        [DisplayName("Email")]
        public String email { get; set; }

        [Required(ErrorMessage = ("Please enter your password!"))]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = "Please enter a password with at least 8 characters including lowercase letters and special characters!")]
        [DisplayName("Password")]
        public String password { get; set; }

        [Required(ErrorMessage = ("Please enter your Fullname!"))]
        [DisplayName("Fullname")]
        public String fullName { get; set; }

        [Required(ErrorMessage = ("Please enter your Phonenumber!"))]
        [RegularExpression(@"^0[1-9]{2}[-.\s]?[0-9]{3}[-.\s]?[0-9]{4}$", ErrorMessage = "Please enter a 10-digit phone number beginning with the number '0'!")]
        [DisplayName("Phone number")]
        public String phoneNumber { get; set; }

        [Required(ErrorMessage = ("Please enter Address!"))]
        [DisplayName("Address")]
        public String address { get; set; }

        [DisplayName("Picture")]
        public String picture { get; set; }

        [DisplayName("Date Add")]
        public DateTime createdAt { get; set; }

        [DisplayName("Date Update")]
        public DateTime updateAt { get; set; }

        [DisplayName("User Type")]
        public int userType { get; set; } // admin: 1, staff: 2, customer: 3, account ban: 4

        public ICollection<Order> orders { get; set; }
        public ICollection<Feedback> feedbacks { get; set; }
        public ICollection<Cart> carts { get; set; }

        public User()
        {

        }
        public User(String userName, String email, String password, String fullName, String phoneNumber, String address, String picture, DateTime createdAt, DateTime updateAt, int userType)
        {
            this.userName = userName;
            this.email = email;
            this.password = password;
            this.fullName = fullName;
            this.phoneNumber = phoneNumber;
            this.address = address;
            this.picture = picture;
            this.createdAt = createdAt;
            this.updateAt = updateAt;
            this.userType = userType;

        }
    }
}