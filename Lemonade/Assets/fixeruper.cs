using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class EnsureCanvasInitialized : MonoBehaviour
{
    public Canvas targetCanvas; // optional override; will use attached Canvas if null

    void Awake()
    {
        if (targetCanvas == null)
            targetCanvas = GetComponent<Canvas>();

        if (targetCanvas == null)
            return;

        // Ensure canvas is active so any internal UI setup runs, then disable next frame
        targetCanvas.gameObject.SetActive(true);
        StartCoroutine(DisableNextFrame());
    }

    System.Collections.IEnumerator DisableNextFrame()
    {
        yield return null; // wait one frame
        if (targetCanvas != null)
            targetCanvas.gameObject.SetActive(false);
    }
}
