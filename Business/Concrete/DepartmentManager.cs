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
    public class DepartmentManager : IDepartmentService
    {
        private readonly IDepartmentDal _departmentDal;

        public DepartmentManager(IDepartmentDal departmentDal)
        {
            _departmentDal = departmentDal;
        }

        [CacheRemoveAspect("IDepartmentService.Get")]
        [ValidationAspect(typeof(DepartmentValidator))]
        [SecuredOperation("admin")]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> AddAsync(Department entity)
        {
            if (entity.Id==Guid.Empty)
            {
                entity.Id = Guid.NewGuid();

            }
            await _departmentDal.AddAsync(entity);
            return new SuccessResult(Messages.DepartmentAdded);
        }

        [CacheRemoveAspect("IDepartmentService.Get")]
        [SecuredOperation("admin")]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> DeleteAsync(Department entity)
        {
            await _departmentDal.DeleteAsync(entity);
            return new SuccessResult(Messages.DepartmentDeleted);
        }

        [CacheRemoveAspect("IDepartmentService.Get")]
        [ValidationAspect(typeof(DepartmentValidator))]
        [SecuredOperation("admin")]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> UpdateAsync(Department entity)
        {
            await _departmentDal.UpdateAsync(entity);
            return new SuccessResult(Messages.DepartmentUpdated);
        }

        [CacheAspect]
        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<List<Department>>> GetAllAsync(int page, int pageSize,string? searchParam)
        {
            Expression<Func<Department, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(searchParam))
            {
                var term = searchParam.Trim().ToLower();


                filter = e =>
                    e.Description.ToLower().Contains(term) ||
                    e.Name.ToLower().Contains(term);
            }

            var data = await _departmentDal.GetAllAsync(filter, page, pageSize);
            return new SuccessDataResult<List<Department>>(data, Messages.DepartmentListed);
        }

        [CacheAspect]
        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<Department>> GetByIdAsync(Guid id)
        {
            var entity = await _departmentDal.GetByIdAsync(id);
            if (entity == null)
            {
                return new ErrorDataResult<Department>(Messages.DepartmentNotFound);
            }

            return new SuccessDataResult<Department>(entity, Messages.DepartmentListed);
        }
    }
}
