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

    // Priority: special -> after -> normal
    bool UseSpecial() => globalVariables.specialEventActive;      // set this elsewhere when special should play
    bool UseAfter()   => globalVariables.afterEventTriggered;     // set this elsewhere when after-dialogues should play

    public NPCDialogueSO GetCurrentDialogue()
    {
        if (UseSpecial() && dialoguesspecial != null && dialoguesspecial.Count > 0)
        {
            return dialoguesspecial[currentDialogueIndexspecial];
        }

        if (UseAfter() && dialoguesafter != null && dialoguesafter.Count > 0)
        {
            return dialoguesafter[currentDialogueIndexafter];
        }

        // fallback to normal dialogues
        if (dialogues != null && dialogues.Count > 0)
        {
            return dialogues[currentDialogueIndex];
        }

        return null;
    }

    public void AdvanceIndexForCurrentList()
    {
        if (UseSpecial() && dialoguesspecial != null && dialoguesspecial.Count > 0)
        {
            currentDialogueIndexspecial = (currentDialogueIndexspecial + 1) % dialoguesspecial.Count;
            return;
        }

        if (UseAfter() && dialoguesafter != null && dialoguesafter.Count > 0)
        {
            currentDialogueIndexafter = (currentDialogueIndexafter + 1) % dialoguesafter.Count;
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