using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TingStoreAPI.DB;
using TingStoreAPI.Helper;
using TingStoreAPI.Service;

namespace TingStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        private readonly IEmailService emailService;

        public ForgotPasswordController(IEmailService emailService, ApplicationDBContext db)
        {
            this.emailService = emailService;
            this._db = db;
        }

        [HttpGet("SendMail/{toIEmail}")]
        public async Task<IActionResult> SendMail(string toIEmail)
        {
            string rdn = randomNumber();
            try
            {
                Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = toIEmail;
                mailrequest.Subject = "Ting Store - Reset Password";
                mailrequest.Body = GetHtmlcontent(toIEmail, rdn);
                await emailService.SendEmailAsync(mailrequest);
                return Ok(rdn);
            }
            catch (Exception ex)
            {
                throw new Exception("Can't send mail");
            }
        }
        private string randomNumber()
        {
            string rs = "";
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                rs += random.Next(0, 10) + " ";
            }
            return rs.Trim();
        }
        private string GetHtmlcontent(string username, string id)
        {
            string Response = "<div style=\"width:100%;background-color:lightblue;text-align:center;margin:10px\">";
            Response += "<h1>Dear " + username + " </h1>";
            Response += "<h2>You have requested to reset the password for your Ting Store account. Here is your verification code:</h2>";
            Response += "<h1>" + id + "</h1>";
            Response += "<p>Please note that this verification code is only valid for a short period of time. Please do not share this code with anyone else.</p>";
            Response += "<p>If you did not request a password reset, please disregard this email.</p>";
            Response += "<div><h1> Contact us : tingstoresp23@gmail.com</h1></div>";
            Response += "</div>";
            return Response;
        }
        [HttpGet("ResetPass/{Iusername}/{Ipass}")]
        public IActionResult ResetPass(string Iusername, string Ipass)
        {
            if (Iusername == null)
            {
                return BadRequest("Username can't be nul;");
            }
            if (Ipass == null)
            {
                return BadRequest("Pass can't be null");
            }
            var user = this._db.users.FirstOrDefault(u => u.userName.Equals(Iusername));
            user.password = Ipass;
            this._db.SaveChanges();
            return Ok(User);
        }
    }
}