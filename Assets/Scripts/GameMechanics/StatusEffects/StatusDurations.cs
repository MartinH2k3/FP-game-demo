using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics.StatusEffects
{
[CreateAssetMenu(menuName = "ScriptableObjects/StatusEffects/DurationTable")]
public class StatusDurations: ScriptableObject {
    [System.Serializable]
    public class DurationRow {
        public Effect effect;
        [Tooltip("Duration by strength from 1 to 10 (index 0 = strength 1)")]
        public List<float> durations = new(new float[10]);
    }

    [SerializeField] private List<DurationRow> durationRows = new();

    private Dictionary<Effect, List<float>> _durationTable;

    public static StatusDurations Instance { get; private set; }
    private void OnEnable() {
        Instance = this;

        _durationTable = new Dictionary<Effect, List<float>>();

        foreach (var row in durationRows) {
            if (!_durationTable.ContainsKey(row.effect)) {
                _durationTable[row.effect] = row.durations;
            }
            else {
                Debug.LogWarning($"Duplicate effect {row.effect} found in StatusDurations. Only the first entry will be used.");
            }
        }
    }

    public float GetDuration(Effect effect, int strength) {
        if (_durationTable.TryGetValue(effect, out var durations)) {
            if (strength < 1 || strength > durations.Count) {
                Debug.LogWarning($"Invalid strength {strength} for effect {effect}. Returning 0.");
                return 0f;
            }
            return durations[strength - 1];
        }
        Debug.LogWarning($"Effect {effect} not found in StatusDurations. Returning 0.");
        return 0f;
    }
}
}