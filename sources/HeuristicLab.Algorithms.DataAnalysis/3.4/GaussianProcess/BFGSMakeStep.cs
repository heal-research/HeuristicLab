#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "BFGSMakeStep", Description = "Makes a step in the BFGS optimization algorithm.")]
  public sealed class BFGSMakeStep : SingleSuccessorOperator {
    private const string TerminationCriterionParameterName = "TerminationCriterion";
    private const string HyperparameterParameterName = "Hyperparameter";
    private const string BFGSStateParameterName = "BFGSState";

    #region Parameter Properties
    public ILookupParameter<BFGSState> BFGSStateParameter {
      get { return (ILookupParameter<BFGSState>)Parameters[BFGSStateParameterName]; }
    }
    public ILookupParameter<BoolValue> TerminationCriterionParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[TerminationCriterionParameterName]; }
    }
    public ILookupParameter<DoubleArray> HyperparameterParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[HyperparameterParameterName]; }
    }
    #endregion


    #region Properties
    public BFGSState BFGSState { get { return BFGSStateParameter.ActualValue; } }
    #endregion

    [StorableConstructor]
    private BFGSMakeStep(bool deserializing) : base(deserializing) { }
    private BFGSMakeStep(BFGSMakeStep original, Cloner cloner) : base(original, cloner) { }
    public BFGSMakeStep()
      : base() {
      // in & out
      Parameters.Add(new LookupParameter<BFGSState>(BFGSStateParameterName, "The state of the BFGS algorithm."));
      // out
      Parameters.Add(new LookupParameter<BoolValue>(TerminationCriterionParameterName, "The termination criterion indicating that the BFGS optimization algorithm should stop."));
      Parameters.Add(new LookupParameter<DoubleArray>(HyperparameterParameterName, "The parameters of the function to optimize."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BFGSMakeStep(this, cloner);
    }

    public override IOperation Apply() {
      var state = BFGSState;
      bool stop = alglib.minlbfgs.minlbfgsiteration(state.State);
      TerminationCriterionParameter.ActualValue = new BoolValue(stop);
      if (!stop) {
        HyperparameterParameter.ActualValue = new DoubleArray(state.State.x);
      } else {
        double[] x = new double[state.State.x.Length];
        alglib.minlbfgs.minlbfgsreport rep = new alglib.minlbfgs.minlbfgsreport();
        alglib.minlbfgs.minlbfgsresults(state.State, ref x, rep);
        HyperparameterParameter.ActualValue = new DoubleArray(x);
      }
      return base.Apply();
    }
  }
}
