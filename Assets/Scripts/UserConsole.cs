using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserConsole : MonoBehaviour
{
    public GameObject TextPrefab;
    public Transform Content;
    public Button ClearBtn;
    public Scrollbar Scrollbar;
    private readonly List<GameObject> _textEntries = new List<GameObject>();
    public readonly int ConsoleFontSize = 24;
    // Cleared all texts when the app starts
    private void Start()
    {
        ClearBtn.onClick.AddListener(ClearTexts);

    }
    public void AddObjectInfo(string info)
    {
        // Create a new text entry from the user actions
        GameObject newEntry = Instantiate(TextPrefab, Content);
        TMP_Text tmpText = newEntry.GetComponent<TMP_Text>();

        // Enable word wrapping and customize text appearance
        tmpText.enableWordWrapping = true;
        tmpText.fontSize = ConsoleFontSize; // Set fixed font size
        tmpText.text = info;

        _textEntries.Add(newEntry);

        // Update the height of the RectTransform based on text length
        AdjustLayout(newEntry, tmpText);
        // Use a coroutine to update the scrollbar value after the layout is updated
        StartCoroutine(UpdateScrollPosition());

    }

    public void AddObjectInfo(List<string> info)
    {
        // Create a new text entry from the user actions
        foreach (var textItem in info)
        {
            AddObjectInfo(textItem);
        }

        // Add a gap after each big output
        AddGap();

        //Clearing the list for non duplicate results
        info.Clear();   
    }

    // Use a coroutine to update the scrollbar value after the layout is updated
    private IEnumerator UpdateScrollPosition()
    {
        // Wait until the end of the frame so that the layout can update
        yield return new WaitForEndOfFrame();

        // Set the scrollbar value to the bottom
        Scrollbar.value = 0;
    }

    // Adjust the height of the RectTransform based on text length
    private void AdjustLayout(GameObject textObject, TMP_Text tmpText)
    {
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        float textHeight = tmpText.preferredHeight; // Get the preferred height based on the text
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, textHeight); // Set the height of the RectTransform

        LayoutRebuilder.ForceRebuildLayoutImmediate(Content.GetComponent<RectTransform>());
    }
    //Adding a gap for user to understand eachline clearly
    public void AddGap()
    {
        GameObject gap = Instantiate(TextPrefab, Content);
        TMP_Text gap_txt = gap.GetComponent<TMP_Text>();
        gap_txt.text = " ";
        _textEntries.Add(gap);
    }
    //Destroy all texts in the List
    public void ClearTexts()
    {
        // Clear all the text entries in the console
        foreach (var entry in _textEntries)
        {
            Destroy(entry);
        }
        _textEntries.Clear();
    }
}
