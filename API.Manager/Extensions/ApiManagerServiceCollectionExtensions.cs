﻿using API.Manager.Core;
using API.Manager.Infrastracture;
using API.Manager.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Data.SqlClient;

namespace API.Manager.Extensions
{
    public static class ApiManagerServiceCollectionExtensions
    {
        private const string DefaultSchema = "ApiManager";
        private const string DefaultHeaderKey = "Channel";

        private static void ValidateOptions(ApiManagerOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (options.Channels is null || options.Channels.Length <= 0)
                throw new ArgumentNullException(nameof(options.Channels));

            if (string.IsNullOrWhiteSpace(options.Schema))
                options.Schema = DefaultSchema;

            if (!options.CreateTableIfNeccassary.HasValue)
                options.CreateTableIfNeccassary = true;

            if (string.IsNullOrWhiteSpace(options.HeaderKey))
                options.HeaderKey = DefaultHeaderKey;
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
            catch (Exception)
            {
                throw;
            }

        }

        public static IServiceCollection AddApiManager(this IServiceCollection services, string connectionString, ApiManagerOptions options)
        {
            ValidateOptions(options);

            services.AddSingleton(options);
            services.AddTransient<IDbConnection>(db => new SqlConnection(connectionString));
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<IChannelRepository, ChannelRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IPreparatoryService, PreparatoryService>();
            services.AddScoped<IPreparatoryRepository, PreparatoryRepository>();

            return services;
        }
    }
}