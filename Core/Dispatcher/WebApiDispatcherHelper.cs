using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dispatcher
{
    internal class WebApiDispatcherHelper
    {
        private static ConcurrentDictionary<string, DispatcherInfo> _registeredMethods { get; } = new();

        public static DispatcherInfo? GetMethod(Type key)
        {
            return GetMethod(key.Name);
        }

        public static DispatcherInfo? GetMethod(string key)
        {
            if (_registeredMethods.TryGetValue(key, out DispatcherInfo? info) && info.Resolved)
                return info;
            return default;
        }

        public static bool TryRegisterMethod<TIn, TInstance>(string methodName)
        {
            return TryRegisterMethod<TInstance>(typeof(TIn), methodName);
        }

        public static bool TryRegisterMethod<TInstance>(Type typeRequest, string methodName)
        {
            var key = typeRequest.Name;
            return TryRegisterMethod<TInstance>(key, typeRequest, methodName);
        }

        public static bool TryRegisterMethod<TInstance>(string key, string methodName)
        {
            return TryRegisterMethod<TInstance>(key, null, methodName);
        }

        public static bool ReturnTypeIsAssignableTo<T>(DispatcherInfo info)
        {
            return info.ResponseType == typeof(T) || (info.ResponseType?.IsAssignableTo(typeof(T)) ?? false);
        }

        private static bool TryRegisterMethod<TInstance>(string key, Type? typeRequest, string methodName)
        {
            var typeInstance = typeof(TInstance);
            if (_registeredMethods.ContainsKey(key))
                return false;
            var info = new DispatcherInfo(key, typeInstance, methodName)
            {
                RequestType = typeRequest
            };
            var methodInfo = info.InstanceType.GetMethod(info.MethodName, typeRequest != null ? new[] { typeRequest } : Array.Empty<Type>());
            if (methodInfo == null)
            {
                _registeredMethods.AddOrUpdate(key, info, (_, _) => info);
                return false;
            }
            var asAsync = IsAsync(methodInfo);
            info.ResponseType = asAsync ? methodInfo.ReturnType.GenericTypeArguments[0] : methodInfo.ReturnType;
            info.AsAsync = asAsync;
            info.MethodInfo = methodInfo;
            info.Resolved = true;
            _registeredMethods.AddOrUpdate(key, info, (_, _) => info);
            return true;
        }

        private static bool IsAsync(MethodInfo info)
        {
            if (info.IsDefined(typeof(AsyncStateMachineAttribute), false) || (info.ReturnType.IsGenericType && info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)))
            {
                return true;
            }
            return false;
        }
    }
}
