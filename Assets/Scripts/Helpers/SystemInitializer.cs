using System;
using GameMechanics.StatusEffects;
using UnityEngine;

namespace Helpers
{
public class SystemInitializer: MonoBehaviour
{
    [SerializeField] private StatusDurations durationTable;
    [SerializeField] private SlowFactors slowFactors;
    private void Awake() {
        _ = durationTable;
    }
}
}