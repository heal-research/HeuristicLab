#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A base class for heuristic optimization algorithms using an execution engine.
  /// </summary>
  [Item("Heuristic Optimization Enigne Algorithm", "A base class for heuristic optimization algorithms using an execution engine.")]
  [StorableClass]
  public abstract class HeuristicOptimizationEngineAlgorithm : EngineAlgorithm {
    public new IHeuristicOptimizationProblem Problem {
      get { return (IHeuristicOptimizationProblem)base.Problem; }
      set { base.Problem = value; }
    }

    protected HeuristicOptimizationEngineAlgorithm() : base() { }
    protected HeuristicOptimizationEngineAlgorithm(string name) : base(name) { }
    protected HeuristicOptimizationEngineAlgorithm(string name, ParameterCollection parameters) : base(name, parameters) { }
    protected HeuristicOptimizationEngineAlgorithm(string name, string description) : base(name, description) { }
    protected HeuristicOptimizationEngineAlgorithm(string name, string description, ParameterCollection parameters) : base(name, description, parameters) { }

    [StorableConstructor]
    protected HeuristicOptimizationEngineAlgorithm(bool deserializing) : base(deserializing) { }
    protected HeuristicOptimizationEngineAlgorithm(HeuristicOptimizationEngineAlgorithm original, Cloner cloner) : base(original, cloner) { }

    #region Events
    protected override void DeregisterProblemEvents() {
      Problem.SolutionCreatorChanged -= new EventHandler(Problem_SolutionCreatorChanged);
      Problem.EvaluatorChanged -= new EventHandler(Problem_EvaluatorChanged);
      base.DeregisterProblemEvents();
    }
    protected override void RegisterProblemEvents() {
      base.RegisterProblemEvents();
      Problem.SolutionCreatorChanged += new EventHandler(Problem_SolutionCreatorChanged);
      Problem.EvaluatorChanged += new EventHandler(Problem_EvaluatorChanged);
    }

    protected virtual void Problem_SolutionCreatorChanged(object sender, EventArgs e) { }
    protected virtual void Problem_EvaluatorChanged(object sender, EventArgs e) { }
    #endregion
  }
}
