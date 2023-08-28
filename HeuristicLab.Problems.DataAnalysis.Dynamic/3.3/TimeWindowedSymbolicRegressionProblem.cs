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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.Dynamic {
  [Item("Time Windowed Symbolic Regression Problem", "Applies a dynamic time window on a regression problem")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 210)]
  [StorableType("B8C7B816-9332-4786-8C5A-187B2A4AAAC6")]
  public class TimeWindowedSymbolicRegressionProblem : SingleObjectiveStatefulDynamicProblem<SymbolicExpressionTreeEncoding, SymbolicExpressionTree, TimeWindowedSymbolicRegressionProblemState> {

    #region Propeties
    public override bool Maximization => !Parameters.ContainsKey(StateParameterName) || (State?.Maximization??true);
    #endregion

    #region Constructors and Cloning
    public TimeWindowedSymbolicRegressionProblem() {
      InitialState = new TimeWindowedSymbolicRegressionProblemState();
      Encoding = InitialState.Encoding;
      SolutionCreator = InitialState.SolutionCreator;
      RegisterProblemEventHandlers();
    }

    private void RegisterProblemEventHandlers() {
      InitialState.SolutionCreatorChanged+= (a,b)=> SolutionCreator = InitialState.SolutionCreator;
    }

    [StorableConstructor]
    protected TimeWindowedSymbolicRegressionProblem(StorableConstructorFlag _) : base(_) { }

    
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterProblemEventHandlers();
    }
    

    protected TimeWindowedSymbolicRegressionProblem(TimeWindowedSymbolicRegressionProblem original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TimeWindowedSymbolicRegressionProblem(this, cloner);
    }
    #endregion

    #region ProblemMethods
    protected override double Evaluate(Individual individual, IRandom random, bool dummy) {
      return State.Evaluate(individual, random);
    }
    #endregion
  }
}
