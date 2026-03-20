using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using globalVariables = GlobalVariables; // Alias to avoid confusion with the class name

namespace RedstoneinventeGameStudio
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager instance;

        public TMP_Text title;
        public TMP_Text content;

        public float characterDelay = 0.1f;
        public float punctuationDelay = 0.5f;

        public int maxWords = 10;

        public bool moveNext;
        public GameObject moveNextButt;

        public Canvas dialogueCanvas;

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
            StartCoroutine(ShowDialogueC(nPCManager));
            globalVariables.istalking = true;

        }

        IEnumerator ShowDialogueC(NPCManager nPCManager)
        
{
    var current = nPCManager.GetCurrentDialogue();
    if (current == null)
    {
        dialogueCanvas.enabled = false;
        yield break;
    }

    title.text = current.title;
    content.text = "";

    dialogueCanvas.enabled = true;

    foreach (char character in current.lines)
    {
        content.text += character;
        yield return new WaitForSeconds(IsPunctuation(character) ? punctuationDelay : characterDelay);

        if (content.textInfo.wordCount >= maxWords && (character == ' ' || IsPunctuation(character) || character == ','))
        {
            moveNextButt.SetActive(true);
            yield return new WaitUntil(() => moveNext);

            content.text = "";
            moveNextButt.SetActive(false);
            moveNext = false;
        }
    }

    moveNextButt.SetActive(true);
    yield return new WaitUntil(() => moveNext);

    content.text = "";
    moveNextButt.SetActive(false);
    moveNext = false;

    dialogueCanvas.enabled = false;

    nPCManager.AdvanceIndexForCurrentList();
}
        

        bool IsPunctuation(char character)
        {
            return character == '.' || character == '?' || character == '!';
        }
    }

}