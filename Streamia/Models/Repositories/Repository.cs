using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Streamia.Models.Contexts;
using Streamia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Streamia.Models.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly StreamiaContext context;
        private DbSet<T> entitySet;

        public Repository(StreamiaContext context)
        {
            this.context = context;
            entitySet = context.Set<T>();
        }

        public async Task<T> Add(T entity)
        {
            await entitySet.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> Add(IEnumerable<T> entities)
        {
            await entitySet.AddRangeAsync(entities);
            await context.SaveChangesAsync();
            return entities;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await entitySet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll(string[] models)
        {
            IQueryable<T> record = entitySet;
            foreach (var model in models)
            {
                record = record.Include(model);
            }
            return await record.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await entitySet.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<T> GetById(int id, string[] models)
        {
            IQueryable<T> record = entitySet;
            foreach (var model in models)
            {
                record = record.Include(model);
            }
            return await record.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<T>> Search(Expression<Func<T, bool>> expression)
        {
            return await entitySet.Where(expression).ToListAsync();
        }

        public async Task<T> Edit(T entity)
        {
            entitySet.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> Edit(IEnumerable<T> entities)
        {
            entitySet.UpdateRange(entities);
            await context.SaveChangesAsync();
            return entities;
        }

        public async Task Delete(int id)
        {
            var entity = await entitySet.FirstOrDefaultAsync(e => e.Id == id);
            if (entity != null)
            {
                entitySet.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(Expression<Func<T, bool>> expression)
        {
            var entities = await entitySet.Where(expression).ToListAsync();
            if (entities != null)
            {
                entitySet.RemoveRange(entities);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> expression)
        {
            return await entitySet.AnyAsync(expression);
        }

        public async Task<IEnumerable<T>> Paginate(int skip, int take)
        {
            return await entitySet.OrderByDescending(m => m.Id).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> Count()
        {
            return await entitySet.CountAsync();
        }
    }
}
