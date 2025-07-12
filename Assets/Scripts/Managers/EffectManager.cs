using System;
using System.Collections;
using UnityEngine;
using Visuals;

namespace Managers
{

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }
    [SerializeField] private GameObject explosionPrefab;

    private void Awake() {
        // Singleton enforcement
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CallExplosion(Vector3 position, Quaternion rotation = default) {
        PlayEffectVisuals(explosionPrefab, position, rotation);
    }

    private void PlayEffectVisuals(GameObject prefab, Vector3 position, Quaternion rotation = default) {
        if (prefab == null) {
            Debug.LogWarning("Effect prefab is not assigned.");
            return;
        }

        var effectInstance = Instantiate(prefab, position, rotation);
        DestroyEffect(effectInstance);
    }

    private void DestroyEffect(GameObject effectInstance) {
        var effect = effectInstance.GetComponent<Effect>();
        var spriteRenderer = effectInstance.GetComponent<SpriteRenderer>();
        if (spriteRenderer is null) {
            Destroy(effectInstance, effect.duration);
        }
        StartCoroutine(FadeAndDestroy(effectInstance, effect, spriteRenderer));
    }

    private IEnumerator FadeAndDestroy(GameObject go, Effect ef, SpriteRenderer sr) {
        float duration = ef.duration;
        float elapsed = 0f;
        Color initialColor = sr.color;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = Mathf.Pow(t, ef.fadingSpeed);
            float alpha = Mathf.Lerp(1f, 0f, easedT);
            sr.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        Destroy(go);
    }


}

}