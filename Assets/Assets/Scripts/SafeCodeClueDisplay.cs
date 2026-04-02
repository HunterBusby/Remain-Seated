using TMPro;
using UnityEngine;

public class SafeCodeClueDisplay : MonoBehaviour
{
    [SerializeField] private SafeLockController safeLock;
    [SerializeField] private TMP_Text clueText;
    [SerializeField] private string prefix = "Code: ";

    private void Start()
    {
        RefreshClue();
    }

    public void RefreshClue()
    {
        if (safeLock == null || clueText == null)
        {
            Debug.LogWarning("SafeLockController or clueText is missing.");
            return;
        }

        clueText.text = prefix + safeLock.GeneratedCode;
    }
}