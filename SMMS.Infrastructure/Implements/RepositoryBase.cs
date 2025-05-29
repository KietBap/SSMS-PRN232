using Microsoft.EntityFrameworkCore;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Context;
using System.Linq.Expressions;


namespace SMMS.Infrastructure.Implements
{
	public class RepositoryBase<T> : IRepositoryBase<T> where T : class
	{
		private readonly DatabaseContext _context;

		public RepositoryBase(DatabaseContext context)
		{
			_context = context;
		}
		public void Create(T entity)
		{
			_context.Set<T>().Add(entity);
		}

		public void Delete(T entity)
		{
			_context.Set<T>().Remove(entity);
		}

		public IQueryable<T> FindAll(bool trackChanges)
		{
			if (!trackChanges) return _context.Set<T>().AsNoTracking();
			return _context.Set<T>();
		}

		public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
		{
			if (!trackChanges) return _context.Set<T>().Where(expression).AsNoTracking();
			return _context.Set<T>().Where(expression);
		}

		public void Update(T entity)
		{
			_context.Set<T>().Update(entity);
		}
	}
}
