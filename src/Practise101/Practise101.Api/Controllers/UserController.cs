using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practise101.Api.Data;
using Practise101.Api.Models;
using System.Security.Cryptography;

namespace Practise101.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public UserController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (_dataContext.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest("User already exists.");
            }
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken()
            };

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();

            return Ok("User succesfully created.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (user.VerifiedAt == null)
            {
                return BadRequest("Not verified.");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password is incorrect.");
            }

            return Ok($"Welcome back, {user.Email}! :)");

        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.VerificationToken == token);

            if (user == null)
            {
                return BadRequest("Invalid token.");
            }

            user.VerifiedAt = DateTime.Now;
            await _dataContext.SaveChangesAsync();

            return Ok("User verified!");

        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);
            await _dataContext.SaveChangesAsync();

            return Ok("You may now reset your password.");

        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.PasswordResetToken == resetPasswordRequest.Token);

            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return BadRequest("Invalid token.");
            }

            CreatePasswordHash(resetPasswordRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            await _dataContext.SaveChangesAsync();

            return Ok("Password succesfully reset.");

        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}