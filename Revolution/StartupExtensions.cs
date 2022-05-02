using Core.Abstractions.Dispatcher;
using Core.Dispatcher;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Revolution
{
    public static class StartupExtensions
    {
        public static IServiceCollection RegisterDispatcher(this IServiceCollection services)
        {
            services.AddSingleton(WebApiDispatcherFactory);
            return services;
        }

        public static IDispatcher WebApiDispatcherFactory(IServiceProvider provider)
        {
            return new WebApiDispatcher(provider)
                .RegisterManager<DateTime, WeatherForecastService>(nameof(WeatherForecastService.GetForecastAsync));
        }

    }
}
