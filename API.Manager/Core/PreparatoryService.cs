using API.Manager.Attribute;
using API.Manager.Core.Models;
using API.Manager.Infrastracture;
using API.Manager.Options;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core
{
    public class PreparatoryService : ServiceBase, IPreparatoryService
    {
        private readonly ApiManagerOptions _options;
        private readonly IPreparatoryRepository _preparatoryRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IServiceRepository _serviceRepository;

        public PreparatoryService(IPreparatoryRepository preparatoryRepository, IServiceRepository serviceRepository, IChannelRepository channelRepository, ApiManagerOptions options)
        {
            _options = options;
            _preparatoryRepository = preparatoryRepository;
            _channelRepository = channelRepository;
            _serviceRepository = serviceRepository;
        }

        private bool GetCustomAttributePredicate(CustomAttributeData data)
        {
            IList<Type> attributeTypes = new List<Type>();

            attributeTypes.Add(typeof(HttpGetAttribute));
            attributeTypes.Add(typeof(HttpPostAttribute));
            attributeTypes.Add(typeof(HttpPutAttribute));
            attributeTypes.Add(typeof(HttpDeleteAttribute));

            if (attributeTypes.Contains(data.AttributeType))
                return true;
            else
                return false;
        }

        private Task<IList<string>> GetExistChannels(CancellationToken cancellationToken = default)
        {
            return _channelRepository.GetAsync(cancellationToken);
        }

        private async Task AddChannelsIfNotExist(IList<string> existChannels, CancellationToken cancellationToken = default)
        {
            var newChannels = _options.Channels.Where(channel => !existChannels.Contains(channel));

            foreach (var channel in newChannels)
                await _channelRepository.AddAsync(channel, cancellationToken);
        }

        private async Task DeleteDifferentChannelsAndRelations(IList<string> existChannels, CancellationToken cancellationToken = default)
        {
            var differenceList = existChannels.Where(channel => !_options.Channels.Contains(channel));

            foreach (var differentChannel in differenceList)
                await _channelRepository.DeleteAsync(differentChannel, cancellationToken);
        }

        private async Task ClearDeletedServicesFromDb(IList<Service> existServices, IList<Service> services, CancellationToken cancellationToken = default)
        {
            IList<int> deleteListIds;

            if (services is null || !services.Any())
            {
                deleteListIds = existServices.Select(s => s.Id).ToList();
                await _serviceRepository.ClearDeletedServicesFromDbAsync(deleteListIds, cancellationToken);
            }
            else
            {
                deleteListIds = existServices.Where(existService => !services.Any(service =>
                       existService.Project == service.Project
                       && existService.Controller == service.Controller
                       && existService.Method == service.Method
                       && existService.MethodType == service.MethodType)).Select(s => s.Id).ToList();

                if (deleteListIds is not null || deleteListIds.Any())
                    await _serviceRepository.ClearDeletedServicesFromDbAsync(deleteListIds, cancellationToken);
            }
        }

        private async Task DeleteExistServicesFromCreatedServices(IList<Service> existServices, IList<Service> services, CancellationToken cancellationToken = default)
        {
            var existList = services.Where(service => existServices.Any(existService =>
                     existService.Project == service.Project
                     && existService.Controller == service.Controller
                     && existService.Method == service.Method
                     && existService.MethodType == service.MethodType
                     && existService.Channel == service.Channel)).ToList();


            foreach (var existService in existList)
                services.Remove(existService);

            await FromResult(services);
        }

        private async Task<IList<Service>> CreateServicesWithTypes(IEnumerable<Type> controllers, IList<Service> existServices, CancellationToken cancellationToken = default)
        {
            IList<Service> services = new List<Service>();

            for (int i = 0; i < _options.Channels.Length; i++)
            {
                foreach (var controller in controllers)
                {
                    var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(method => method.CustomAttributes.Any(attribute => GetCustomAttributePredicate(attribute)));

                    foreach (var method in methods)
                    {
                        Service service = new Service();
                        service.Project = controller.Assembly.GetName().Name;
                        service.Controller = controller.Name;

                        if (method.CustomAttributes != null && method.CustomAttributes.Any())
                        {
                            var attributes = method.CustomAttributes.FirstOrDefault(attribute => GetCustomAttributePredicate(attribute));

                            if (attributes != null)
                            {
                                service.MethodType = attributes.AttributeType.Name.Replace("Attribute", string.Empty).Replace("Http", string.Empty).Trim().ToUpper();
                                service.IsServiceable = _options.IsServiceable;
                                service.Channel = _options.Channels[i];

                                if (attributes.ConstructorArguments != null && attributes.ConstructorArguments.Count > 0)
                                {
                                    var value = attributes.ConstructorArguments.FirstOrDefault().Value;

                                    if (value != null)
                                        service.Method = !string.IsNullOrWhiteSpace(value.ToString()) ? value.ToString() : method.Name;
                                }
                                else
                                {
                                    service.Method = method.Name;
                                }
                            }
                        }
                        services.Add(service);
                    }
                }
            }

            return await FromResult<IList<Service>>(services);
        }

        public async Task PrepareServiceData(CancellationToken cancellationToken = default)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            var controllerTypes = assembly.GetTypes().Where(type => type.CustomAttributes.Any(c => c.AttributeType == typeof(OnlyServiceableAttribute)));

            var existingChannels = await GetExistChannels(cancellationToken);

            await DeleteDifferentChannelsAndRelations(existingChannels, cancellationToken);

            await AddChannelsIfNotExist(existingChannels, cancellationToken);

            var existServices = await _serviceRepository.GetAllAsync(cancellationToken);

            var services = await CreateServicesWithTypes(controllerTypes, existServices, cancellationToken);

            await ClearDeletedServicesFromDb(existServices, services, cancellationToken);

            await DeleteExistServicesFromCreatedServices(existServices, services, cancellationToken);

            if (services.Count > 0)
                await _serviceRepository.AddBulkServiceAsync(services, _options.Channels, cancellationToken);
        }

        public async Task PrepareServiceTablesAsync(CancellationToken cancellationToken = default)
        {
            await _preparatoryRepository.PrepareServiceTablesAsync(cancellationToken);
        }
    }
}
