using System.Collections.Generic;
using UnityEngine;
using globalVariables = GlobalVariables; 

namespace RedstoneinventeGameStudio
{
    public class NPCManager : MonoBehaviour
    {
        public List<NPCDialogueSO> dialogues;
        public int currentDialogueIndex = 0;

        public void MoveNext()
        {
            currentDialogueIndex++;
            if (currentDialogueIndex >= dialogues.Count)
            {
                currentDialogueIndex = 0;
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