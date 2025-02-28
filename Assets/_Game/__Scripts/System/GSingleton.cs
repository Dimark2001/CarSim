using Cysharp.Threading.Tasks;

public abstract class GSingleton
{
    public bool IsInitialized;
    
    public abstract UniTask Initialize();

    public abstract UniTask AfterInitialize();
}