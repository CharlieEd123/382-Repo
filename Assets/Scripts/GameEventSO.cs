using System;
using UnityEngine;

/// <summary>
/// LAB 8 - A ScriptableObject that holds NO data. Its only job is to be Raised(),
/// which announces "something happened" to everyone who is listening.
/// Create instances with: Assets > Create > Game > Game Event (for example "OnHealthChanged").
///
/// The script that raises it and the scripts that listen to it never reference each other.
/// Both only reference this shared asset.
/// </summary>
[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Game/Game Event")]
public class GameEventSO : ScriptableObject
{
    private event Action onRaised;

    // Listeners hand over a method to call (like attackAction.performed += ... in Week 6).
    public void RegisterListener(Action listener)
    {
        onRaised += listener;
    }

    public void UnregisterListener(Action listener)
    {
        onRaised -= listener;
    }

    // Call this to announce "it happened". Every registered method runs.
    public void Raise()
    {
        onRaised?.Invoke();
    }
}
