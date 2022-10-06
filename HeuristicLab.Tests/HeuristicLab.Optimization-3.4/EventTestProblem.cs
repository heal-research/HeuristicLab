#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;

namespace HeuristicLab.Optimization.Tests{
  internal class EventTestProblem : SingleObjectiveBasicProblem<RealVectorEncoding> {

  public int State {
    get;
    private set;
  }

  public EventTestProblem() {
    State = 0;
  }

  public EventTestProblem(EventTestProblem original, Cloner cloner) : base(original, cloner) {
    State = original.State;
  }

  public override IDeepCloneable Clone(Cloner cloner) {
    return new EventTestProblem();
  }

  public override void RegisterAlgorithmEvents(IAlgorithm algorithm) {
    base.RegisterAlgorithmEvents(algorithm);
    algorithm.ExceptionOccurred += OnAlgorithmException;
    algorithm.ExecutionStateChanged += OnAlgorithmExecutionStateChanged;
  }

  private void OnAlgorithmExecutionStateChanged(object sender, EventArgs e) {
    var alg = (IAlgorithm)sender;
    if (State < 0) return;
    switch (alg.ExecutionState) {
      case ExecutionState.Prepared:
        State = 1;
        break;
      case ExecutionState.Started:
        State = 2;
        break;
      case ExecutionState.Paused:
        State = 3;
        break;
      case ExecutionState.Stopped:
        State = 4;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private void OnAlgorithmException(object sender, EventArgs<Exception> e) {
    State = -1;
  }

  public override void DeregisterAlgorithmEvents(IAlgorithm algorithm) {
    base.RegisterAlgorithmEvents(algorithm);
    algorithm.ExceptionOccurred -= OnAlgorithmException;
    algorithm.ExecutionStateChanged -= OnAlgorithmExecutionStateChanged;
  }

  public override double Evaluate(Individual individual, IRandom random) {
    return State;
  }

  public override bool Maximization => false;
  }
}
