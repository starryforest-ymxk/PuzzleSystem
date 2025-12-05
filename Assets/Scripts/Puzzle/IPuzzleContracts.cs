using System;

namespace PuzzleSystem.Puzzle
{
    public interface IPuzzleTrigger
    {
        string EventKey { get; }

        bool ShouldTrigger(object eventPayload, PuzzleGraphController controller);
    }

    public interface IPuzzleEffect
    {
        void ApplyEffect(PuzzleGraphController controller);
    }
}
