using UnityEngine;

namespace PuzzleSystem.Puzzle
{
    public class PuzzleCutscenePlayer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Optional bridge to a dialogue system or timeline.")]
        private MonoBehaviour dialogueController;

        public void PlayDialogue(string dialogueId)
        {
            if (string.IsNullOrEmpty(dialogueId))
            {
                return;
            }

            Debug.Log($"[Puzzle Cutscene] Dialogue: {dialogueId}", this);
            var receiver = dialogueController as IPuzzleDialogueAdapter;
            receiver?.PlayDialogue(dialogueId);
        }

        public void PlayEffect(string effectId)
        {
            if (string.IsNullOrEmpty(effectId))
            {
                return;
            }

            Debug.Log($"[Puzzle Cutscene] Effect: {effectId}", this);
            var receiver = dialogueController as IPuzzleDialogueAdapter;
            receiver?.PlayEffect(effectId);
        }
    }

    public interface IPuzzleDialogueAdapter
    {
        void PlayDialogue(string dialogueId);

        void PlayEffect(string effectId);
    }
}
