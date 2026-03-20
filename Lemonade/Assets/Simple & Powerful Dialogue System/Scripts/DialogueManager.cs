using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using globalVariables = GlobalVariables;

namespace RedstoneinventeGameStudio
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager instance;

        public TMP_Text title;
        public TMP_Text content;

        public float characterDelay = 0.05f;
        public float punctuationDelay = 0.25f;

        public Canvas dialogueCanvas;

        private void Awake()
        {
            instance = this;
        }

        public void ShowDialogue(NPCManager npc)
        {
            if (globalVariables.istalking) return;

            StartCoroutine(ShowLine(npc));
        }

        IEnumerator ShowLine(NPCManager npc)
        {
            globalVariables.istalking = true;

            dialogueCanvas.gameObject.SetActive(true);

            var dialogue = npc.GetCurrentDialogue();
            if (dialogue == null)
            {
                EndDialogue();
                yield break;
            }

            title.text = dialogue.title ?? "";
            content.text = "";

            string line = dialogue.lines ?? "";

            // typing effect
            for (int i = 0; i < line.Length; i++)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    content.text = line;
                    break;
                }

                content.text += line[i];

                yield return new WaitForSeconds(
                    (line[i] == '.' || line[i] == '?' || line[i] == '!')
                    ? punctuationDelay
                    : characterDelay
                );
            }

            // wait for click
            while (!Input.GetMouseButtonDown(0))
                yield return null;

            npc.Advance();

            EndDialogue();
        }

        void EndDialogue()
        {
            dialogueCanvas.gameObject.SetActive(false);
            globalVariables.istalking = false;
        }
    }
}