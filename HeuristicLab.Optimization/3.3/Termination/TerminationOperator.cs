#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("TerminationOperator", "An operator which either calls the terminate- or the continue branch.")]
  [StorableClass]
  public sealed class TerminationOperator : InstrumentedOperator, ITerminationBasedOperator {
    public ILookupParameter<ITerminator> TerminatorParameter {
      get { return (ILookupParameter<ITerminator>)Parameters["Terminator"]; }
    }
    public ILookupParameter<BoolValue> TerminateParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Terminate"]; }
    }
    private OperatorParameter ContinueBranchParameter {
      get { return (OperatorParameter)Parameters["ContinueBranch"]; }
    }
    private OperatorParameter TerminateBranchParameter {
      get { return (OperatorParameter)Parameters["TerminateBranch"]; }
    }

    public IOperator ContinueBranch {
      get { return ContinueBranchParameter.Value; }
      set { ContinueBranchParameter.Value = value; }
    }
    public IOperator TerminateBranch {
      get { return TerminateBranchParameter.Value; }
      set { TerminateBranchParameter.Value = value; }
    }

    [StorableConstructor]
    private TerminationOperator(bool deserializing) : base(deserializing) { }
    private TerminationOperator(TerminationOperator original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TerminationOperator(this, cloner);
    }

    public TerminationOperator()
      : base() {
      Parameters.Add(new LookupParameter<ITerminator>("Terminator", "The termination criteria which sould be checked."));
      Parameters.Add(new LookupParameter<BoolValue>("Terminate", "The parameter which will be set to determine if execution should be terminated or schould continue."));
      Parameters.Add(new OperatorParameter("ContinueBranch", "The operator which is executed if no termination criteria has met."));
      Parameters.Add(new OperatorParameter("TerminateBranch", "The operator which is executed if any termination criteria has met."));

      var assigner = new Assigner() { Name = "Terminate = false" };
      assigner.LeftSideParameter.ActualName = TerminateParameter.Name;
      assigner.RightSideParameter.Value = new BoolValue(false);

      var placeholder = new Placeholder() { Name = "Check termination criteria (Placeholder)" };
      placeholder.OperatorParameter.ActualName = TerminatorParameter.Name;

      BeforeExecutionOperators.Add(assigner);
      BeforeExecutionOperators.Add(placeholder);
    }

    public override IOperation InstrumentedApply() {
      var next = new OperationCollection(base.InstrumentedApply());
      if (TerminateParameter.ActualValue.Value) {
        if (TerminateBranch != null) next.Insert(0, ExecutionContext.CreateOperation(TerminateBranch));
      } else {
        if (ContinueBranch != null) next.Insert(0, ExecutionContext.CreateOperation(ContinueBranch));
      }
      return next;
    }
  }
}