using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    public enum InstanceType : byte {
        Singleton = 0,
        Transient = 1
    }

    public class IOCContainer {
        private ConcurrentDictionary<Type, object> singeltons = new ConcurrentDictionary<Type, object>();
        private ConcurrentDictionary<Type, Type> types = new ConcurrentDictionary<Type, Type>();

        public static IOCContainer Instance { get; } = new IOCContainer();
        public void Register<I, T>() where T : I {
            singeltons.TryAdd(typeof(I), Activator.CreateInstance(typeof(T)));
            types.TryAdd(typeof(I), typeof(T));
        }

        public void Register<I, T>(object obj) {
            singeltons.TryAdd(typeof(I), obj);
            types.TryAdd(typeof(I), typeof(T));
        }

        public I GetInstance<I>(InstanceType type = InstanceType.Transient) {
            if (type == InstanceType.Transient && types.TryGetValue(typeof(I), out Type t))
                return (I)Activator.CreateInstance(t);
            else if (singeltons.TryGetValue(typeof(I), out object obj))
                return (I)obj;
            else
                throw new InvalidOperationException("Unable to find registered type.");
        }
    }
}
