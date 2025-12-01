using Core.DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Context;
using DataAccess.Abstract;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfTitleDal : EfEntityBaseRepository<Title, AppDbContext>, ITitleDal
    {
        private AppDbContext dbContext;

        public EfTitleDal(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
