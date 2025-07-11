using System.Collections.Generic;
using UnityEngine;

namespace Helpers {

public class PrefabDictionary: MonoBehaviour {
    [SerializeField] private GameObject[] prefabs;
    private Dictionary<string, GameObject> _prefabDictionary = new Dictionary<string, GameObject>();
    public static PrefabDictionary Instance { get; private set; }
    private void OnEnable() {
        Instance = this;
        foreach (var prefab in prefabs) {
            _prefabDictionary[prefab.name] = prefab;
        }
    }

    public T GetPrefab<T>(string prefabName) {
        prefabName = prefabName.EndsWith("(Clone)") ? prefabName.Replace("(Clone)", "").Trim() : prefabName;
        if (_prefabDictionary.TryGetValue(prefabName, out var prefab)) {
            return prefab.GetComponent<T>();
        }
        Debug.LogWarning($"Prefab with name {prefabName} not found in PrefabDictionary.");
        return default;
    }

    public T GetPrefab<T>(T item) where T : Component {
        return GetPrefab<T>(item.gameObject.name);
    }
}

}