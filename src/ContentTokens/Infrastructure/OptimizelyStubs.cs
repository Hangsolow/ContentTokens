// Stub implementations for Optimizely/EPiServer types to allow compilation without dependencies
// In a real Optimizely installation, these would come from the EPiServer NuGet packages

namespace EPiServer.Data
{
    public class Identity
    {
        private readonly Guid? _id;

        private Identity(Guid? id)
        {
            _id = id;
        }

        public static Identity NewIdentity => new Identity(null);
        
        public static Identity Create(Guid id) => new Identity(id);

        public override string ToString() => _id?.ToString() ?? string.Empty;

        public static bool operator ==(Identity? a, Identity? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a._id == b._id;
        }

        public static bool operator !=(Identity? a, Identity? b) => !(a == b);

        public override bool Equals(object? obj) => obj is Identity other && this == other;

        public override int GetHashCode() => _id?.GetHashCode() ?? 0;
    }

    public interface IDynamicData
    {
        Identity Id { get; set; }
    }
}

namespace EPiServer.Data.Dynamic
{
    using EPiServer.Data;
    using System.Collections.Generic;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Class)]
    public class EPiServerDataStoreAttribute : Attribute
    {
        public bool AutomaticallyCreateStore { get; set; }
        public bool AutomaticallyRemapStore { get; set; }
    }

    public class DynamicDataStore
    {
        private readonly Dictionary<string, object> _store = new Dictionary<string, object>();

        public IQueryable<T> Items<T>() where T : IDynamicData
        {
            return _store.Values.OfType<T>().AsQueryable();
        }

        public void Save<T>(T item) where T : IDynamicData
        {
            if (item.Id == null || item.Id == Identity.NewIdentity)
            {
                item.Id = Identity.Create(Guid.NewGuid());
            }
            _store[item.Id.ToString()] = item;
        }

        public void Delete(Identity id)
        {
            _store.Remove(id.ToString());
        }
    }

    public class DynamicDataStoreFactory
    {
        private static DynamicDataStoreFactory? _instance;
        public static DynamicDataStoreFactory Instance => _instance ??= new DynamicDataStoreFactory();

        private readonly Dictionary<Type, DynamicDataStore> _stores = new Dictionary<Type, DynamicDataStore>();

        public DynamicDataStore? GetStore(Type type)
        {
            return _stores.TryGetValue(type, out var store) ? store : null;
        }

        public DynamicDataStore CreateStore(Type type)
        {
            var store = new DynamicDataStore();
            _stores[type] = store;
            return store;
        }
    }
}

namespace EPiServer.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InitializableModuleAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModuleDependencyAttribute : Attribute
    {
        public ModuleDependencyAttribute(Type type) { }
    }
}

namespace EPiServer.Framework.Initialization
{
    using Microsoft.Extensions.DependencyInjection;

    public interface IInitializableModule
    {
        void Initialize(InitializationEngine context);
        void Uninitialize(InitializationEngine context);
    }

    public interface IConfigurableModule : IInitializableModule
    {
        void ConfigureContainer(ServiceConfigurationContext context);
    }

    public class InitializationEngine
    {
    }

    public class ServiceConfigurationContext
    {
        public IServiceCollection Services { get; set; } = null!;
    }
}

namespace EPiServer.ServiceLocation
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceConfigurationAttribute : Attribute
    {
        public ServiceConfigurationAttribute(Type serviceType) { }
        public ServiceConfigurationAttribute(Type serviceType, Lifecycle lifecycle) { }
    }

    public enum Lifecycle
    {
        Singleton,
        Transient,
        Scoped
    }
}

namespace EPiServer.Globalization
{
    using System.Globalization;

    public static class ContentLanguage
    {
        public static CultureInfo? PreferredCulture => CultureInfo.CurrentUICulture;
    }
}

namespace EPiServer.Web
{
    using EPiServer.Framework.Initialization;

    public class InitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context) { }
        public void Uninitialize(InitializationEngine context) { }
    }
}

namespace EPiServer.Shell.UI
{
    using EPiServer.Framework.Initialization;

    public class InitializationModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context) { }
        public void Uninitialize(InitializationEngine context) { }
    }
}

namespace EPiServer.Shell.Modules
{
    public interface IModuleDescriptor
    {
        string Name { get; }
        IEnumerable<string> Dependencies { get; }
        string ModuleName { get; }
    }
}
