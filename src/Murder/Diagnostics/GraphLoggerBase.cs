using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Diagnostics
{
    public class GraphLoggerBase
    {
        public virtual void ClearAllGraphs()
        {
        }
        public virtual void ClearGraph(string callerFilePath)
        {
        }

        public virtual void PlotGraph(float value, string callerFilePath)
        {
        }
    }
}
