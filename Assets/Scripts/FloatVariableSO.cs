using UnityEngine;

/// <summary>
/// LAB 8 - A ScriptableObject that holds ONE number and nothing else.
/// Create instances with: Assets > Create > Game > Float Variable
/// (for example "PlayerHealth" and "PlayerMaxHealth").
/// Any script can read or change .value. It's the "value holder" half of Lab 5's HealthSO.
/// </summary>
[CreateAssetMenu(fileName = "NewFloatVariable", menuName = "Game/Float Variable")]
public class FloatVariableSO : ScriptableObject
{
    [Tooltip("The value this variable starts every play session with. Edit this one.")]
    [SerializeField] private float initialValue = 100f;

    [Tooltip("The live value while the game runs (it resets to Initial Value when you press Play).")]
    public float value;

    // Assets keep changes made in Play mode, so reset to the starting value each time the game loads.
    private void OnEnable()
    {
        value = initialValue;
    }
}
