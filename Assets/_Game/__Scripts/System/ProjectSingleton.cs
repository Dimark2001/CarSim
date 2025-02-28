using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class ProjectSingleton
{
    public bool IsInitialized;
    
    public abstract UniTask Initialize();

    public abstract UniTask AfterInitialize();
}