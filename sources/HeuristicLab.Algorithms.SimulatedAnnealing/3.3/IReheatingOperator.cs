using HeuristicLab.Core;
using HeuristicLab.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Algorithms.SimulatedAnnealing
{
    public interface IReheatingOperator : IOperator
    {
        void Parameterize();
    }
}
