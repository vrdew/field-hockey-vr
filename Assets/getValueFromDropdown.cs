using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class getValueFromDropdown : MonoBehaviour
{
    [SerializeField] public TMP_Dropdown dropdown; // Reference to the TMP_Dropdown component
    [SerializeField] public TextMeshProUGUI displayText; // Reference to the TextMeshProUGUI component to display the selected value

    void Start()
    {
        // Subscribe to the dropdown's onValueChanged event
        dropdown.onValueChanged.AddListener(delegate { getDropdownValue(); });

        // Initialize the display text with the default value
        getDropdownValue();
    }

    public void getDropdownValue()
    {
        // Get the index of the selected option
        int pickedEntryIndex = dropdown.value;

        // Get the text of the selected option
        string selectedText = dropdown.options[pickedEntryIndex].text;

        // Display the selected text
        displayText.text = selectedText+ " Results";

        // Log the selected index (optional)
        Debug.Log(pickedEntryIndex);
    }
}
