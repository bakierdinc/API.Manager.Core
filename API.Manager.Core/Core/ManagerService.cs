using API.Manager.Core.Infrastracture;
using API.Manager.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core
{
    public class ManagerService : ServiceBase, IManagerService
    {
        private const string CacheKey = "Methods";
        private TimeSpan SlidingTime = new TimeSpan(0, 30, 0);


        private readonly IMemoryCache _memoryCache;
        private readonly IChannelRepository _channelRepository;
        private readonly IServiceRepository _serviceRepository;

        public ManagerService(IChannelRepository channelRepository, IServiceRepository serviceRepository, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
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
            if (_memoryCache.TryGetValue(CacheKey, out IList<Service> services))
            {
                var matchedService = services.FirstOrDefault(c =>
                 c.Channel == service.Channel &&
                 c.Project == service.Project &&
                 c.Controller == service.Controller &&
                 c.Method == service.Method &&
                 c.MethodType == service.MethodType);

                return matchedService.IsServiceable;
            }
            else
            {
                services = await _serviceRepository.GetAllAsync(cancellationToken);
                _memoryCache.Set(CacheKey, services, new MemoryCacheEntryOptions() { SlidingExpiration = SlidingTime });
            }

            return await _serviceRepository.IsServiceableAsync(service, cancellationToken);
        }
    }
}
