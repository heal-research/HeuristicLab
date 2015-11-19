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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.GradientDescent {
  [StorableClass]
  [Item(Name = "LBFGS MakeStep", Description = "Makes a step in the LM-BFGS optimization algorithm.")]
  public sealed class LbfgsMakeStep : SingleSuccessorOperator {
    private const string TerminationCriterionParameterName = "TerminationCriterion";
    private const string PointParameterName = "Point";
    private const string StateParameterName = "State";

    #region Parameter Properties
    public ILookupParameter<LbfgsState> StateParameter {
      get { return (ILookupParameter<LbfgsState>)Parameters[StateParameterName]; }
    }
    public ILookupParameter<BoolValue> TerminationCriterionParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[TerminationCriterionParameterName]; }
    }
    public ILookupParameter<RealVector> PointParameter {
      get { return (ILookupParameter<RealVector>)Parameters[PointParameterName]; }
    }
    #endregion


    #region Properties
    private LbfgsState State { get { return StateParameter.ActualValue; } }
    #endregion

    [StorableConstructor]
    private LbfgsMakeStep(bool deserializing) : base(deserializing) { }
    private LbfgsMakeStep(LbfgsMakeStep original, Cloner cloner) : base(original, cloner) { }
    public LbfgsMakeStep()
      : base() {
      // in & out
      Parameters.Add(new LookupParameter<LbfgsState>(StateParameterName, "The state of the LM-BFGS algorithm."));
      // out
      Parameters.Add(new LookupParameter<BoolValue>(TerminationCriterionParameterName, "The termination criterion indicating that the LM-BFGS optimization algorithm should stop."));
      Parameters.Add(new LookupParameter<RealVector>(PointParameterName, "The next point that should be evaluated in the LM-BFGS algorithm."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LbfgsMakeStep(this, cloner);
    }

    public override IOperation Apply() {
      var state = State;
      bool @continue = alglib.minlbfgs.minlbfgsiteration(state.State);
      TerminationCriterionParameter.ActualValue = new BoolValue(!@continue);
      if (@continue) {
        PointParameter.ActualValue = new RealVector(state.State.x);
      } else {
        double[] x = new double[state.State.x.Length];
        alglib.minlbfgs.minlbfgsreport rep = new alglib.minlbfgs.minlbfgsreport();
        alglib.minlbfgs.minlbfgsresults(state.State, ref x, rep);
        if (rep.terminationtype < 0) {
          if (rep.terminationtype == -1)
            throw new OperatorExecutionException(this, "Incorrect parameters were specified.");
          else if (rep.terminationtype == -2)
            throw new OperatorExecutionException(this, "Rounding errors prevent further improvement.");
          else if (rep.terminationtype == -7)
            throw new OperatorExecutionException(this, "Gradient verification failed.");
        }
        PointParameter.ActualValue = new RealVector(x);
      }
      return base.Apply();
    }
  }
}
