using API.Manager.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core
{
    public interface IManagerService
    {
        Task<IList<string>> GetChannelsAsync(CancellationToken cancellationToken = default);
        Task<IList<string>> GetProjectsAsync(CancellationToken cancellationToken = default);
        Task<IList<string>> GetControllersByProjectNameAsync(string projectName, CancellationToken cancellationToken = default);
        Task<IList<Service>> GetMethodsByControllerNameAsync(string controllerName, CancellationToken cancellationToken = default);
        Task UpdateServiceStatusByChannel(string channel, bool isServiceable, CancellationToken cancellationToken = default);
        Task UpdateServiceStatusByProject(string project, bool isServiceable, CancellationToken cancellationToken = default);
        Task UpdateServiceStatusByController(string controller, bool isServiceable, CancellationToken cancellationToken = default);
        Task UpdateServiceStatusByIdAsync(int id, bool isServiceable, CancellationToken cancellation);
        Task<bool> IsServiceable(Service service, CancellationToken cancellationToken = default);
    }
}
