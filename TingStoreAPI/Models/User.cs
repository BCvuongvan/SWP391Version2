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

        public User()
        {

        }
        public User(String userName, String email, String password, String fullName, String phoneNumber, DateTime updateAt)
        {
            this.userName = userName;
            this.email = email;
            this.password = password;
            this.fullName = fullName;
            this.phoneNumber = phoneNumber;
            this.picture = "l60Hf-150x150.png";
            this.createdAt = DateTime.Now;
            this.updateAt = DateTime.Now;
        }

        public User(String userName, String email, String password, String fullName, String phoneNumber,String address ,String picture, DateTime createdAt, DateTime updateAt,int userType)
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