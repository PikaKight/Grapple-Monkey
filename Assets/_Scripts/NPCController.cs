using UnityEngine;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;        // drag the same Animator component here

    [Header("detection")]
    public float interactionRange = 3f;
    public KeyCode interactKey = KeyCode.E;
    public GameObject prompt;       // “Press E to interact” UI element

    [Header("dialog")]
    public string greeting = "hello traveler! what can i help you with?";
    public string farewell = "farewell! stay safe on your journey.";

    [Header("options")]
    public List<string> optionText;     // e.g. “heal”, “speed boost”, “immunity”
    public List<int> optionCost;        // cost in coins
    public List<float> effectValue;     // heal amount / speed amount
    public List<float> effectDuration;  // duration for speed/immunity

    // internal
    private PlayerMovement player;
    private Transform _player;
    private NPCUIManager _ui;
    private bool _inRange, _busy;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        _player = GameObject.FindWithTag("Player").transform;
        _ui = FindObjectOfType<NPCUIManager>();

        // hide prompt + UI
        prompt.SetActive(false);
        _ui.HideAll();
    }

    void Update()
    {
        if (_busy) return;

        // check range
        float d = Vector3.Distance(transform.position, _player.position);
        bool now = d <= interactionRange;
        if (now != _inRange)
            prompt.SetActive(now);
        _inRange = now;

        // if in range & press E, open dialog
        if (_inRange && Input.GetKeyDown(interactKey))
            OpenDialog();
    }

    void OpenDialog()
    {
        _busy = true;
        prompt.SetActive(false);

        // build labels & interactable flags
        int coins = PlayerPrefs.GetInt("Flames", 0);
        var labels = new List<string>();
        var interact = new List<bool>();
        for (int i = 0; i < optionText.Count; i++)
        {
            labels.Add($"{optionText[i]} ({optionCost[i]} coins)");
            interact.Add(coins >= optionCost[i]);
        }

        // update UI
        _ui.UpdateCoinDisplay(coins);

        player.changeFlames(coins);

        _ui.ShowGreeting(
            greeting,
            labels,
            interact,
            farewell
        );
    }

    /// <summary>
    /// called by UI when an option button is clicked
    /// </summary>
    public void PurchaseOption(int idx)
    {
        int coins = PlayerPrefs.GetInt("Flames", 0);
        if (coins < optionCost[idx]) return;

        // deduct the cost
        coins -= optionCost[idx];
        PlayerPrefs.SetInt("Flames", coins);

        switch (idx)
        {
            case 0: // heal
                int h = PlayerPrefs.GetInt("Health", 100);
                PlayerPrefs.SetInt("Health", h + Mathf.RoundToInt(effectValue[0]));
                break;

            case 1: // speed boost
                var pm = FindObjectOfType<PlayerMovement>();
                if (pm != null)
                    pm.StartCoroutine(
                        pm.BoostSpeed(effectValue[1], effectDuration[1])
                    );
                break;

            case 2: // immunity
                var ih = FindObjectOfType<ImmunityHandler>();
                if (ih != null)
                    ih.grantImmunity(effectDuration[2]);
                break;
        }

        // refresh coins display + show farewell
        _ui.UpdateCoinDisplay(coins);
        _ui.ShowFarewell(farewell);
    }


    /// <summary>
    /// called by UI when the farewell popup has fully closed
    /// </summary>
    public void OnDialogClosed()
    {
        _busy = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
