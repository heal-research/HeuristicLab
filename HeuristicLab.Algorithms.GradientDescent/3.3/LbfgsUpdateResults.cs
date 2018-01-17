#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.GradientDescent {
  [StorableClass]
  [Item(Name = "LBFGS UpdateResults", Description = "Sets the results (function value and gradients) for the next optimization step in the LM-BFGS algorithm.")]
  public sealed class LbfgsUpdateResults : SingleSuccessorOperator {
    private const string QualityGradientsParameterName = "QualityGradients";
    private const string QualityParameterName = "Quality";
    private const string StateParameterName = "State";
    private const string ApproximateGradientsParameterName = "ApproximateGradients";
    private const string MaximizationParameterName = "Maximization";

    #region Parameter Properties
    public ILookupParameter<BoolValue> ApproximateGradientsParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[ApproximateGradientsParameterName]; }
    }
    public ILookupParameter<RealVector> QualityGradientsParameter {
      get { return (ILookupParameter<RealVector>)Parameters[QualityGradientsParameterName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<LbfgsState> StateParameter {
      get { return (ILookupParameter<LbfgsState>)Parameters[StateParameterName]; }
    }
    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    #endregion

    #region Properties
    private BoolValue ApproximateGradients { get { return ApproximateGradientsParameter.ActualValue; } }
    private RealVector QualityGradients { get { return QualityGradientsParameter.ActualValue; } }
    private DoubleValue Quality { get { return QualityParameter.ActualValue; } }
    private LbfgsState State { get { return StateParameter.ActualValue; } }

    private BoolValue Maximization {
      get {
        // BackwardsCompatibility3.3
        #region Backwards compatible code, remove with 3.4
        // the parameter is new, previously we assumed minimization problems
        if (MaximizationParameter.ActualValue == null) return new BoolValue(false);
        #endregion
        return MaximizationParameter.ActualValue;
      }
    }

    #endregion

    [StorableConstructor]
    private LbfgsUpdateResults(bool deserializing) : base(deserializing) { }
    private LbfgsUpdateResults(LbfgsUpdateResults original, Cloner cloner) : base(original, cloner) { }
    public LbfgsUpdateResults()
      : base() {
      // in
      Parameters.Add(new LookupParameter<RealVector>(QualityGradientsParameterName, "The gradients at the evaluated point of the function to optimize."));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The value at the evaluated point of the function to optimize."));
      Parameters.Add(new LookupParameter<BoolValue>(ApproximateGradientsParameterName,
                                                    "Flag that indicates if gradients should be approximated."));
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "Flag that indicates if we solve a maximization problem."));
      // in & out
      Parameters.Add(new LookupParameter<LbfgsState>(StateParameterName, "The state of the LM-BFGS algorithm."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3

      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(MaximizationParameterName)) {
        // previous behaviour defaulted to minimization
        Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "Flag that indicates if we solve a maximization problem."));
      }
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LbfgsUpdateResults(this, cloner);
    }

    public override IOperation Apply() {
      var state = State;
      var sign = Maximization.Value ? -1.0 : 1.0;
      var f = sign * Quality.Value;
      state.State.f = f;
      if (!ApproximateGradients.Value) {
        var g = QualityGradients.Select(gi => sign * gi).ToArray();
        state.State.g = g;
      }
      return base.Apply();
    }
  }
}
