#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// A branch of two operators which are executed with a specified probability.
  /// </summary>
  [Item("StochasticBranch", "A branch of two operators which are executed with a specified probability.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public class StochasticBranch : SingleSuccessorOperator {
    public LookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<DoubleData> ProbabilityParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Probability"]; }
    }
    protected OperatorParameter FirstBranchParameter {
      get { return (OperatorParameter)Parameters["FirstBranch"]; }
    }
    protected OperatorParameter SecondBranchParameter {
      get { return (OperatorParameter)Parameters["SecondBranch"]; }
    }
    public IOperator FirstBranch {
      get { return FirstBranchParameter.Value; }
      set { FirstBranchParameter.Value = value; }
    }
    public IOperator SecondBranch {
      get { return SecondBranchParameter.Value; }
      set { SecondBranchParameter.Value = value; }
    }

    public StochasticBranch()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<DoubleData>("Probability", "The probability to execute the first branch."));
      Parameters.Add(new OperatorParameter("FirstBranch", "The operator which is executed with the given probability."));
      Parameters.Add(new OperatorParameter("SecondBranch", "The operator which is executed if the first branch is not executed."));
    }

    public override IOperation Apply() {
      OperationCollection next = new OperationCollection(base.Apply());
      if (RandomParameter.ActualValue.NextDouble() < ProbabilityParameter.ActualValue.Value) {
        if (FirstBranch != null) next.Insert(0, ExecutionContext.CreateOperation(FirstBranch));
      } else {
        if (SecondBranch != null) next.Insert(0, ExecutionContext.CreateOperation(SecondBranch));
      }
      return next;
    }
  }
}
