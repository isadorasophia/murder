using Murder.Diagnostics;

namespace Murder.Editor.Systems.Debug
{
    internal class EditorGraphLogger : GraphLogger
    {
        public class Graph
        {
            private readonly List<float> _values = new(512);
            private float[] _valuesCache = [];
            private float _maxValue = float.Epsilon;

            public void Plot(float point)
            {
                _values.Add(point);
                _maxValue = MathF.Max(point, _maxValue);
                _valuesCache = [.. _values];
            }

            public float[] Values => _valuesCache;
            public float Max => _maxValue;
        }

        public readonly Dictionary<string, Graph> Graphs = new();

        public override void PlotGraph(float value, string callerFilePath)
        {
            string callerClassName = Path.GetFileNameWithoutExtension(callerFilePath);
            if (!Graphs.ContainsKey(callerClassName))
            {
                Graphs[callerClassName] = new Graph();
            }

            Graph graph = Graphs[callerClassName];
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
