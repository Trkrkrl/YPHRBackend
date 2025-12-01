using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Logger;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using Entities.DTOs;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        [CacheRemoveAspect("IUserService.Get")]
        [LogAspect(typeof(FileLogger))]
        public async Task<Result> AddAsync(User user)
        {
            await _userDal.AddAsync(user);
            return new SuccessResult();
        }

        [CacheRemoveAspect("IUserService.Get")]
        [LogAspect(typeof(FileLogger))]
        public async Task<Result> DeleteAsync(User user)
        {
            await _userDal.DeleteAsync(user);
            return new Result(true, Messages.UserDeleted);
        }

        [CacheRemoveAspect("IUserService.Get")]
        [LogAspect(typeof(FileLogger))]
        public async Task<Result> UpdateAsync(User user)
        {
            await _userDal.UpdateAsync(user);
            return new Result(true);
        }

        [CacheAspect]
        [LogAspect(typeof(FileLogger))]
        public async Task<DataResult<User>> GetByMailAsync(string email)
        {
            var result = (await _userDal.GetAllAsync(u => u.Email == email)).FirstOrDefault();
            return new SuccessDataResult<User>(result);
        }

        [CacheAspect]
        [LogAspect(typeof(FileLogger))]
        public async Task<DataResult<User>> GetByIdAsync(Guid userId)
        {
            var user = (await _userDal.GetAllAsync(u => u.Id == userId)).FirstOrDefault();
            return new SuccessDataResult<User>(user);
        }

        [LogAspect(typeof(FileLogger))]
        public async Task<Result> UpdatePasswordAsync(UserPasswordUpdateDto userPasswordUpdateDto, Guid userId)
        {
            var user = (await GetByIdAsync(userId)).Data;
            if (user == null)
            {
                return new ErrorResult(Messages.CouldNotFindUser);
            }

            if (!HashingHelper.VerifyPasswordHash(userPasswordUpdateDto.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                return new ErrorResult(Messages.OldPasswordIsWrong);
            }

            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(userPasswordUpdateDto.NewPassword, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Status = "active";
            await _userDal.UpdateAsync(user);

            return new SuccessDataResult<User>(user, Messages.PasswordUpdated);
        }

        public async Task<IDataResult<List<OperationClaim>>> GetClaimsAsync(User user)
        {
            var claims = await _userDal.GetClaimsAsync(user);
            return new SuccessDataResult<List<OperationClaim>>(claims);
        }
    }
}
