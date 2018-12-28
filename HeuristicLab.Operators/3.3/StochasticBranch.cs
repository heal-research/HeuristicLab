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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Operators {
  /// <summary>
  /// A branch of two operators which are executed with a specified probability.
  /// </summary>
  [Item("StochasticBranch", "A branch of two operators which are executed with a specified probability.")]
  [StorableType("5B4F6317-501A-4D29-A6D7-0B926BAD0A88")]
  public class StochasticBranch : SingleSuccessorOperator {
    public LookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<DoubleValue> ProbabilityParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["Probability"]; }
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

    [StorableConstructor]
    protected StochasticBranch(StorableConstructorFlag _) : base(_) { }
    protected StochasticBranch(StochasticBranch original, Cloner cloner)
      : base(original, cloner) {
    }
    public StochasticBranch()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("Probability", "The probability to execute the first branch."));
      Parameters.Add(new OperatorParameter("FirstBranch", "The operator which is executed with the given probability."));
      Parameters.Add(new OperatorParameter("SecondBranch", "The operator which is executed if the first branch is not executed."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticBranch(this, cloner);
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
