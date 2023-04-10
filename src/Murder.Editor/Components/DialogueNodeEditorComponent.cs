using Bang.Components;
using Murder.Assets;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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

            for (int i = 0; i < situation.Dialogs.Length; i++)
            {
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

        public class SimulatorNodes
        {
            public Vector2 Position;
            public Vector2 Speed;
            public int NodeId;
        }
    }
}
