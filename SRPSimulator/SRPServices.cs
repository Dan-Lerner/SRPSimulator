using System;

using Microsoft.Extensions.DependencyInjection;

namespace SRPSimulator
{
    using MathModel;

    // DI Services

    class SRPServices
    {
        static IServiceCollection services;
        private static IServiceProvider Provider;

        public SRPServices()
        {
            services = new ServiceCollection();

            // Math model
            services.AddSingleton<DriveConfigBrowsable>();
            services.AddSingleton<VFCConfigBrowsable>();
            services.AddSingleton<FluidConfigBrowsable>();
            services.AddSingleton<TubingConfigBrowsable>();
            services.AddSingleton<RodConfigBrowsable>();
            services.AddSingleton<SRPConfigBrowsable>();
            services.AddSingleton<PUnitConfigBrowsable>();
            services.AddSingleton<Drive>();
            services.AddSingleton<VFC>();
            services.AddSingleton<Fluid>();
            services.AddSingleton<Tubing>();
            services.AddSingleton<Rod>();
            services.AddSingleton<SRP>();
            services.AddSingleton<PUnit>();
            services.AddSingleton<Simulator>();
            
            // HttpClient
            services.AddSingleton<JsonHttpClient>();

            Provider = services.BuildServiceProvider();
        }

        public static object GetService(Type type)
        {
            return Provider.GetService(type);
        }
    }
}
