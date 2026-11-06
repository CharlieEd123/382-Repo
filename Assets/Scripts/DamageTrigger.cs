using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// LAB 5 / LAB 8 - Trigger volume. When the player walks into it, it subtracts damage from
/// the health FloatVariableSO, then raises the GameEventSO to announce "health changed".
/// It never refers to the UI or the sound. Whatever is listening reacts by itself.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DamageTrigger : MonoBehaviour
{
    [Tooltip("The PlayerHealth FloatVariableSO asset.")]
    [SerializeField] private FloatVariableSO currentHealth;

    [Tooltip("The OnHealthChanged GameEventSO asset. It is raised after the health changes.")]
    [SerializeField] private GameEventSO onHealthChanged;

    [Tooltip("How much health this zone removes each time the player enters.")]
    [SerializeField] private float damage = 10f;

    [Tooltip("The Tag the player object uses.")]
    [SerializeField] private string playerTag = "Player";

    // Optional: still available if you want to wire extra reactions in the Inspector.
    public UnityEvent onDamaged;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (currentHealth == null)
        {
            Debug.LogWarning($"{name}: No health FloatVariableSO assigned to DamageTrigger.");
            return;
        }

        currentHealth.value = Mathf.Max(0f, currentHealth.value - damage);   // 1) change the data

        if (onHealthChanged != null) onHealthChanged.Raise();                // 2) announce it
        onDamaged.Invoke();
    }
}
