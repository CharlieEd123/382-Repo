using UnityEngine;
using TMPro;

/// <summary>
/// LAB 5 - The simple, visible reaction: shows the HealthSO value as UI text.
/// Wire DamageTrigger's UnityEvent to Refresh() in the Inspector.
/// </summary>
public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private HealthSO health;
    [SerializeField] private TMP_Text label;

    private void Start()
    {
        Refresh(); // show the starting value when the game begins
    }

    // Public so the UnityEvent can call it from the Inspector.
    public void Refresh()
    {
        label.text = "Health: " + health.currentHealth;
    }
}
