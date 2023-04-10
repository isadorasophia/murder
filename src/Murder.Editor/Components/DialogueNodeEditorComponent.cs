using Bang.Components;
using Murder.Assets;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

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
                    position = new Vector2(Game.Random.NextFloat(-1f,1f), 2f);
                }
                Nodes[i] = new SimulatorNodes()
                {
                    NodeId = node.Id,
                    Position = position
                };
            }
        }

        private IEnumerable<float> PopulateNodes(Dialog dialog, int depth, float startX)
        {
            if (Nodes[dialog.Id] != null)
                yield return 0;

            float x = 0;
            Nodes[dialog.Id] = new()
            {
                NodeId = dialog.Id,
                Position = new Vector2(startX + x, depth *0.5f - 2)
            };

            if (_situation.Edges.ContainsKey(dialog.Id))
            {

                foreach (var childId in _situation.Edges[dialog.Id].Dialogs)
                {
                    float toAdd = 0;
                    foreach (var n in PopulateNodes(_situation.Dialogs[childId], depth + 1, startX+x))
                        toAdd+=n;
                    x += toAdd;
                }
                
            }
            x += 0.25f;

            yield return x;
        }

        public class SimulatorNodes
        {
            public Vector2 Position;
            public Vector2 Speed;
            public int NodeId;
        }
    }
}
