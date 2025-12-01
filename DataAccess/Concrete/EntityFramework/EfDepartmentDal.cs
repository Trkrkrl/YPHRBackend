using Core.DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Context;
using DataAccess.Abstract;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfDepartmentDal : EfEntityBaseRepository<Department, AppDbContext>, IDepartmentDal
    {
        private AppDbContext dbContext;

        public EfDepartmentDal(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
