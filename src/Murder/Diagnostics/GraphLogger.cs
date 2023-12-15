namespace Murder.Diagnostics
{
    /// <summary>
    /// Implement this for plotting graphs into the debug system.
    /// </summary>
    public class GraphLogger
    {
        public virtual void ClearAllGraphs()
        { }

        public virtual void ClearGraph(string callerFilePath)
        { }

        public virtual void PlotGraph(float value, string callerFilePath)
        { }
    }
}
