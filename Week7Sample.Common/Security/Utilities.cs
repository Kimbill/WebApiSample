﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Week7Sample.Model;

namespace Week7Sample.Common.Security
{
    public class Utilities
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        public Utilities(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task<string> GenerateJwt(User user)
        {
            var listOfClaims = new List<Claim>();

            listOfClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            listOfClaims.Add(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"));

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                listOfClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Key").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(listOfClaims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            var token = tokenHandler.WriteToken(createdToken);

            return token;
        }
    }
}
