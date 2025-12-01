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
using Microsoft.AspNetCore.Http;
using Core.Utilities.Helpers.FileHelpers;
using System.Linq.Expressions;

namespace Business.Concrete
{
    public class EmployeeManager : IEmployeeService
    {
        private readonly IEmployeeDal _employeeDal;
        private readonly IFileHelper _fileHelper;
        private IEmployeeDal personnelDal;

        public EmployeeManager(IEmployeeDal employeeDal, IFileHelper fileHelper)
        {
            _employeeDal = employeeDal;
            _fileHelper = fileHelper;
        }

        public EmployeeManager(IEmployeeDal employeeDal)
        {
            _employeeDal = employeeDal;
        }

        [CacheRemoveAspect("IEmployeeService.Get")]
        [ValidationAspect(typeof(EmployeeValidator))]
        [SecuredOperation("admin")]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> AddAsync(Employee entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();

            }
            await _employeeDal.AddAsync(entity);
            return new SuccessResult(Messages.EmployeeAdded);
        }

        [CacheRemoveAspect("IEmployeeService.Get")]
        [SecuredOperation("admin")]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> DeleteAsync(Employee entity)
        {
            await _employeeDal.DeleteAsync(entity);
            return new SuccessResult(Messages.EmployeeDeleted);
        }

        [CacheRemoveAspect("IEmployeeService.Get")]
        [ValidationAspect(typeof(EmployeeValidator))]
        [SecuredOperation("admin")]
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> UpdateAsync(Employee entity)
        {
            await _employeeDal.UpdateAsync(entity);
            return new SuccessResult(Messages.EmployeeUpdated);
        }

        [CacheAspect]
        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<List<Employee>>> GetAllAsync(int page, int pageSize, string? searchParam)
        {
            Expression<Func<Employee, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(searchParam))
            {
                var term = searchParam.Trim().ToLower();

                filter = e =>
                    e.FirstName.ToLower().Contains(term) ||
                    e.LastName.ToLower().Contains(term) ||
                    e.RegistryNumber.ToLower().Contains(term);
            }

            var data = await _employeeDal.GetAllAsync(filter, page, pageSize);
            return new SuccessDataResult<List<Employee>>(data, Messages.EmployeeListed);
        }

        [CacheAspect]
        [LogAspect(typeof(FileLogger))]
        public async Task<IDataResult<Employee>> GetByIdAsync(Guid id)
        {
            var entity = await _employeeDal.GetByIdAsync(id);
            if (entity == null)
            {
                return new ErrorDataResult<Employee>(Messages.EmployeeNotFound);
            }

            return new SuccessDataResult<Employee>(entity, Messages.EmployeeListed);
        }

        [CacheRemoveAspect("IEmployeeService.Get")]
        [SecuredOperation("admin")] 
        [LogAspect(typeof(FileLogger))]
        public async Task<IResult> UploadEmployeeImageAsync(IFormFile file, Guid employeeId)
        {
           
            var getEmployeeResult = await GetByIdAsync(employeeId);
            if (!getEmployeeResult.Success)
            {
                
                return new ErrorResult(Messages.EmployeeNotFound);
            }

            var employeeToUpdate = getEmployeeResult.Data;

            try
            {
               
                if (!string.IsNullOrEmpty(employeeToUpdate.ImagePath))
                {
                   await _fileHelper.DeleteAsync(employeeToUpdate.ImagePath);
                }

                var uploadedFilePath = await _fileHelper.UploadAsync(file, PathConstants.ImagesPath);


                employeeToUpdate.ImagePath = uploadedFilePath.Data;

                var updateResult = await UpdateAsync(employeeToUpdate);

                if (updateResult.Success)
                {
                    return new SuccessResult(Messages.EmployeeImageUploaded);
                }

                await _fileHelper.DeleteAsync(uploadedFilePath.Data);
                return new ErrorResult(updateResult.Message);
            }
            catch (Exception ex)
            {

                return new ErrorResult($"{Messages.EmployeeImageUploadFailed}: {ex.Message}");
            }
        }
    }
}
