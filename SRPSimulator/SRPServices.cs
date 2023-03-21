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
            services.AddSingleton<Drive>();
            services.AddSingleton<PUnit>();
            services.AddSingleton<Fluid>();
            services.AddSingleton<Tubing>();
            services.AddSingleton<Rod>();
            services.AddSingleton<SRP>();
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
