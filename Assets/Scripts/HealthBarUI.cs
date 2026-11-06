using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LAB 8 - Listens for a GameEventSO and, when it fires, (1) updates the UI Slider and
/// (2) plays a sound effect, both at exactly the same moment.
/// Put this on the Slider (or any object in the Canvas), next to an AudioSource.
/// This script never refers to whatever causes the damage.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [Header("Shared assets (drag from the Project window)")]
    [SerializeField] private FloatVariableSO currentHealth;
    [SerializeField] private FloatVariableSO maxHealth;
    [SerializeField] private GameEventSO onHealthChanged;

    [Header("Scene objects")]
    [SerializeField] private Slider slider;

    [Header("Sound")]
    [Tooltip("The AudioSource that plays the sound. Auto-found on this object if left empty.")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageSound;

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    // Subscribe while enabled (same pattern as Week 6's Input System events).
    private void OnEnable()
    {
        if (onHealthChanged != null) onHealthChanged.RegisterListener(UpdateSlider);
    }

    private void OnDisable()
    {
        if (onHealthChanged != null) onHealthChanged.UnregisterListener(UpdateSlider);
    }

    private void Start()
    {
        RefreshSlider(); // show the starting value when the game begins (no sound)
    }

    // Called by the GameEventSO the moment it is raised. Nobody calls this directly.
    private void UpdateSlider()
    {
        RefreshSlider();

        if (audioSource != null && damageSound != null)
            audioSource.PlayOneShot(damageSound);   // plays at the exact moment the event fires
    }

    private void RefreshSlider()
    {
        if (slider == null || currentHealth == null || maxHealth == null || maxHealth.value <= 0f) return;

        slider.value = Mathf.Clamp01(currentHealth.value / maxHealth.value);
    }
}
