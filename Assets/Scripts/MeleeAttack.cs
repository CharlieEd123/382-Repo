using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// LAB - Core interaction mechanic: a melee attack.
/// Press F, left mouse button, or gamepad West (X / Square) to attack.
/// Physics.OverlapSphere finds every collider inside the hitbox on the chosen LayerMask,
/// and the Console logs the name of each target hit.
/// OnDrawGizmos draws the hitbox in the Scene view so you can see and tune it.
/// </summary>
public class MeleeAttack : MonoBehaviour
{
    [Header("Hitbox")]
    [Tooltip("Empty child object placed in front of the character. The hitbox is centred on it. " +
             "If left empty, the hitbox sits in front of the character automatically.")]
    [SerializeField] private Transform attackPoint;
    [Tooltip("Radius of the hitbox sphere, in metres.")]
    [SerializeField] private float attackRadius = 1f;
    [Tooltip("Only objects on these layers can be hit (e.g. a 'Target' layer).")]
    [SerializeField] private LayerMask targetLayers;

    [Header("Timing")]
    [Tooltip("Seconds to wait before you can attack again.")]
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Debug Gizmo")]
    [SerializeField] private Color idleColor = new Color(1f, 1f, 0f, 0.8f);   // yellow
    [SerializeField] private Color hitColor = new Color(1f, 0f, 0f, 0.8f);    // red
    [Tooltip("How long the gizmo stays red after an attack.")]
    [SerializeField] private float flashTime = 0.2f;

    private float nextAttackTime;
    private float lastAttackTime = -999f;

    // Where the hitbox is centred. Falls back to 1m in front of the character at chest height.
    private Vector3 HitboxCenter
    {
        get
        {
            if (attackPoint != null) return attackPoint.position;
            return transform.position + transform.forward * 1f + Vector3.up * 1f;
        }
    }

    private void Update()
    {
        if (AttackPressed() && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private bool AttackPressed()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;
        Gamepad pad = Gamepad.current;

        return (kb != null && kb.fKey.wasPressedThisFrame)
            || (mouse != null && mouse.leftButton.wasPressedThisFrame)
            || (pad != null && pad.buttonWest.wasPressedThisFrame);
    }

    private void Attack()
    {
        lastAttackTime = Time.time;

        // Everything whose collider overlaps the sphere AND is on the target layers
        Collider[] hits = Physics.OverlapSphere(HitboxCenter, attackRadius, targetLayers);

        if (hits.Length == 0)
        {
            Debug.Log("Attack hit nothing.");
            return;
        }

        foreach (Collider hit in hits)
        {
            Debug.Log("Hit: " + hit.name);

            HitFlash flash = hit.GetComponent<HitFlash>();
            if (flash != null) flash.Flash();
        }
    }

    // Draws in the Scene view (and Game view if Gizmos is on), even when nothing is selected.
    private void OnDrawGizmos()
    {
        bool justAttacked = Application.isPlaying && Time.time - lastAttackTime < flashTime;
        Gizmos.color = justAttacked ? hitColor : idleColor;
        Gizmos.DrawWireSphere(HitboxCenter, attackRadius);
    }
}
