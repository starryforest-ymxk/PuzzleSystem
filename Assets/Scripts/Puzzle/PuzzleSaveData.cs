using System;
using System.Collections.Generic;

namespace PuzzleSystem.Puzzle
{
    [Serializable]
    public class PuzzleNodeSaveData
    {
        public string nodeGuid;
        public PuzzleNodeExecutionStatus status;
        public string customState;
    }

    [Serializable]
    public class SerializableVariable
    {
        public string key;
        public string value;
    }

    [Serializable]
    public class PuzzleSaveData
    {
        public string graphAssetName;
        public List<PuzzleNodeSaveData> nodes = new List<PuzzleNodeSaveData>();
        public List<SerializableVariable> variables = new List<SerializableVariable>();
        public List<string> triggeredConnections = new List<string>();
    }
}
