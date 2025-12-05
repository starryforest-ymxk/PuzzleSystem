using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace PuzzleSystem.Puzzle
{
    [GraphInfo(
        packageName = "PuzzleGraph",
        description = "Custom puzzle orchestration graph.",
        title = "Puzzle Graph")]
    public class PuzzleGraph : Graph
    {
        public IEnumerable<BasePuzzleNode> EntryNodes => allNodes.OfType<BasePuzzleNode>().Where(node => node.inConnections.Count == 0);

        public override Type baseNodeType => typeof(BasePuzzleNode);

        public override bool requiresAgent => false;

        public override bool requiresPrimeNode => false;

        public override bool isTree => false;

        public override bool allowBlackboardOverrides => true;

        public override bool canAcceptVariableDrops => true;

        public override PlanarDirection flowDirection => PlanarDirection.Horizontal;

        protected override void OnGraphStarted()
        {
            foreach (var node in allNodes.OfType<BasePuzzleNode>())
            {
                node.ResetNode();
            }
        }
    }
}
