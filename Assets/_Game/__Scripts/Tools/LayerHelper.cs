using System.Linq;
using UnityEngine;

public static class LayerHelper
{
    public static LayerMask NameToLayer(string layerName)
    {
        var maskValue = (1 << LayerMask.NameToLayer(layerName));
        var layerMask = new LayerMask { value = maskValue, };
        return layerMask;
    }
    
    public static LayerMask NamesToLayerMask(string[] layerNames)
    {
        var maskValue = layerNames.Aggregate(0, (current, name) => current | (1 << LayerMask.NameToLayer(name)));

        var layerMask = new LayerMask { value = maskValue, };

        return layerMask;
    }

    public static bool Contains(this LayerMask layerMask, int layer)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}