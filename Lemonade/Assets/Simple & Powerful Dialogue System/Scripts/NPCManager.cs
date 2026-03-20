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

        private int indexBefore = 0;
        private int indexSpecial = 0;
        private int indexAfter = 0;

        public enum State
        {
            Before,
            Special,
            After
        }

        public State currentState = State.Before;

        public void EnableSpecial()
        {
            currentState = State.Special;
            indexSpecial = 0;
        }

        public NPCDialogueSO GetCurrentDialogue()
        {
            switch (currentState)
            {
                case State.Special:
                    return dialoguesspecial.Count > 0
                        ? dialoguesspecial[Mathf.Clamp(indexSpecial, 0, dialoguesspecial.Count - 1)]
                        : null;

                case State.After:
                    return dialoguesafter.Count > 0
                        ? dialoguesafter[Mathf.Clamp(indexAfter, 0, dialoguesafter.Count - 1)]
                        : null;

                default:
                    return dialogues.Count > 0
                        ? dialogues[Mathf.Clamp(indexBefore, 0, dialogues.Count - 1)]
                        : null;
            }
        }

        public void Advance()
        {
            switch (currentState)
            {
                case State.Special:
                    indexSpecial++;

                    if (indexSpecial >= dialoguesspecial.Count)
                    {
                        currentState = State.After;
                        indexAfter = 0;
                    }
                    break;

                case State.After:
                    if (indexAfter < dialoguesafter.Count - 1)
                        indexAfter++;
                    break;

                case State.Before:
                    indexBefore = (indexBefore + 1) % dialogues.Count;
                    break;
            }
        }

        private void OnMouseUp()
        {
            if (globalVariables.istalking) return;
            DialogueManager.instance.ShowDialogue(this);
        }
    }
}