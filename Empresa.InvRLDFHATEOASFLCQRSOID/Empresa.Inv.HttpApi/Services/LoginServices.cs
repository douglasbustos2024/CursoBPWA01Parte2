using Empresa.Inv.Dtos;
using Jose;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
 

namespace Empresa.Inv.HttpApi.Services
{
    public class LoginServices
    {             
        public UserDTO AuthenticateUser(LoginModel login)
        {
            // Aquí deberías autenticar al usuario con tu lógica de negocio
            // Este es solo un ejemplo de usuario
            var user = new UserDTO
            {
                Id = 2,
                UserName = "exampleUser",
                Roles = "operador"  ,
                Email="douglas.bustos101@gmail.com",
                TwoFactorCode="",
                TwoFactorExpiry=null 
            };

                                        
            // Aquí puedes devolver el usuario con sus claims, por ejemplo, en un token JWT
            return user; // Asegúrate de incluir los claims en el token si es necesario


        }
                  
    }



}
