using UnityEngine;

namespace Helpers
{
public static class HelperMethods
{
    public static bool LayerInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask & (1 << layer)) != 0;
    }
}
}