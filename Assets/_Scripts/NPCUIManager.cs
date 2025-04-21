using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NPCUIManager : MonoBehaviour
{
    [Header("panel references")]
    public GameObject dialogPanel;         // parent of all dialog UI
    public TextMeshProUGUI dialogText;     // for greeting/farewell
    public GameObject optionsPanel;        // container under which buttons are spawned
    public GameObject buttonPrefab;        // prefab with Button + TMP child
    public TextMeshProUGUI coinText;       // shows “coins: X”

    [Header("timing")]
    public float farewellDelay = 2f;

    // internal
    private List<GameObject> _spawned = new List<GameObject>();
    private string _currentFarewell;       // remember the NPC’s farewell text

    void Awake()
    {
        HideAll();
    }

    /// <summary>
    /// hides everything
    /// </summary>
    public void HideAll()
    {
        dialogPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    /// <summary>
    /// show a greeting + dynamically spawn option buttons + exit
    /// </summary>
    /// <param name="greeting">what NPC says first</param>
    /// <param name="labels">button labels (with cost text)</param>
    /// <param name="interactable">can player afford each?</param>
    /// <param name="farewell">what to say on exit</param>
    public void ShowGreeting(
        string greeting,
        List<string> labels,
        List<bool> interactable,
        string farewell
    )
    {
        // remember the NPC’s farewell for the exit button
        _currentFarewell = farewell;

        // clear old buttons
        foreach (var go in _spawned) Destroy(go);
        _spawned.Clear();

        // show the dialog panel
        dialogPanel.SetActive(true);
        dialogText.text = greeting;

        // spawn options
        optionsPanel.SetActive(true);
        for (int i = 0; i < labels.Count; i++)
        {
            var btnObj = Instantiate(buttonPrefab, optionsPanel.transform);
            _spawned.Add(btnObj);

            // label
            var txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = labels[i];

            // interactable
            var b = btnObj.GetComponent<Button>();
            b.interactable = interactable[i];

            // hook click
            int idx = i; // capture
            b.onClick.AddListener(() => OnOptionPicked(idx));
        }

        // spawn Exit button
        var exitBtn = Instantiate(buttonPrefab, optionsPanel.transform);
        _spawned.Add(exitBtn);

        exitBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Exit";
        exitBtn.GetComponent<Button>()
               .onClick.AddListener(() => ShowFarewell(_currentFarewell));
    }

    void OnOptionPicked(int idx)
    {
        // hand off to NPCController
        FindObjectOfType<NPCController>().PurchaseOption(idx);
    }

    /// <summary>
    /// show the farewell text (always overwrite) then close
    /// </summary>
    public void ShowFarewell(string farewell)
    {
        // hide the option buttons
        optionsPanel.SetActive(false);

        // overwrite the dialog text with the farewell
        dialogText.text = farewell;

        // wait, then hide everything and notify NPCController
        StartCoroutine(CloseAfter());
    }

    IEnumerator CloseAfter()
    {
        yield return new WaitForSeconds(farewellDelay);
        HideAll();
        FindObjectOfType<NPCController>().OnDialogClosed();
    }

    /// <summary>
    /// update the coin count display
    /// </summary>
    public void UpdateCoinDisplay(int coins)
    {
        coinText.text = $"Sacred Flames: {coins}";
    }
}
