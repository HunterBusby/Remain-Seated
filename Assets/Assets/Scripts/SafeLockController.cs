using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SafeLockController : MonoBehaviour
{
    [Header("Code Settings")]
    [SerializeField] private int codeLength = 5;
    [SerializeField] private bool allowRepeatedDigits = true;
    [SerializeField] private List<int> availableDigits = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    [Header("Display")]
    [SerializeField] private TMP_Text inputDisplay;
    [SerializeField] private bool hideInputWithAsterisks = false;

    [Header("Events")]
    [SerializeField] private UnityEvent onCorrectCode;
    [SerializeField] private UnityEvent onWrongCode;

    private string generatedCode = "";
    private string currentInput = "";
    private bool isUnlocked = false;

    public string GeneratedCode => generatedCode;
    public bool IsUnlocked => isUnlocked;

    private void Awake()
    {
        GenerateNewCode();
        UpdateInputDisplay();
    }

    public void GenerateNewCode()
    {
        generatedCode = CreateRandomCode();
        currentInput = "";
        isUnlocked = false;

        Debug.Log("Safe Code Generated: " + generatedCode);
        UpdateInputDisplay();
    }

    private string CreateRandomCode()
    {
        if (availableDigits == null || availableDigits.Count == 0)
        {
            Debug.LogError("No available digits assigned for the safe code.");
            return "";
        }

        if (!allowRepeatedDigits && availableDigits.Count < codeLength)
        {
            Debug.LogError("Not enough unique digits to create this code length.");
            return "";
        }

        StringBuilder sb = new StringBuilder();
        List<int> tempDigits = new List<int>(availableDigits);

        for (int i = 0; i < codeLength; i++)
        {
            if (allowRepeatedDigits)
            {
                int randomIndex = Random.Range(0, availableDigits.Count);
                sb.Append(availableDigits[randomIndex]);
            }
            else
            {
                int randomIndex = Random.Range(0, tempDigits.Count);
                sb.Append(tempDigits[randomIndex]);
                tempDigits.RemoveAt(randomIndex);
            }
        }

        return sb.ToString();
    }

    public void PressDigit(int digit)
    {
        if (isUnlocked)
            return;

        if (!availableDigits.Contains(digit))
        {
            Debug.LogWarning($"Digit {digit} is not allowed.");
            return;
        }

        if (currentInput.Length >= codeLength)
            return;

        currentInput += digit.ToString();
        UpdateInputDisplay();
    }

    public void ClearInput()
    {
        if (isUnlocked)
            return;

        currentInput = "";
        UpdateInputDisplay();
    }

    public void EnterCode()
    {
        if (isUnlocked)
            return;

        if (currentInput == generatedCode)
        {
            isUnlocked = true;
            Debug.Log("Correct code entered.");
            onCorrectCode?.Invoke();
        }
        else
        {
            Debug.Log("Wrong code entered.");
            onWrongCode?.Invoke();
        }
    }

    private void UpdateInputDisplay()
    {
        if (inputDisplay == null)
            return;

        if (string.IsNullOrEmpty(currentInput))
        {
            inputDisplay.text = "";
            return;
        }

        inputDisplay.text = hideInputWithAsterisks
            ? new string('*', currentInput.Length)
            : currentInput;
    }
}