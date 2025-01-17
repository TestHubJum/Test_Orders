using Application.Models.Authentication;

namespace Application.Abstractions
{
    public interface IAuthService
    {
        Task<UserResponce> Register(UserRegisterDto userRegisterModel);
        Task<UserResponce> Login(UserLoginDto userLoginModel);
    }
}
