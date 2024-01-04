using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Mapster;
using TheStarters.Clients.Web.Application.Abstractions.Repository;
using TheStarters.Clients.Web.Domain.Common.Contracts;

namespace TheStarters.Clients.Web.Infrastructure.Context;

public class ApplicationDbRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T>
    where T : class, IEntity
{
    public ApplicationDbRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }
    protected override IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
        ApplySpecification(specification, false)
            .ProjectToType<TResult>();
}