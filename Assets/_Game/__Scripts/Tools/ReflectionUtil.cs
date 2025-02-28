using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class ReflectionUtil
{
    public static Type[] FindAllSubslasses<T>()
    {
        Type baseType = typeof(T);
        Assembly assembly = Assembly.GetAssembly(baseType);

        Type[] types = assembly.GetTypes();
        Type[] subclasses = types.Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract).ToArray();

        return subclasses;
    }

    public static void FillContainerWithNewInstances<T>(ref List<T> container) where T : class
    {
        var subs = FindAllSubslasses<T>();
        foreach (var subclass in subs)
        {
            container.Add(Activator.CreateInstance(subclass) as T);
        }
    }
}