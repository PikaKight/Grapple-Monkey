using UnityEngine;
using System.Collections;

public class ImmunityHandler : MonoBehaviour
{
    // true while the player should NOT take damage
    public bool isImmune { get; private set; } = false;

    // optional VFX to show when immune
    [Tooltip("optional effect to show while immune")]
    public GameObject immunityEffect;

    private void Awake()
    {
        // hide the effect at start
        if (immunityEffect)
            immunityEffect.SetActive(false);
    }

    /// <summary>
    /// grant immunity for exactly `duration` seconds
    /// </summary>
    public void GrantImmunity(float duration)
    {
        StartCoroutine(ImmunityCoroutine(duration));
    }

    private IEnumerator ImmunityCoroutine(float duration)
    {
        isImmune = true;
        if (immunityEffect)
            immunityEffect.SetActive(true);

        yield return new WaitForSeconds(duration);

        isImmune = false;
        if (immunityEffect)
            immunityEffect.SetActive(false);
    }

    /// <summary>
    /// return true if this object should take damage right now
    /// </summary>
    public bool ShouldTakeDamage()
    {
        return !isImmune;
    }
}
