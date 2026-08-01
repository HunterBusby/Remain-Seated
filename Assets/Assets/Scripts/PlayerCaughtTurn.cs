using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCaughtTurn : MonoBehaviour
{
    [Header("Turn Settings")]
    [Tooltip("The object that should rotate, usually the player root.")]
    public Transform objectToRotate;

    [Tooltip("The creature the player should turn toward.")]
    public Transform creature;

    [Tooltip("How long the forced turn takes.")]
    public float turnDuration = 0.5f;

    [Tooltip("Invoked after the player finishes turning.")]
    public UnityEvent onTurnFinished;

    private bool isTurning;

    private void Awake()
    {
        if (objectToRotate == null)
        {
            objectToRotate = transform;
        }
    }

    public void TurnTowardCreature()
    {
        if (creature == null || isTurning)
        {
            return;
        }

        StartCoroutine(TurnCoroutine());
    }

    private IEnumerator TurnCoroutine()
    {
        isTurning = true;

        Quaternion startingRotation = objectToRotate.rotation;

        Vector3 directionToCreature =
            creature.position - objectToRotate.position;

        // Prevent the player from looking upward or downward.
        directionToCreature.y = 0f;

        if (directionToCreature.sqrMagnitude <= 0.001f)
        {
            isTurning = false;
            yield break;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(directionToCreature);

        float elapsedTime = 0f;

        while (elapsedTime < turnDuration)
        {
            elapsedTime += Time.deltaTime;

            float percentage =
                Mathf.Clamp01(elapsedTime / turnDuration);

            objectToRotate.rotation = Quaternion.Slerp(
                startingRotation,
                targetRotation,
                percentage);

            yield return null;
        }

        objectToRotate.rotation = targetRotation;
        isTurning = false;

        onTurnFinished?.Invoke();
    }
}