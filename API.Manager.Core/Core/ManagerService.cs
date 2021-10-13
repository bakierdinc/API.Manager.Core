using API.Manager.Core.Models;
using API.Manager.Core.Infrastracture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core
{
    public class ManagerService : ServiceBase, IManagerService
    {
        private readonly IChannelRepository _channelRepository;
        private readonly IServiceRepository _serviceRepository;

        public ManagerService(IChannelRepository channelRepository, IServiceRepository serviceRepository)
        {
            _channelRepository = channelRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<IList<string>> GetChannelsAsync(CancellationToken cancellationToken = default)
        {
            return await _channelRepository.GetAsync(cancellationToken);
        }

        public async Task<IList<string>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            return await _serviceRepository.GetProjectsAsync(cancellationToken);
        }

        public async Task<IList<string>> GetControllersByProjectNameAsync(string projectName, CancellationToken cancellationToken = default)
        {
            return await _serviceRepository.GetControllersByProjectNameAsync(projectName, cancellationToken);
        }

        public async Task<IList<Service>> GetMethodsByControllerNameAsync(string controllerName, CancellationToken cancellationToken = default)
        {
            return await _serviceRepository.GetMethodsByControllerNameAsync(controllerName, cancellationToken);
        }

        public async Task UpdateServiceStatusByChannel(string channel, bool isServiceable, CancellationToken cancellationToken = default)
        {
            await _serviceRepository.UpdateServiceStatusByChannel(channel, isServiceable, cancellationToken);
        }

        public async Task UpdateServiceStatusByProject(string project, bool isServiceable, CancellationToken cancellationToken = default)
        {
            await _serviceRepository.UpdateServiceStatusByProject(project, isServiceable, cancellationToken);
        }

        public async Task UpdateServiceStatusByController(string controller, bool isServiceable, CancellationToken cancellationToken = default)
        {
            await _serviceRepository.UpdateServiceStatusByController(controller, isServiceable, cancellationToken);
        }

        public async Task UpdateServiceStatusByIdAsync(int id, bool isServiceable, CancellationToken cancellationToken = default)
        {
            await _serviceRepository.UpdateServiceStatusByIdAsync(id, isServiceable, cancellationToken);
        }

        public async Task<bool> IsServiceable(Service service, CancellationToken cancellationToken = default)
        {
            return await _serviceRepository.IsServiceableAsync(service, cancellationToken);
        }
    }
}
