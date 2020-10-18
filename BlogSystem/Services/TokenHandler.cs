using System;
using System.Text;
using System.Linq;
using BlogSystem.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

/*
Copyright (c) 2020 Annice Strömberg – Annice.se

This script is MIT (Massachusetts Institute of Technology) licensed, which means that
permission is granted, free of charge, to any person obtaining a copy of this software
and associated documentation files to deal in the software without restriction. This
includes, without limitation, the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the software, and to permit persons to whom the software
is furnished to do so subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the software.
*/
namespace BlogSystem.Services
{
    /// <summary>
    /// This service class creates and validates a JSON Web Token used in password recovery scenarios.
    /// </summary>
    public class TokenHandler : ITokenHandler
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Inject dependencies.
        /// </summary>
        /// <param name="config"></param>
        public TokenHandler(IConfiguration config) => _config = config;

        /// <summary>
        /// Method to generate a JSON Web Token to be used for a password recovery link sent to the admin email.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public string CreateJWT(BsUser userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id.ToString()) };

            var token = new JwtSecurityToken
                            (
                                issuer: _config["Jwt:Issuer"],
                                audience: _config["Jwt:Audience"],
                                claims: claims,
                                expires: DateTime.Now.AddMinutes(30),
                                signingCredentials: credentials
                            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Validate a JSON web token based on a specified key, issuer and audience fetched from appsettings.json.
        /// </summary>
        /// <param name="token">JSON web token.</param>
        /// <returns>True or false as in valid or invalid token.</returns>
        public bool ValidateJWT(string token)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var myIssuer = _config["Jwt:Issuer"];
            var myAudience = _config["Jwt:Audience"];

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Decode the token to get the user ID.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string DecodeJWT(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedJWT = handler.ReadJwtToken(token);

            var userID = decodedJWT.Claims.First(claim => claim.Type == "sub").Value;

            return userID;
        }

    } // End class.
} // End namespace.
