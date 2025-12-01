using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Logger;
using Core.Utilities.Business;

using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.JWT;
using DataAccess.Abstract;
using Core.Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;
        private readonly IUserDal _userDal;
        

        public AuthManager(
            IUserDal userDal,
            IUserService userService,
            ITokenHelper tokenHelper
         )
        {
            _userService = userService;
            _userDal = userDal;
            _tokenHelper = tokenHelper;
        
        }

        [ValidationAspect(typeof(UserResigterValidator))]
        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<User>> RegisterAsync(UserForRegisterDto userForRegisterDto)
        {
            var ruleResult = BusinessRules.Run(
                await IsEmailUniqueAsync(userForRegisterDto.Email)
            );

            if (ruleResult != null)
            {
                return new ErrorDataResult<User>(Messages.CouldNotCreateUser);
            }

            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(userForRegisterDto.Password, out passwordHash, out passwordSalt);

            var user = new User
            {
                Email = userForRegisterDto.Email,
                FirstName = userForRegisterDto.FirstName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = "active"
            };

            await _userService.AddAsync(user);



            return new SuccessDataResult<User>(user, Messages.UserRegistered);
        }

        [ValidationAspect(typeof(UserLoginWithMailValidator))]
        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<User>> LoginWithEmailAsync(UserMailLoginDto userMailLoginDto)
        {
            var userToCheck = await _userService.GetByMailAsync(userMailLoginDto.Email);

            var ruleResult = BusinessRules.Run(await UserExistsAsync(userMailLoginDto.Email));
            if (ruleResult != null)
            {
                return new ErrorDataResult<User>(Messages.CouldNotFindUser2);
            }

            if (userToCheck.Data == null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }

            var user = userToCheck.Data;

            if (user.EndOfSuspension <= DateTime.Now && user.Status == "suspended")
            {
                user.Status = "active";
            }

            if (user.Status == "active")
            {
                if (!HashingHelper.VerifyPasswordHash(userMailLoginDto.Password, user.PasswordHash, user.PasswordSalt))
                {
                    user.FailedRecentLoginAttempts += 1;

                    if (user.FailedRecentLoginAttempts == 3)
                    {
                        user.Status = "suspended";
                        user.FailedRecentLoginAttempts = 0;
                        user.EndOfSuspension = DateTime.Now.AddMinutes(10);

     
                    }

                    await _userService.UpdateAsync(user);

                    if (user.Status == "active")
                    {
                        return new ErrorDataResult<User>(Messages.PasswordError);
                    }

                    return new ErrorDataResult<User>(Messages.UserSuspended);
                }

                return new SuccessDataResult<User>(user, Messages.SuccessfulLogin);
            }

            return new ErrorDataResult<User>(Messages.UserSuspended);
        }

        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<AccessToken>> CreateAccessTokenAsync(User user)
        {
            var claimsResult = await _userService.GetClaimsAsync(user);
            var accessToken = _tokenHelper.CreateToken(user, claimsResult.Data);
            return new SuccessDataResult<AccessToken>(accessToken, Messages.AccessTokenCreated);
        }

        private async Task<IResult> UserExistsAsync(string email)
        {
            var exists = await _userDal.AnyAsync(x => x.Email == email);
            if (!exists)
            {
                return new ErrorResult();
            }
            return new SuccessResult();
        }




        private async Task<IResult> IsEmailUniqueAsync(string email)
        {
            var result = await _userService.GetByMailAsync(email);
            if (result.Success && result.Data != null)
            {
                return new ErrorResult(Messages.EmailOnUse);
            }
            return new SuccessResult();
        }
    }
}
