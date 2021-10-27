using API.Manager.Core.Infrastracture;
using API.Manager.Core.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Data.SqlClient;

namespace API.Manager.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string DefaultSchemaKey = "ApiManager";
        private const string DefaultHeaderKey = "Channel";
        private static string[] DefaultChannels = { "Default" };

        private static void ValidateOptions(ApiManagerOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.Channels is null || options.Channels.Length <= 0)
                options.Channels = DefaultChannels;

            if (string.IsNullOrWhiteSpace(options.Schema))
                options.Schema = DefaultSchemaKey;

            if (!options.IsServiceable.HasValue)
                options.IsServiceable = true;

            if (!options.CreateTableIfNeccassary.HasValue)
                options.CreateTableIfNeccassary = true;

            if (string.IsNullOrWhiteSpace(options.HeaderKey))
                options.HeaderKey = DefaultHeaderKey;
        }
        
        private static IServiceCollection AddOptions(this IServiceCollection services, ApiManagerOptions options)
        {
            services.AddSingleton(options);
            return services;
        }

        private static IServiceCollection AddDbConnection(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            services.AddSingleton<IDbConnection>(db => new SqlConnection(connectionString));

            return services;
        }
        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<IChannelRepository, ChannelRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IPreparatoryService, PreparatoryService>();
            services.AddScoped<IPreparatoryRepository, PreparatoryRepository>();

            return services;
        }

        public static void PrepareApiManager(this IServiceCollection services)
        {
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IPreparatoryService preparatoryService = serviceProvider.GetService<IPreparatoryService>();

            try
            {
                preparatoryService.PrepareServiceTablesAsync(default);
                preparatoryService.PrepareServiceDataAsync(default);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        public static IServiceCollection AddApiManager(this IServiceCollection services, string connectionString, ApiManagerOptions options)
        {
            ValidateOptions(options);

            services.AddOptions(options);
            services.AddDbConnection(connectionString);
            services.AddServices();
            services.AddMemoryCache();

            return services;
        }
    }
}
