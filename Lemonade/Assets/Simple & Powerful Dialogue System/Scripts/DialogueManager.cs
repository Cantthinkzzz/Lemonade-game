using System.Collections;
using TMPro;
using UnityEngine;
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

        private Coroutine typingCoroutine;

        private void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// Plays all lines of the NPC's dialogue once, stopping after completing one full cycle.
        /// </summary>
        public void ShowDialogue(NPCManager npc)
        {
            if (globalVariables.istalking) return;
            StartCoroutine(PlayDialogueOnce(npc));
        }

        private IEnumerator PlayDialogueOnce(NPCManager npc)
        {
            globalVariables.istalking = true;
            dialogueCanvas.gameObject.SetActive(true);

            var dialogue = npc.GetCurrentDialogue();
            if (dialogue == null || string.IsNullOrEmpty(dialogue.lines))
            {
                EndDialogue();
                yield break;
            }

            title.text = dialogue.title ?? "";

            // Split lines by newline or pipe
            string[] lines = dialogue.lines.Split(new char[] { '\n', '|' });
            int lineCount = lines.Length;
            int currentIndex = 0;

            // Play each line once
            while (currentIndex < lineCount)
            {
                string line = lines[currentIndex];
                content.text = "";

                // Start typing coroutine
                typingCoroutine = StartCoroutine(TypeLine(line));

                bool lineCompleted = false;

                // Wait until line finishes typing OR player clicks
                while (!lineCompleted)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        StopCoroutine(typingCoroutine);
                        content.text = line; // skip typing
                        lineCompleted = true;
                    }

                    if (content.text == line)
                        lineCompleted = true;

                    yield return null;
                }

                // Wait for click to advance to next line
                bool clickedToAdvance = false;
                while (!clickedToAdvance)
                {
                    if (Input.GetMouseButtonDown(0))
                        clickedToAdvance = true;
                    yield return null;
                }

                currentIndex++;
                npc.Advance();
            }

            // Reached the first line again → stop
            EndDialogue();
        }

        private IEnumerator TypeLine(string line)
        {
            content.text = "";
            for (int i = 0; i < line.Length; i++)
            {
                content.text += line[i];
                float delay = (line[i] == '.' || line[i] == '?' || line[i] == '!') ? punctuationDelay : characterDelay;
                yield return new WaitForSeconds(delay);
            }
        }

        private void EndDialogue()
        {
            dialogueCanvas.gameObject.SetActive(false);
            globalVariables.istalking = false;
        }
    }
}