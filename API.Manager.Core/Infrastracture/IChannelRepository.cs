using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core.Infrastracture
{
    public interface IChannelRepository
    {
        Task<IList<string>> GetAsync(CancellationToken cancellationToken = default);
        Task AddAsync(string channel, CancellationToken cancellationToken = default);
        Task DeleteAsync(string channel, CancellationToken cancellationToken = default);
    }
}
