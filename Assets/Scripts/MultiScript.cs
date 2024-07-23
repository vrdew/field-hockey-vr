using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class MultiSelectManager : MonoBehaviour
{
    public Button[] selectableButtons = new Button[9];
    public Button submitButton;
    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;

    private Dictionary<string, bool> selectedOptions = new Dictionary<string, bool>();
    private const string JSON_FILE_NAME = "selected_targets.json";

    void Start()
    {
        InitializeButtons();
        SetupSubmitButton();
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < selectableButtons.Length; i++)
        {
            Button button = selectableButtons[i];

            if (button != null)
            {
                string buttonName = button.gameObject.name;
                selectedOptions[buttonName] = false;

                void OnClickHandler()
                {
                    OnButtonClicked(buttonName, button);
                }
                button.onClick.AddListener(OnClickHandler);

                SetButtonColor(button, defaultColor);
            }
            else
            {
                Debug.LogWarning($"Button not assigned for index {i}");
            }
        }
    }

    private void SetupSubmitButton()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmit);
        }
        else
        {
            Debug.LogWarning("Submit button not assigned!");
        }
    }

    private void OnButtonClicked(string buttonName, Button clickedButton)
    {
        bool isSelected = !selectedOptions[buttonName];
        selectedOptions[buttonName] = isSelected;

        SetButtonColor(clickedButton, isSelected ? selectedColor : defaultColor);

        Debug.Log($"Option '{buttonName}' is now {(isSelected ? "selected" : "deselected")}");
    }

    private void SetButtonColor(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.selectedColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        button.colors = colors;
    }

    private void OnSubmit()
    {
        List<string> selectedOptionsList = new List<string>();
        foreach (var option in selectedOptions)
        {
            if (option.Value)
            {
                selectedOptionsList.Add(option.Key);
            }
        }

        SaveToJsonFile(selectedOptionsList);

        Debug.Log("Selected options: " + string.Join(", ", selectedOptionsList));

        SceneManager.LoadScene("MainScene");
    }

    private void SaveToJsonFile(List<string> selectedOptions)
    {
        SelectedOptions data = new SelectedOptions { options = selectedOptions };
        string json = JsonUtility.ToJson(data);
        string filePath = Path.Combine(Application.persistentDataPath, JSON_FILE_NAME);
        File.WriteAllText(filePath, json);
        Debug.Log($"Selected options saved to {filePath}");
    }

    [System.Serializable]
    private class SelectedOptions
    {
        public List<string> options;
    }
}