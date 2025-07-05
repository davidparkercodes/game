using System.Threading;
using System.Threading.Tasks;

namespace Game.Application.Shared.Cqrs;

public interface IQueryHandler<in TQuery, TResponse> 
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
