using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using globalVariables = GlobalVariables; // Alias to avoid confusion with the class name

namespace RedstoneinventeGameStudio
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager instance;

        public TMP_Text title;
        public TMP_Text content;

        public float characterDelay = 0.05f;      // default per-character delay
        public float punctuationDelay = 0.25f;    // longer delay for punctuation

        public int maxWords = 10;

        public bool moveNext;
        public GameObject moveNextButt;

        public Canvas dialogueCanvas;

        [HideInInspector] public bool isShowing;

        private void Awake()
        {
            instance = this;
        }

        public void MoveNext()
        {
            moveNext = true;
            globalVariables.istalking = false;
        }

        public void ShowDialogue(NPCManager nPCManager)
        {
            if (isShowing) return;

            // Mark active and enable canvas immediately so Update() won't flag it
            isShowing = true;
            globalVariables.istalking = true;
            if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(true);

            StartCoroutine(ShowDialogueC(nPCManager));
        }

        IEnumerator ShowDialogueC(NPCManager nPCManager)
        {
            Debug.Log("ShowDialogueC start");
            if (nPCManager == null)
            {
                if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(false);
                isShowing = false;
                globalVariables.istalking = false;
                Debug.Log("ShowDialogueC end (null NPCManager)");
                yield break;
            }

            int? startIndex = null;
            try
            {
                var idxProp = nPCManager.GetType().GetProperty("currentIndex");
                if (idxProp != null)
                    startIndex = (int?)idxProp.GetValue(nPCManager);
            }
            catch { startIndex = null; }

            var first = nPCManager.GetCurrentDialogue();
            string firstKey = first != null ? (first.title + "|" + (first.lines ?? "")) : null;

            while (true)
            {
                var current = nPCManager.GetCurrentDialogue();
                Debug.Log("Current dialogue: " + (current == null ? "null" : current.title));
                if (current == null)
                {
                    if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(false);
                    break;
                }

                if (title != null) title.text = current.title ?? "";
                if (content != null) content.text = "";

                // Canvas already enabled by ShowDialogue; do not enable here to avoid race conditions
                if (moveNextButt != null) moveNextButt.SetActive(false);

                bool lineFullyShown = false;
                string lineText = current.lines ?? "";

                for (int i = 0; i < lineText.Length; i++)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (content != null) content.text = lineText;
                        lineFullyShown = true;
                        break;
                    }

                    if (content != null) content.text += lineText[i];

                    char ch = lineText[i];
                    float wait = IsPunctuation(ch) ? punctuationDelay : characterDelay;

                    float t = 0f;
                    while (t < wait)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (content != null) content.text = lineText;
                            lineFullyShown = true;
                            break;
                        }
                        t += Time.deltaTime;
                        yield return null;
                    }

                    if (lineFullyShown) break;
                }

                if (moveNextButt != null) moveNextButt.SetActive(true);

                bool advanced = false;
                moveNext = false;
                while (!advanced)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        advanced = true;
                        break;
                    }
                    if (moveNext)
                    {
                        advanced = true;
                        break;
                    }
                    yield return null;
                }

                moveNext = false;
                if (moveNextButt != null) moveNextButt.SetActive(false);
                if (content != null) content.text = "";

                try
                {
                    nPCManager.AdvanceIndexForCurrentList();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("AdvanceIndexForCurrentList threw: " + e);
                    break;
                }

                Debug.Log("Advanced. Next dialogue: " + (nPCManager.GetCurrentDialogue() == null ? "null" : nPCManager.GetCurrentDialogue().title));
                yield return null;

                var next = nPCManager.GetCurrentDialogue();
                if (next == null)
                {
                    if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(false);
                    break;
                }

                if (startIndex.HasValue)
                {
                    int? curIndex = null;
                    try
                    {
                        var idxProp = nPCManager.GetType().GetProperty("currentIndex");
                        if (idxProp != null)
                            curIndex = (int?)idxProp.GetValue(nPCManager);
                    }
                    catch { curIndex = null; }

                    if (curIndex.HasValue && curIndex.Value == startIndex.Value)
                    {
                        Debug.Log("Wrapped to start index, breaking.");
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(firstKey))
                {
                    string nextKey = (next.title ?? "") + "|" + (next.lines ?? "");
                    if (nextKey == firstKey)
                    {
                        Debug.Log("Next equals firstKey, breaking.");
                        break;
                    }
                }
            }

            // Ensure canvas is hidden no matter how we exit
            if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(false);

            // additional small safeguard: re-hide after a frame
            yield return null;
            if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(false);

            isShowing = false;
            globalVariables.istalking = false;
            Debug.Log("ShowDialogueC end");
            yield break;
        }

        bool IsPunctuation(char character)
        {
            return character == '.' || character == '?' || character == '!';
        }

        // Optional helper for debugging: detects unexpected re-enables at runtime
        private void Update()
        {
            if (dialogueCanvas != null && dialogueCanvas.gameObject.activeSelf && !isShowing)
            {
                Debug.LogWarning("Dialogue canvas active while not isShowing", dialogueCanvas.gameObject);
            }
        }
    }
}
