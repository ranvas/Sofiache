using Core.Abstractions.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Dispatcher.WebApiDispatcherHelper;

namespace Core.Dispatcher
{
    public class WebApiDispatcher : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        public WebApiDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public WebApiDispatcher RegisterManager<TIn, TInstance>(string methodName)
        {
            if (!TryRegisterMethod<TIn, TInstance>(methodName))
            {
                //throw Exception or logging?
            }
            return this;
        }

        public async Task<TOut?> DispatchSimple<TIn, TOut>(TIn request)
        {
            var key = typeof(TIn).Name;
            return await DispatchSimple<TOut>(request, key);
        }

        public async Task<TOut?> DispatchSimple<TOut>(string key)
        {
            return await DispatchSimple<TOut>(null, key);
        }

        public async Task<TOut?> DispatchSimple<TIn, TOut>(string key, TIn request)
        {
            return await DispatchSimple<TOut>(request, key);
        }

        private async Task<TOut?> DispatchSimple<TOut>(object? request, string key)
        {
            var info = GetMethod(key);
            if (info == null)
                throw new NotImplementedException($"The handler for key {key} is not registered");
            if (!ReturnTypeIsAssignableTo<TOut>(info))
                throw new InvalidCastException($"The handler for key {key} returns {info.ResponseType}, although {typeof(TOut)} is expected");
            using var scope = _serviceProvider.CreateScope();
            return (TOut?)await Invoke(info, request, scope);
        }

        public async Task<TOut?> DispatchSpecified<TIn, TOut>(TIn request)
            where TIn : IDispatchedRequest
            where TOut : IDispatchedResponse
        {
            var key = typeof(TIn).Name;
            var info = GetMethod(key);
            if (info == null)
                throw new NotImplementedException($"The handler for key {key} is not registered");
            if (!ReturnTypeIsAssignableTo<TOut>(info))
                throw new InvalidCastException($"The handler for key {key} returns {info.ResponseType}, although {typeof(TOut)} is expected");

            using var scope = _serviceProvider.CreateScope();
            request = await DispatchRequest(request, scope);
            var response = await Invoke(info, request, scope);
            return await DispatchResponse<TOut>(response, scope);
        }

        private async Task<T> DispatchRequest<T>(T request, IServiceScope scope)
        {
            return await Task.FromResult(request);
        }

        private async Task<T?> DispatchResponse<T>(object? response, IServiceScope scope)
        {
            return await Task.FromResult((T?)response);
        }

        private static async Task<object?> Invoke(DispatcherInfo info, object? request, IServiceScope scope)
        {
            var manager = scope.ServiceProvider.GetService(info.InstanceType);
            object? response;
            if (info.AsAsync)
            {
                dynamic? task = info.MethodInfo!.Invoke(manager, request != null ? new[] { request } : null);
                response = task != null ? await task : null;
            }
            else
            {
                var task = Task.Run(() =>
                {
                    response = info.MethodInfo!.Invoke(manager, request != null ? new[] { request } : null);
                    return response;
                }).ConfigureAwait(false);
                response = await task;
            }
            return response;
        }
    }
}
