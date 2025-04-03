using System;
using UnityEngine;
using Object = UnityEngine.Object;

public static class DiFabric
{
    public static T Create<T>(params object[] args) where T : class
    {
        var instance = Activator.CreateInstance(typeof(T), args) as T;
        Injector.Inject(instance);
        return instance;
    }

    public static T InstantiatePrefab<T>(T prefab, Transform parent = null) where T : Object
    {
        var instance = Object.Instantiate(prefab, parent);
        
        switch (instance)
        {
            case GameObject gameObject:
                InjectDependenciesInGameObject(gameObject);
                break;
            case Component component:
                InjectDependenciesInGameObject(component.gameObject);
                break;
        }
        
        return instance;
    }

    private static void InjectDependenciesInGameObject(GameObject gameObject)
    {
        var components = gameObject.GetComponentsInChildren<MonoBehaviour>();
        foreach (var component in components)
        {
            Injector.Inject(component);
        }
    }
}