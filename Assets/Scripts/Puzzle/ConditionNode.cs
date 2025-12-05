using System;
using NodeCanvas.Framework;
using UnityEngine;

namespace PuzzleSystem.Puzzle
{
    [Name("Condition")]
    [Category("Puzzle/Logic")]
    public class ConditionNode : BasePuzzleNode, IPuzzleTrigger
    {
        [SerializeField]
        [Tooltip("Event key from PuzzleEventBus that should wake this node.")]
        private string eventKey;

        [SerializeField]
        [Tooltip("Optional variable key read from PuzzleGraphController variable table.")]
        private string variableKey;

        [SerializeField]
        [Tooltip("Expected value for the variable check.")]
        private string expectedValue = "true";

        [SerializeField]
        [Tooltip("If enabled, the node will also execute when no event key is provided.")]
        private bool allowEmptyEvent;

        public string EventKey => eventKey;

        public bool ShouldTrigger(object eventPayload, PuzzleGraphController controller)
        {
            if (!allowEmptyEvent && string.IsNullOrEmpty(eventKey))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(variableKey) && controller != null)
            {
                if (controller.TryGetVariable(variableKey, out var value))
                {
                    return string.Equals(value?.ToString(), expectedValue, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            }

            return true;
        }

        protected override PuzzleNodeResult OnExecute(PuzzleGraphController controller, object eventData)
        {
            var passed = ShouldTrigger(eventData, controller);
            return passed ? PuzzleNodeResult.Success(outConnections) : PuzzleNodeResult.Failed;
        }

#if UNITY_EDITOR
        protected override void OnNodeInspectorGUI()
        {
            base.OnNodeInspectorGUI();
            eventKey = UnityEditor.EditorGUILayout.TextField("Event Key", eventKey);
            variableKey = UnityEditor.EditorGUILayout.TextField("Variable Key", variableKey);
            expectedValue = UnityEditor.EditorGUILayout.TextField("Expected", expectedValue);
            allowEmptyEvent = UnityEditor.EditorGUILayout.Toggle("Allow Empty Event", allowEmptyEvent);
        }
#endif
    }
}
