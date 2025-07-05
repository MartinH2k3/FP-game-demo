using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics.StatusEffects
{
[CreateAssetMenu(menuName = "ScriptableObjects/StatusEffects/SlowFactors")]
public class SlowFactors: ScriptableObject {
    public List<float> slowFactors = new List<float>(new float[10]);

    public float GetSlowFactor(int strength) {
        if (strength < 1 || strength > slowFactors.Count) {
            Debug.LogWarning($"Invalid strength {strength} for slow factor. Returning 0.");
            return 0f;
        }
        return slowFactors[strength - 1];
    }
}
}