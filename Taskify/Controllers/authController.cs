//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Taskify.Core.Entities;
//using Taskify.Core.Servieces;
//using Taskify.Models;

//namespace Taskify.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class authController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;
//        private readonly IUserService _userService;
//        public authController(IConfiguration configuration, IUserService userService)
//        {
//            _configuration = configuration;
//            _userService = userService;
//        }

//        [HttpPost]
//        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
//        {
//            if (loginModel?.Name == null || loginModel?.Email == null)
//                return BadRequest(new { message = "Name and Email are required." });

//            var user = await _userService.GetUsersByNameAndEmailAsync(loginModel.Name.Trim(), loginModel.Email.Trim());
//            if (user != null)
//            {
//                var claims = new List<Claim>()
//    {
//        new Claim(ClaimTypes.Email, user.Email),
//        new Claim(ClaimTypes.Role, user.Level.ToString())
//    };
//                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT:Key")));
//                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
//                var tokeOptions = new JwtSecurityToken(
//                    issuer: _configuration.GetValue<string>("JWT:Issuer"),
//                    audience: _configuration.GetValue<string>("JWT:Audience"),
//                    claims: claims,
//                    expires: DateTime.Now.AddMinutes(6),
//                    signingCredentials: signinCredentials
//                );
//                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
//                return Ok(new { Token = tokenString });
//            }

//            var anyUsers = await _userService.GetUsersAsync();
//            if (!anyUsers.Any())
//                return Unauthorized(new { message = "No users in system. First create admin: POST /api/User/setup-admin" });

//            return Unauthorized(new { message = "Invalid name or email. User not found." });
//        }

//        /// <summary>
//        /// עזר לפיתוח - מחזיר רשימת משתמשים קיימים (שם + אימייל) כדי לדעת עם מה להתחבר
//        /// </summary>
//        [HttpGet("list-users")]
//        public async Task<IActionResult> ListUsers()
//        {
//            var users = await _userService.GetUsersAsync();
//            var list = users.Select(u => new { u.Name, u.Email, u.Role }).ToList();
//            return Ok(new { users = list, message = "השתמשי ב-name ו-email האלה ב-Login" });
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;

using Taskify.Core.Entities;

using Taskify.Core.Servieces;

using Taskify.Models;



namespace Taskify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // כל האקשנים בקונטרולר הזה פתוחים כברירת מחדל (Login, list-users)
    public class authController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        private readonly IUserService _userService;



        public authController(IConfiguration configuration, IUserService userService)

        {

            _configuration = configuration;

            _userService = userService;

        }



        [HttpPost]
        [AllowAnonymous] // לא דורש JWT כדי להתחבר
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            // 1. בדיקה בסיסית של הבקשה
            if (loginModel == null)
                return BadRequest(new { message = "Request body is empty. Send JSON like { \"email\": \"admin@taskify.com\" }" });

            if (string.IsNullOrWhiteSpace(loginModel.Email))
                return BadRequest(new { message = "Email is required." });

            // 2. חיפוש משתמש לפי אימייל בלבד
            var users = await _userService.GetUsersAsync();
            var user = users.FirstOrDefault(u =>
                !string.IsNullOrEmpty(u.Email) &&
                u.Email.Trim().ToLower() == loginModel.Email.Trim().ToLower());

            if (user == null)
                return Unauthorized(new { message = "Invalid email. User not found." });

            // 3. בניית ה-Claims לפי המשתמש שנמצא
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Level.ToString()) // worker / manager / headmanager
    };

            // 4. יצירת ה-JWT
            var key = _configuration.GetValue<string>("JWT:Key");
            if (string.IsNullOrEmpty(key) || key.Length < 16)
                return StatusCode(500, new { message = "JWT Key is missing or too short in configuration." });

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("JWT:Issuer"),
                audience: _configuration.GetValue<string>("JWT:Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            // 5. החזרה בפורמט שה-React מצפה לו
            return Ok(new { token = tokenString });
        }



        [HttpGet("list-users")]
        [AllowAnonymous] // גם רשימת המשתמשים פתוחה כדי שתוכלי לבדוק במה להתחבר
        public async Task<IActionResult> ListUsers()

        {

            var users = await _userService.GetUsersAsync();

            var list = users.Select(u => new { u.Name, u.Email, Role = u.Level.ToString() }).ToList();

            return Ok(new { users = list, message = "השתמשי ב-email וב-role האלה לצורך בדיקת הקונטרולרים" });

        }

    }

}