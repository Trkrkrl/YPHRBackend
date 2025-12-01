using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Validation;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Logger;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System.Linq.Expressions;

namespace Business.Concrete
{
    public class TitleManager : ITitleService
    {
        private readonly ITitleDal _titleDal;

        public TitleManager(ITitleDal titleDal)
        {
            _titleDal = titleDal;
        }

        [CacheRemoveAspect("ITitleService.Get")]
        [ValidationAspect(typeof(TitleValidator))]
        [SecuredOperation("admin")]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> AddAsync(Title entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();

            }
            await _titleDal.AddAsync(entity);
            return new SuccessResult(Messages.TitleAdded);
        }

        [CacheRemoveAspect("ITitleService.Get")]
        [SecuredOperation("admin")]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> DeleteAsync(Title entity)
        {
            await _titleDal.DeleteAsync(entity);
            return new SuccessResult(Messages.TitleDeleted);
        }

        [CacheRemoveAspect("ITitleService.Get")]
        [ValidationAspect(typeof(TitleValidator))]
        [SecuredOperation("admin")]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> UpdateAsync(Title entity)
        {
            await _titleDal.UpdateAsync(entity);
            return new SuccessResult(Messages.TitleUpdated);
        }

        [CacheAspect]
        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<List<Title>>> GetAllAsync(int page, int pageSize, string searchParam)
        {
            Expression<Func<Title, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(searchParam))
            {
                var term = searchParam.Trim().ToLower();
                filter = t =>
                    t.Name.ToLower().Contains(term) ||
                    (t.Description != null && t.Description.ToLower().Contains(term));
            }

            var data = await _titleDal.GetAllAsync(filter, page, pageSize);

            return new SuccessDataResult<List<Title>>(data, Messages.TitleListed);
        }

        [CacheAspect]
        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<Title>> GetByIdAsync(Guid id)
        {
            var entity = await _titleDal.GetByIdAsync(id);
            if (entity == null)
            {
                return new ErrorDataResult<Title>(Messages.TitleNotFound);
            }

            return new SuccessDataResult<Title>(entity, Messages.TitleListed);
        }
    }
}
