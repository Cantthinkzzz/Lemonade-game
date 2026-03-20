using System.Collections.Generic;
using UnityEngine;
using globalVariables = GlobalVariables;

namespace RedstoneinventeGameStudio
{
    public class NPCManager : MonoBehaviour
    {
        public List<NPCDialogueSO> dialogues;
        public List<NPCDialogueSO> dialoguesspecial;
        public List<NPCDialogueSO> dialoguesafter;

        public int currentDialogueIndex = 0;
        public int currentDialogueIndexspecial = 0;
        public int currentDialogueIndexafter = 0;

        [Header("Runtime flags")]
        public bool specialActiveForThisNPC = false;
        public bool afterActiveForThisNPC = false;

        // Priority: special -> after -> normal
        bool UseSpecial() => specialActiveForThisNPC || globalVariables.specialEventActive;
        bool UseAfter() => afterActiveForThisNPC || globalVariables.afterEventTriggered;

        // Public method to enable special dialogue for this NPC (call from other scripts)
        public void EnableSpecialForThisNPC()
        {
            specialActiveForThisNPC = true;
        }

        // Public method to enable after-dialogue for this NPC
        public void EnableAfterForThisNPC()
        {
            afterActiveForThisNPC = true;
        }

        public NPCDialogueSO GetCurrentDialogue()
        {
            if (UseSpecial() && dialoguesspecial != null && dialoguesspecial.Count > 0)
            {
                currentDialogueIndexspecial = Mathf.Clamp(currentDialogueIndexspecial, 0, Mathf.Max(0, dialoguesspecial.Count - 1));
                return dialoguesspecial[currentDialogueIndexspecial];
            }

            if (UseAfter() && dialoguesafter != null && dialoguesafter.Count > 0)
            {
                currentDialogueIndexafter = Mathf.Clamp(currentDialogueIndexafter, 0, Mathf.Max(0, dialoguesafter.Count - 1));
                return dialoguesafter[currentDialogueIndexafter];
            }

            if (dialogues != null && dialogues.Count > 0)
            {
                currentDialogueIndex = Mathf.Clamp(currentDialogueIndex, 0, Mathf.Max(0, dialogues.Count - 1));
                return dialogues[currentDialogueIndex];
            }

            return null;
        }

        public void AdvanceIndexForCurrentList()
        {
            if (UseSpecial() && dialoguesspecial != null && dialoguesspecial.Count > 0)
            {
                currentDialogueIndexspecial = (currentDialogueIndexspecial + 1) % dialoguesspecial.Count;

                // If you want special to be one-shot, disable after exhausting:
                if (currentDialogueIndexspecial == 0)
                    specialActiveForThisNPC = false;

                return;
            }

            if (UseAfter() && dialoguesafter != null && dialoguesafter.Count > 0)
            {
                currentDialogueIndexafter = (currentDialogueIndexafter + 1) % dialoguesafter.Count;

                if (currentDialogueIndexafter == 0)
                    afterActiveForThisNPC = false;

                return;
            }

            if (dialogues != null && dialogues.Count > 0)
            {
                currentDialogueIndex = (currentDialogueIndex + 1) % dialogues.Count;
            }
        }

        private void OnMouseUp()
        {
            if (globalVariables.istalking == true)
            {
                return;
            }

            DialogueManager.instance.ShowDialogue(this);
        }
    }
}
