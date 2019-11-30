using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Back.Models
{
    public class CifradoJWT
    {
        public string Nombre { get; set; }
        public string GenerateToken(string key, Usuario json)
        {


            var llaveSeguridad = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var crencial = new SigningCredentials(llaveSeguridad, SecurityAlgorithms.HmacSha256);
            var reclamo = new[]
            {
                new Claim("usuario",json.User),
                new Claim("Contraseña",json.Password)

            };



            var token = new JwtSecurityToken("xyz",
                "xtz", reclamo,
                DateTime.UtcNow,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: crencial);
            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
