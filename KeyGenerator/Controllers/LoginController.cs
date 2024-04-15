using KeyGenerator.Models.NonDBModels;
using KeyGenerator.Data;
using KeyGenerator.Encryptions;
using KeyGenerator.Models;
using KeyGenerator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KeyGen.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly KeyGeneratorDBContext _context;
        private readonly ILoggerService _logger;

        public LoginController(IConfiguration configuration, KeyGeneratorDBContext context, ILoggerService logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }


        private string GenerateToken(UserAuth user)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserID.ToString()), // Assuming UserID is the unique identifier
                new Claim("AutoGenPass", user.AutoGenPass.ToString()), // Convert bool to string
            };

            var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials
        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize]
        [HttpPost("Extend")]
        public IActionResult Extend()
        {
            IActionResult response = Unauthorized();

            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var userauth = _context.UserAuthentication.FirstOrDefault(i => i.UserID == userId);
                if (userauth != null)
                {
                    var token = GenerateToken(userauth);

                    _logger.LogEvent($"User-Login Extended", "Login", userauth.UserID);
                    response = Ok(new { token = token, userauth.UserID, userauth.AutoGenPass });

                }
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] MLoginRequest model)
        {
            IActionResult response = Unauthorized();
            /*var user = _context.Users.FirstOrDefault(u => u.EmailAddress == model.Email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            int UserId = user.User_ID;

            var userAuth = _context.UserAuthentication.FirstOrDefault(ua => ua.User_ID == UserId);*/

            var user1 = _context.Users.FirstOrDefault(u => u.EmailAddress == model.Email);
            if (user1 == null)
            {
                return NotFound();
            }
            else
            {
                if (user1.Status == false)
                {
                    return Unauthorized("User is inactive");
                }
            }
            var userAuth = (from user in _context.Users
                            join ua in _context.UserAuthentication on user.UserID equals ua.UserID
                            where user.EmailAddress == model.Email
                            select ua).FirstOrDefault();

            if (userAuth == null)
            {
                return NotFound("User not found");
            }
            string hashedPassword = Sha256Hasher.ComputeSHA256Hash(model.Password);
            Console.WriteLine(hashedPassword);
            if (hashedPassword != userAuth.Password)
            {
                return Unauthorized("Invalid password");
            }

            if (userAuth != null)
            {
                var token = GenerateToken(userAuth);

                _logger.LogEvent($"User Logged-in", "Login", userAuth.UserID);
                response = Ok(new { token = token, userAuth.UserID, userAuth.AutoGenPass });

            }
            return response;
        }

        [HttpPut("Forgotpassword")]
        public IActionResult ResetPassword([FromBody] LoginRequest user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = _context.Users.FirstOrDefault(u => u.EmailAddress == user.Email);

            if (users == null)
            {
                return NotFound("User not found");
            }

            var userauth = _context.UserAuthentication.FirstOrDefault(i => i.UserID == users.UserID);

            if (userauth == null)
            {
                return NotFound("User Authentication Data Not Found");
            }

            string newPassword = PasswordGenerate.GeneratePassword();

            string hashedPassword = Sha256Hasher.ComputeSHA256Hash(newPassword);

            userauth.Password = hashedPassword;

            userauth.AutoGenPass = true;

            _context.SaveChanges();

            string emailBody = $@"
                <div style=""text-align: center; background-color: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); border: 2px solid black; min-width: 200px; max-width: 300px; width: 100%; margin: 50px auto;"">
                    <h2 style=""color: blue;"">New Login Credentials <hr /></h2>
                    <p>
                        <strong>Username:</strong><br /> {users.EmailAddress}
                    </p>
                    <p>
                        <strong>Password:</strong><br /> {newPassword}
                    </p>
                    <p style=""color: #F00;"">
                        Please change the password immediately after login.
                    </p>
                    <a href=""#login-link"" style=""display: inline-block; padding: 10px 20px; background-color: #007BFF; color: #fff; text-decoration: none; border-radius: 5px; margin-top: 15px;"">Login Here</a>
                </div>";
            string result =new EmailService(_context,_logger).SendEmail(users.EmailAddress, "Reset-Password", emailBody);
            /*var emailservice = new EmailService(_configuration);
            var result = emailservice.SendEmail(users.EmailAddress, "Reset-Password", emailBody);*/
            _logger.LogEvent("Password-Reset", "Login", users.UserID);
            return Ok(new { newPassword, result });


        }
        [Authorize]
        [HttpPut("Changepassword/{id}")]
        public IActionResult ChangePassword(int id, MChangePassword cred)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            string oldHashPass = Sha256Hasher.ComputeSHA256Hash(cred.OldPassword);
            var userauth = _context.UserAuthentication.FirstOrDefault(i => i.UserID == id);

            if (userauth == null)
            {
                return NotFound("User Authentication Data Not Found");
            }
            if (userauth.Password != oldHashPass)
            {
                return BadRequest("Existing Password Invalid");
            }

            string newPassword = cred.NewPassword;

            string hashedPassword = Sha256Hasher.ComputeSHA256Hash(newPassword);

            userauth.Password = hashedPassword;

            userauth.AutoGenPass = false;

            _context.SaveChanges();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                // Now you have the user ID
                _logger.LogEvent("Password-Changed", "Login", userId);
            }

            return Ok(new { newPassword, hashedPassword });


        }
    }
}
