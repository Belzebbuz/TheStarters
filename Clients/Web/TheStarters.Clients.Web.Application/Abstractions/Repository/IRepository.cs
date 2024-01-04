using Ardalis.Specification;
using TheStarters.Clients.Web.Domain.Common.Contracts;

namespace TheStarters.Clients.Web.Application.Abstractions.Repository;

public interface IRepository<T> : IRepositoryBase<T>
	where T : class, IEntity
{
}

public interface IReadRepository<T> : IReadRepositoryBase<T>
	where T : class, IEntity
{
}