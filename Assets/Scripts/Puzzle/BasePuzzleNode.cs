using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using UnityEngine;

namespace PuzzleSystem.Puzzle
{
    public enum PuzzleNodeExecutionStatus
    {
        Idle,
        Waiting,
        Running,
        Completed,
        Failed
    }

    [Serializable]
    public struct PuzzleNodeResult
    {
        public PuzzleNodeResult(bool succeeded, IEnumerable<Connection> connections)
        {
            Succeeded = succeeded;
            Connections = connections ?? Enumerable.Empty<Connection>();
        }

        public bool Succeeded { get; }

        public IEnumerable<Connection> Connections { get; }

        public static PuzzleNodeResult Success(IEnumerable<Connection> connections) => new PuzzleNodeResult(true, connections);

        public static PuzzleNodeResult Failed => new PuzzleNodeResult(false, Enumerable.Empty<Connection>());
    }

    public abstract class BasePuzzleNode : Node
    {
        [SerializeField]
        [Tooltip("Friendly name shown in inspectors and debug logs.")]
        private string displayName;

        public PuzzleNodeExecutionStatus ExecutionStatus { get; private set; } = PuzzleNodeExecutionStatus.Idle;

        public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;

        public override int maxInConnections => -1;

        public override int maxOutConnections => -1;

        public override Type outConnectionType => typeof(Connection);

        public override bool allowAsPrime => true;

        public override bool canSelfConnect => false;

        public override Alignment2x2 commentsAlignment => Alignment2x2.Bottom;

        public override Alignment2x2 iconAlignment => Alignment2x2.Default;

        public virtual bool CanExecute(PuzzleGraphController controller, object eventData)
        {
            return ExecutionStatus != PuzzleNodeExecutionStatus.Running;
        }

        public PuzzleNodeResult ExecuteNode(PuzzleGraphController controller, object eventData)
        {
            if (!CanExecute(controller, eventData))
            {
                return PuzzleNodeResult.Failed;
            }

            ExecutionStatus = PuzzleNodeExecutionStatus.Running;
            var result = OnExecute(controller, eventData);
            ExecutionStatus = result.Succeeded ? PuzzleNodeExecutionStatus.Completed : PuzzleNodeExecutionStatus.Failed;
            OnAfterExecute(controller, eventData, result);
            return result;
        }

        protected virtual PuzzleNodeResult OnExecute(PuzzleGraphController controller, object eventData)
        {
            return PuzzleNodeResult.Success(outConnections);
        }

        protected virtual void OnAfterExecute(PuzzleGraphController controller, object eventData, PuzzleNodeResult result)
        {
        }

        public virtual void ResetNode()
        {
            ExecutionStatus = PuzzleNodeExecutionStatus.Idle;
        }

        public virtual PuzzleNodeSaveData ToSaveData()
        {
            return new PuzzleNodeSaveData
            {
                nodeGuid = UID,
                status = ExecutionStatus,
                customState = SerializeCustomState()
            };
        }

        public virtual void LoadFromSave(PuzzleNodeSaveData data)
        {
            if (data == null)
            {
                return;
            }

            ExecutionStatus = data.status;
            DeserializeCustomState(data.customState);
        }

        protected virtual string SerializeCustomState()
        {
            return string.Empty;
        }

        protected virtual void DeserializeCustomState(string state)
        {
        }

#if UNITY_EDITOR
        protected override void OnNodeInspectorGUI()
        {
            base.OnNodeInspectorGUI();
            UnityEditor.EditorGUILayout.LabelField("Display Name", DisplayName);
            UnityEditor.EditorGUILayout.LabelField("Status", ExecutionStatus.ToString());
        }

        protected override void OnNodeGUI()
        {
            base.OnNodeGUI();
            if (!string.IsNullOrEmpty(DisplayName))
            {
                GUILayout.Label(DisplayName);
            }
        }
#endif
    }
}
