using System.Collections;
using UnityEngine;

/// <summary>
/// LAB - Visual feedback. Put this on a TARGET (e.g. a cube on your 'Target'/'Enemy' layer)
/// that uses a material made from the HitFlash Shader Graph.
/// Calling Flash() makes the target flash and then fade back, by smoothly
/// changing the shader's _FlashAmount float from C#.
/// </summary>
public class HitFlash : MonoBehaviour
{
    [Tooltip("The renderer that uses the HitFlash material. Auto-found if left empty.")]
    [SerializeField] private Renderer targetRenderer;

    [Tooltip("The Reference name of the Float property in the Shader Graph.")]
    [SerializeField] private string flashProperty = "_FlashAmount";

    [Tooltip("Seconds to go from normal to full flash (small = snappy).")]
    [SerializeField] private float flashUpTime = 0.05f;

    [Tooltip("Seconds to fade from full flash back to normal.")]
    [SerializeField] private float flashDownTime = 0.25f;

    private Material material;
    private int flashId;
    private Coroutine running;

    private void Awake()
    {
        if (targetRenderer == null) targetRenderer = GetComponentInChildren<Renderer>();

        // .material gives THIS object its own copy, so flashing one target doesn't flash them all.
        material = targetRenderer.material;
        flashId = Shader.PropertyToID(flashProperty);
        material.SetFloat(flashId, 0f);
    }

    // Call this when the target is hit (from MeleeAttack, a UnityEvent, a trigger...).
    public void Flash()
    {
        if (running != null) StopCoroutine(running); // restart cleanly if hit again mid-flash
        running = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        yield return Fade(0f, 1f, flashUpTime);   // quick flash up
        yield return Fade(1f, 0f, flashDownTime); // smooth fade back down
        running = null;
    }

    // Smoothly moves the shader float from 'from' to 'to' over 'duration' seconds.
    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            material.SetFloat(flashId, Mathf.Lerp(from, to, t / duration));
            yield return null; // wait one frame
        }
        material.SetFloat(flashId, to);
    }
}
