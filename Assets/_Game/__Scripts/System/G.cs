using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class G
{
    public static bool IsInitialized { get; private set; }
    private static List<ProjectSingleton> _container;

    public static async UniTask Initialize()
    {
        Debug.Log("Project Initializing...");
        _container = new List<ProjectSingleton>();
        var subs = ReflectionUtil.FindAllSubslasses<ProjectSingleton>();
        foreach (var subclass in subs)
        {
            _container.Add(Activator.CreateInstance(subclass) as ProjectSingleton);
        }

        foreach (var projectSingleton in _container)
        {
            await projectSingleton.Initialize();
            projectSingleton.IsInitialized = true;
        }

        foreach (var projectSingleton in _container)
        {
            projectSingleton.AfterInitialize();
        }

        IsInitialized = true;
        Debug.Log("Project Initialized!");
    }

    public static T Get<T>() where T : ProjectSingleton
    {
        if (_container == null)
        {
            throw new Exception("Firstly Initialize Project");
        }

        var instance = _container.FirstOrDefault(c => c is T) as T;
        return instance;
    }

    public static List<T> GetAll<T>() where T : class
    {
        if (_container == null)
        {
            throw new Exception("Firstly Initialize Project");
        }

        var f = _container.FindAll(c => c is T).ConvertAll(c => c as T);
        return f;
    }

    public static void ActionForAll<T>(Action<T> action) where T : class
    {
        if (_container == null)
        {
            throw new Exception("Firstly Initialize Project");
        }

        var f = GetAll<T>();
        if (f.Count == 0)
        {
            return;
        }

        foreach (var p in f)
        {
            action?.Invoke(p);
        }
    }
}