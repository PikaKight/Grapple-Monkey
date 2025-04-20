// ImmunityHandler.cs
using UnityEngine;
using System.Collections;

public class ImmunityHandler : MonoBehaviour
{
    public bool isImmune { get; private set; } = false;
    public GameObject immunityEffect; // optional glow when immune

    void Awake()
    {
        // hide any effect at start
        if (immunityEffect) immunityEffect.SetActive(false);
    }

    // grant invulnerability for a few seconds
    public void grantImmunity(float duration)
    {
        StartCoroutine(immunityRoutine(duration));
    }

    IEnumerator immunityRoutine(float duration)
    {
        isImmune = true;
        if (immunityEffect) immunityEffect.SetActive(true);

        yield return new WaitForSeconds(duration);

        isImmune = false;
        if (immunityEffect) immunityEffect.SetActive(false);
    }

    // return true if player should take damage
    public bool shouldTakeDamage()
    {
        return !isImmune;
    }
}
