using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using UnityEngine;

namespace PuzzleSystem.Puzzle
{
    [AddComponentMenu("Puzzle/Puzzle Graph Controller")]
    public class PuzzleGraphController : MonoBehaviour
    {
        [SerializeField]
        private PuzzleGraph puzzleGraphAsset;

        [SerializeField]
        private PuzzleEventBus eventBus;

        [SerializeField]
        private PuzzleCutscenePlayer cutscenePlayer;

        private PuzzleGraph runtimeGraph;
        private readonly Queue<BasePuzzleNode> executionQueue = new Queue<BasePuzzleNode>();
        private readonly Dictionary<string, object> variables = new Dictionary<string, object>();
        private readonly HashSet<string> triggeredConnections = new HashSet<string>();
        private readonly Dictionary<string, BasePuzzleNode> nodeLookup = new Dictionary<string, BasePuzzleNode>();

        public PuzzleCutscenePlayer CutscenePlayer => cutscenePlayer;

        public PuzzleGraph RuntimeGraph => runtimeGraph;

        private void Awake()
        {
            if (puzzleGraphAsset != null)
            {
                runtimeGraph = Instantiate(puzzleGraphAsset);
                runtimeGraph.Initialize(this, null);
                CacheNodes();
            }
        }

        private void OnEnable()
        {
            if (eventBus != null)
            {
                eventBus.RegisterGlobal(OnEventRaised);
            }
        }

        private void OnDisable()
        {
            if (eventBus != null)
            {
                eventBus.UnregisterGlobal(OnEventRaised);
            }
        }

        public void TriggerNode(BasePuzzleNode node, object payload = null)
        {
            if (node == null)
            {
                return;
            }

            executionQueue.Enqueue(node);
            ProcessQueue(payload);
        }

        public void SetVariable(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            variables[key] = value;
        }

        public bool TryGetVariable(string key, out object value)
        {
            return variables.TryGetValue(key, out value);
        }

        public PuzzleSaveData Save()
        {
            var data = new PuzzleSaveData
            {
                graphAssetName = puzzleGraphAsset != null ? puzzleGraphAsset.name : string.Empty,
                triggeredConnections = triggeredConnections.ToList()
            };

            foreach (var variable in variables)
            {
                data.variables.Add(new SerializableVariable
                {
                    key = variable.Key,
                    value = variable.Value != null ? variable.Value.ToString() : string.Empty
                });
            }

            if (runtimeGraph != null)
            {
                foreach (var node in runtimeGraph.allNodes.OfType<BasePuzzleNode>())
                {
                    data.nodes.Add(node.ToSaveData());
                }
            }

            return data;
        }

        public void Load(PuzzleSaveData data)
        {
            if (data == null || runtimeGraph == null)
            {
                return;
            }

            variables.Clear();
            foreach (var variable in data.variables)
            {
                variables[variable.key] = variable.value;
            }

            triggeredConnections.Clear();
            foreach (var connectionId in data.triggeredConnections)
            {
                triggeredConnections.Add(connectionId);
            }

            foreach (var node in runtimeGraph.allNodes.OfType<BasePuzzleNode>())
            {
                node.ResetNode();
            }

            foreach (var saved in data.nodes)
            {
                if (nodeLookup.TryGetValue(saved.nodeGuid, out var node))
                {
                    node.LoadFromSave(saved);
                }
            }
        }

        private void OnEventRaised(string eventKey, object payload)
        {
            if (runtimeGraph == null)
            {
                return;
            }

            foreach (var trigger in runtimeGraph.allNodes.OfType<IPuzzleTrigger>())
            {
                if (!string.IsNullOrEmpty(trigger.EventKey) && trigger.EventKey != eventKey)
                {
                    continue;
                }

                if (trigger is BasePuzzleNode node && trigger.ShouldTrigger(payload, this))
                {
                    TriggerNode(node, payload);
                }
            }
        }

        private void ProcessQueue(object payload)
        {
            while (executionQueue.Count > 0)
            {
                var current = executionQueue.Dequeue();
                var result = current.ExecuteNode(this, payload);
                foreach (var connection in result.Connections)
                {
                    triggeredConnections.Add(connection.UID);
                    if (connection.targetNode is BasePuzzleNode next)
                    {
                        executionQueue.Enqueue(next);
                    }
                }
            }
        }

        private void CacheNodes()
        {
            nodeLookup.Clear();
            if (runtimeGraph == null)
            {
                return;
            }

            foreach (var node in runtimeGraph.allNodes.OfType<BasePuzzleNode>())
            {
                nodeLookup[node.UID] = node;
            }
        }
    }
}
