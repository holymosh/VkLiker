using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Entities;

namespace Database
{
    public class TransactionManager
    {
        private VkContext _context;

        public TransactionManager(VkContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : BaseEntity
        {
            return _context.Set<TEntity>().AsQueryable();
        }

    }
}
