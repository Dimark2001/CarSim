using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class DIContainer
{
    private static readonly Dictionary<Type, object> Instances = new();
    private static bool _isAutoRegistered;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterDependencies()
    {
        if (_isAutoRegistered) 
            return;
        
        var registeredTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.GetCustomAttribute<RegisterAttribute>() != null);

        foreach (var type in registeredTypes)
        {
            if (type.IsAbstract || type.IsInterface)
                continue;

            var instance = Activator.CreateInstance(type);
            RegisterInstance(instance);
        }

        _isAutoRegistered = true;
    }
    
    public static void RegisterInstance(object instance)
    {
        Instances[instance.GetType()] = instance;
    }

    public static object Resolve(Type type)
    {
        if (Instances.TryGetValue(type, out var instance))
            return instance;
        
        throw new Exception($"Dependency of type {type} not registered");
    }

    public static void ClearSceneBindings()
    {
        Instances.Clear();
        _isAutoRegistered = false;
    }
}