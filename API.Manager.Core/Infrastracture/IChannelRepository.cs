using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core.Infrastracture
{
    public interface IChannelRepository
    {
        Task<IList<string>> GetAsync(CancellationToken cancellationToken = default);
        Task AddAsync(IEnumerable<string> channels, CancellationToken cancellationToken = default);
        Task DeleteAsync(IEnumerable<string> channels, CancellationToken cancellationToken = default);
    }
}
