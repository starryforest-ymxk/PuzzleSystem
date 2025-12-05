using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using UnityEngine;

namespace PuzzleSystem.Puzzle
{
    [Name("Branch")]
    [Category("Puzzle/Logic")]
    public class BranchNode : BasePuzzleNode
    {
        [Serializable]
        private class BranchOption
        {
            public string description;
            public string variableKey;
            public string expectedValue = "true";
            public int connectionIndex;
        }

        [SerializeField]
        [Tooltip("Branch conditions checked in order. Connection index uses the node's outgoing link order.")]
        private List<BranchOption> branches = new List<BranchOption>();

        public override int maxOutConnections => -1;

        protected override PuzzleNodeResult OnExecute(PuzzleGraphController controller, object eventData)
        {
            var selected = GetNextConnection(controller);
            if (selected == null)
            {
                return PuzzleNodeResult.Failed;
            }

            return PuzzleNodeResult.Success(new[] { selected });
        }

        private Connection GetNextConnection(PuzzleGraphController controller)
        {
            foreach (var branch in branches)
            {
                if (branch.connectionIndex < 0 || branch.connectionIndex >= outConnections.Count)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(branch.variableKey))
                {
                    return outConnections[branch.connectionIndex];
                }

                if (controller != null && controller.TryGetVariable(branch.variableKey, out var value))
                {
                    if (string.Equals(value?.ToString(), branch.expectedValue, StringComparison.OrdinalIgnoreCase))
                    {
                        return outConnections[branch.connectionIndex];
                    }
                }
            }

            return outConnections.FirstOrDefault();
        }

#if UNITY_EDITOR
        protected override void OnConnectionInspectorGUI(Connection connection)
        {
            base.OnConnectionInspectorGUI(connection);
            var index = outConnections.IndexOf(connection);
            var branch = branches.FirstOrDefault(b => b.connectionIndex == index);
            var label = branch != null ? branch.description : $"Branch {index}";
            UnityEditor.EditorGUILayout.LabelField("Label", label);
        }

        protected override void OnNodeInspectorGUI()
        {
            base.OnNodeInspectorGUI();
            for (var i = 0; i < branches.Count; i++)
            {
                var branch = branches[i];
                UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                branch.description = UnityEditor.EditorGUILayout.TextField($"Description {i}", branch.description);
                branch.variableKey = UnityEditor.EditorGUILayout.TextField("Variable", branch.variableKey);
                branch.expectedValue = UnityEditor.EditorGUILayout.TextField("Expected", branch.expectedValue);
                branch.connectionIndex = UnityEditor.EditorGUILayout.IntField("Connection Index", branch.connectionIndex);
                UnityEditor.EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Branch"))
            {
                branches.Add(new BranchOption { connectionIndex = branches.Count });
            }
        }
#endif
    }
}
