using Murder.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Murder.Editor.Systems.Debug.GraphLogger;

namespace Murder.Editor.Systems.Debug
{
    internal class GraphLogger : GraphLoggerBase
    {

        public class Graph
        {
            private readonly List<float> _values = new List<float>(400);
            private float[] _valuesCache = new float[0];
            public void Plot(float point)
            {
                _values.Add(point);
                _valuesCache = _values.ToArray();
            }

            public float[] Values => _valuesCache;
        }
        public readonly Dictionary<string, Graph> Graphs = new Dictionary<string, Graph>();

        public override void PlotGraph(float value, string callerFilePath)
        {
            string callerClassName = Path.GetFileNameWithoutExtension(callerFilePath);
            if (!Graphs.ContainsKey(callerClassName))
            {
                Graphs[callerClassName] = new Graph();
            }

            var graph = Graphs[callerClassName];
            graph.Plot(value);
        }

        public override void ClearGraph(string callerFilePath)
        {
            string callerClassName = Path.GetFileNameWithoutExtension(callerFilePath);
            Graphs.Remove(callerClassName);
        }

        public override void ClearAllGraphs()
        {
            Graphs.Clear();
        }
    }
}
