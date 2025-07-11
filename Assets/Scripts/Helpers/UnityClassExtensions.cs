using UnityEngine;

namespace Helpers
{
public static class UnityClassExtensions {
    public static T Prefab<T>(this T item) where T : Component{
        return PrefabDictionary.Instance.GetPrefab(item);
    }
}
}