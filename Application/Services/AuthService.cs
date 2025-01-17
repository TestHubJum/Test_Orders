using Application.Abstractions;
using Application.Models.Authentication;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Domain.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService( IOptions<AuthOptions>  authOptions, 
        UserManager<UserEntity> userManager) : IAuthService
    {
        private readonly AuthOptions _authOptions = authOptions.Value; 

        public async Task<UserResponce> Register(UserRegisterDto userRegisterDto)
        {
            if (await userManager.FindByEmailAsync(userRegisterDto.Email) != null)
            {
                throw new DuplicateEntityException($"Email {userRegisterDto.Email} already exists");
            }

            var createUserResult = await userManager.CreateAsync(new UserEntity
            {
                
                Email = userRegisterDto.Email,
                PhoneNumber = userRegisterDto.Phone,
                UserName = userRegisterDto.Username
            }, userRegisterDto.Password);



            if (createUserResult.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(userRegisterDto.Email);
                if (user == null)
                {
                    throw new EntityNotFoundExceptions($"User with email {userRegisterDto.Email} not register");
                }

                var result = await userManager.AddToRoleAsync(user, RoleConsts.User);

                if (result.Succeeded)
                {
                    var response = new UserResponce
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Username = user.UserName,
                        Roles = [RoleConsts.User],
                        Phone = user.PhoneNumber
                    };
                    return GenerateToken(response);
                }

                throw new Exception($"Errors: {string.Join(";",
                            result.Errors.Select(x => $"{x.Code} {x.Description}"))}");

            }
               
            throw new Exception();
            
        }

        public async Task<UserResponce> Login(UserLoginDto userLoginDto)
        {
            var user = await userManager.FindByEmailAsync(userLoginDto.Email);

            if (user == null)
            {
                throw new EntityNotFoundExceptions($"User with email {userLoginDto.Email} not found");
            }

            var checkPasswordResult = await userManager.CheckPasswordAsync(user, userLoginDto.Password);

            if (checkPasswordResult)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var response = new UserResponce
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Roles = userRoles.ToArray(),
                    Phone = user.PhoneNumber
                };
                return GenerateToken(response);
            }

            throw new AuthenticationException();

        }

        public UserResponce GenerateToken(UserResponce userResponce)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authOptions.TokenPrivateKey);
            var credentials = new SigningCredentials
                (new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = GenerateClaim(userResponce),
                Expires = DateTime.UtcNow.AddMinutes(_authOptions.ExpiresIntervalMinutes),
                SigningCredentials = credentials
            };

            var token = handler.CreateToken(tokenDescription);
            userResponce.Token = handler.WriteToken(token);

            return userResponce;
        }

        private static ClaimsIdentity GenerateClaim(UserResponce userResponce)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Name, userResponce.Email!));
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, userResponce.Id.ToString()));

            foreach (var role in userResponce.Roles!)
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return claims;  
        }


    }
}
