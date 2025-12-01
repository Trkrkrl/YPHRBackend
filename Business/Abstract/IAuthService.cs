using Core.Utilities.Results;
using Core.Utilities.Security.JWT;
using Core.Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAuthService
    {
        Task<IDataResult<User>> RegisterAsync(UserForRegisterDto userForRegisterDto);
        Task<IDataResult<User>> LoginWithEmailAsync(UserMailLoginDto userMailLoginDto);

        Task<IDataResult<AccessToken>> CreateAccessTokenAsync(User user);
    }
}
