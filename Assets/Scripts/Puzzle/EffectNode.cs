using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace PuzzleSystem.Puzzle
{
    [Name("Effect")]
    [Category("Puzzle/Logic")]
    public class EffectNode : BasePuzzleNode, IPuzzleEffect
    {
        [SerializeField]
        [TextArea]
        private string logMessage;

        [SerializeField]
        [Tooltip("Optional dialogue identifier for PuzzleCutscenePlayer.")]
        private string dialogueId;

        [SerializeField]
        [Tooltip("Optional cinematic/effect clip id for PuzzleCutscenePlayer.")]
        private string effectId;

        [SerializeField]
        [Tooltip("Variable key to write after the effect is played.")]
        private string resultVariable;

        [SerializeField]
        private string resultValue = "true";

        public void ApplyEffect(PuzzleGraphController controller)
        {
            if (!string.IsNullOrEmpty(logMessage))
            {
                Debug.Log($"[Puzzle Effect] {logMessage}");
            }

            controller?.CutscenePlayer?.PlayEffect(effectId);
            controller?.CutscenePlayer?.PlayDialogue(dialogueId);

            if (controller != null && !string.IsNullOrEmpty(resultVariable))
            {
                controller.SetVariable(resultVariable, resultValue);
            }
        }

        protected override PuzzleNodeResult OnExecute(PuzzleGraphController controller, object eventData)
        {
            ApplyEffect(controller);
            return PuzzleNodeResult.Success(outConnections);
        }

#if UNITY_EDITOR
        protected override void OnNodeInspectorGUI()
        {
            base.OnNodeInspectorGUI();
            logMessage = UnityEditor.EditorGUILayout.TextField("Log", logMessage);
            dialogueId = UnityEditor.EditorGUILayout.TextField("Dialogue Id", dialogueId);
            effectId = UnityEditor.EditorGUILayout.TextField("Effect Id", effectId);
            resultVariable = UnityEditor.EditorGUILayout.TextField("Result Variable", resultVariable);
            resultValue = UnityEditor.EditorGUILayout.TextField("Result Value", resultValue);
        }
#endif
    }
}
