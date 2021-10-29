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
        private static readonly Semaphore _locker = new Semaphore(1, 1);

        private readonly IMemoryCache _memoryCache;
        private readonly IChannelRepository _channelRepository;
        private readonly IServiceRepository _serviceRepository;

        public ManagerService(IChannelRepository channelRepository, IServiceRepository serviceRepository, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _channelRepository = channelRepository;
            _serviceRepository = serviceRepository;
        }

        private bool GetIsServiceableInfo(IList<Service> services, Service service)
        {
            var matchedService = services.FirstOrDefault(c =>
             c.Channel == service.Channel &&
             c.Project == service.Project &&
             c.Controller == service.Controller &&
             c.Method == service.Method &&
             c.MethodType == service.MethodType);

            if (matchedService is not null)
                return matchedService.IsServiceable;
            else
                return _serviceRepository.IsServiceableAsync(service, default).Result;
        }

        private IList<Service> GetServicesFromCache(Service service)
        {
            return _memoryCache.Get<IList<Service>>(CacheKey);
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
            _memoryCache.Remove(CacheKey);
        }

        public bool IsServiceable(Service service)
        {
            var data = GetServicesFromCache(service);

            if (data is not null && data.Any())
                return GetIsServiceableInfo(data, service);

            _locker.WaitOne();
            try
            {
                if (data is not null && data.Any())
                    return GetIsServiceableInfo(data, service);

                data = _serviceRepository.GetAllAsync(default).Result;

                if (data is not null && data.Any())
                {
                    _memoryCache.Set(CacheKey, data, new MemoryCacheEntryOptions() { SlidingExpiration = SlidingTime });
                    return GetIsServiceableInfo(data, service);
                }
            }
            catch
            {
                //ignored
            }
            finally
            {
                _locker.Release();
            }

            return _serviceRepository.IsServiceableAsync(service, default).Result;
        }
    }
}
