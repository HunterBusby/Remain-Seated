using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class pauseMenuScript : MonoBehaviour
{
    [Header("Pause Input")]
    [Tooltip("The action used to pause and unpause the game.")]
    public InputActionReference pauseAction;

    [Header("Inputs To Disable While Paused")]
    [Tooltip("Drag in any gameplay actions here, such as Move, Jump, Interact, Attack, etc.")]
    public InputActionReference[] actionsToDisableWhilePaused;

    [Header("Pause Events")]
    [Tooltip("Called when the game is paused.")]
    public UnityEvent onPaused;

    [Tooltip("Called when the game is resumed.")]
    public UnityEvent onResumed;

    public static bool IsPaused { get; private set; }

    private void Awake()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }

    private void OnEnable()
    {
        if (pauseAction != null && pauseAction.action != null)
            pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        if (pauseAction != null && pauseAction.action != null)
            pauseAction.action.Disable();

        Time.timeScale = 1f;
        IsPaused = false;
    }

    private void Update()
    {
        if (pauseAction == null || pauseAction.action == null)
            return;

        if (pauseAction.action.WasPressedThisFrame())
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (IsPaused)
            return;

        IsPaused = true;
        Time.timeScale = 0f;

        SetGameplayInputsEnabled(false);

        onPaused?.Invoke();
    }

    public void ResumeGame()
    {
        if (!IsPaused)
            return;

        IsPaused = false;
        Time.timeScale = 1f;

        SetGameplayInputsEnabled(true);

        onResumed?.Invoke();
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetGameplayInputsEnabled(bool enabled)
    {
        if (actionsToDisableWhilePaused == null)
            return;

        foreach (InputActionReference actionRef in actionsToDisableWhilePaused)
        {
            if (actionRef == null || actionRef.action == null)
                continue;

            // Never disable the pause action itself
            if (pauseAction != null && actionRef.action == pauseAction.action)
                continue;

            if (enabled)
                actionRef.action.Enable();
            else
                actionRef.action.Disable();
        }
    }
}