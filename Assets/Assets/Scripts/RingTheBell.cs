using System;
using UnityEngine;

public class RingTheBell : MonoBehaviour
{
    public static Action<RingTheBell> OnBellRung;

    [Header("Optional")]
    public AudioSource bellAudio;

    public void RingBell()
    {
        if (bellAudio != null)
            bellAudio.Play();

        OnBellRung?.Invoke(this);
    }
}