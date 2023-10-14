using Bang.Components;
using Murder.Core.Dialogs;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.Components
{
    [Unique]
    readonly struct DialogueNodeEditorComponent : IComponent
    {
        public readonly Situation Situation;
        public readonly DialogNodeEditorSimulator Simulator;
        public DialogueNodeEditorComponent(Situation situation) : this()
        {
            Situation = situation;
            Simulator = new(situation);
        }
    }

    public class DialogNodeEditorSimulator
    {
        private readonly Situation _situation;
        public SimulatorNodes[] Nodes;

        public DialogNodeEditorSimulator(Situation situation)
        {
            _situation = situation;

            Nodes = new SimulatorNodes[situation.Dialogs.Length];

            foreach (var _ in PopulateNodes(situation.Dialogs[0], 0, 0)) ;


            for (int i = 0; i < situation.Dialogs.Length; i++)
            {
                if (Nodes[i] != null)
                    continue;

                var node = situation.Dialogs[i];
                Vector2 position = Vector2.Zero;
                if (i == 0)
                {
                    position = new Vector2(0, -2f);
                }
                else if (situation.Edges.ContainsKey(node.Id))
                {
                    position = Calculator.RandomPointInsideCircle();
                }
                else
                {
                    position = new Vector2(Game.Random.NextFloat(-1f, 1f), 2f);
                }
                Nodes[i] = new SimulatorNodes(node.Id, position, Vector2.Zero);
            }
        }

        private IEnumerable<float> PopulateNodes(Dialog dialog, int depth, float startX)
        {
            if (Nodes[dialog.Id] != null)
                yield return 0;

            float x = 0;
            Nodes[dialog.Id] = new(dialog.Id, new Vector2(startX + x, depth * 0.5f - 2), Vector2.Zero);

            if (_situation.Edges.ContainsKey(dialog.Id))
            {

                foreach (var childId in _situation.Edges[dialog.Id].Dialogs)
                {
                    float toAdd = 0;
                    foreach (var n in PopulateNodes(_situation.Dialogs[childId], depth + 1, startX + x))
                        toAdd += n;
                    x += toAdd;
                }

            }
            x += 0.25f;

            yield return x;
        }

        public class SimulatorNodes
        {
            /// <summary>
            /// This is the node id (which is based on the the dialog id).
            /// </summary>
            public int NodeId;

            public Vector2 Position;
            public Vector2 Speed;

            public SimulatorNodes(int nodeId, Vector2 position, Vector2 speed)
            {
                NodeId = nodeId;
                Position = position;
                Speed = speed;
            }
        }
    }
}