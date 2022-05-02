using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dispatcher
{
    internal class DispatcherInfo
    {
        public DispatcherInfo(string key, Type instanceType, string methodName)
        {
            Key = key;
            MethodName = methodName;
            InstanceType = instanceType;
        }
        public string Key { get; }
        public Type? RequestType { get; set; }
        public Type? ResponseType { get; set; }
        public Type InstanceType { get; set; }
        public string MethodName { get; set; }
        public MethodInfo? MethodInfo { get; set; }
        public bool AsAsync { get; set; }
        public bool Resolved { get; set; }
    }
}
