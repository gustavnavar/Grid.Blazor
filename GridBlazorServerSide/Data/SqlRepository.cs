﻿using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazorServerSide.Data
{
    public abstract class SqlRepository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<T> EfDbSet;

        protected SqlRepository(DbContext context)
        {
            Context = context;
            EfDbSet = context.Set<T>();
        }

        #region IRepository<T> Members

        public virtual IQueryable<T> GetAll()
        {
            return (IOrderedQueryable<T>)EfDbSet;
        }

        public abstract Task<T> GetById(object id);

        #endregion
    }
}