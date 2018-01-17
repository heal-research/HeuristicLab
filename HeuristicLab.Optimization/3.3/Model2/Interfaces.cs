#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System.Threading;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization.Model2 {
  public interface ISolutionScope : IScope {
    double Fitness { get; set; }
  }

  public interface ISolutionScope<TSolution> : ISolutionScope {
    TSolution Solution { get; set; }
  }

  public interface IContext : IExecutionContext {
    new IExecutionContext Parent { get; set; }
    int Iterations { get; set; }
    int EvaluatedSolutions { get; set; }
    double BestQuality { get; set; }
    bool Terminate { get; set; }

    void RunOperator(IOperator op, CancellationToken token);
  }

  public interface IStochasticContext : IContext {
    IRandom Random { get; set; }
  }
}
