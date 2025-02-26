using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class Root : MonoBehaviour
{
    private static Root _this;

    [SerializeField]
    private List<Component> _components;

    private void Awake()
    {
        _this = this;
        _components = new List<Component>();
        DontDestroyOnLoad(gameObject);
    }

    public static T GetReference<T>(bool createNewComponentIfNull = false) where T : MonoBehaviour
    {
        var component = _this._components
            .Find(c => c.MonoBehComponent is T && c.MonoBehComponent != null)
            ?.MonoBehComponent as T;
        if (!component && createNewComponentIfNull)
        {
            return null;
        }

        return component;
    }

    public static MonoBehaviour GetReference(string name)
    {
        var component = _this._components.FirstOrDefault(c => c.Name == name && c.MonoBehComponent != null);
        return component?.MonoBehComponent;
    }

    public static List<T> GetItems<T>()
    {
        return _this._components.Select(c =>
            {
                if (c.MonoBehComponent is T pauseAble)
                {
                    return pauseAble;
                }

                return default;
            }
        ).Where(c => c != null).ToList();
    }

    public static MonoBehaviour RegisterComponent(MonoBehaviour monoBeh)
    {
        var nComponent = new Component(monoBeh);
        _this._components.Add(nComponent);
        return nComponent.MonoBehComponent;
    }

    public static void RegisterComponent(string name, MonoBehaviour monoBeh)
    {
        var nComponent = new Component(name, monoBeh);
        _this._components.Add(nComponent);
    }

    public static void RemoveComponent(MonoBehaviour monoBeh)
    {
        _this._components.Remove(_this._components.Find((c) => c.MonoBehComponent.Equals(monoBeh)));
    }

    public static void ClearReference()
    {
        Debug.Log("ClearReference");
        _this._components.RemoveAll(c => c == null || c.MonoBehComponent == null);
    }

    [System.Serializable]
    public class Component
    {
        [SerializeField]
        public string _name;

        public string Name => _name;

        [SerializeField]
        private MonoBehaviour _monoComponent;

        public MonoBehaviour MonoBehComponent => _monoComponent;

        public Component()
        {
            _name = string.Empty;
            _monoComponent = null;
        }

        public Component(MonoBehaviour mb)
        {
            _monoComponent = mb;
        }

        public Component(string name, MonoBehaviour mb)
        {
            _name = name;
            _monoComponent = mb;
        }
    }
}