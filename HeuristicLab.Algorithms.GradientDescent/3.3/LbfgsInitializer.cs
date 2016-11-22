#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item(Name = "LBFGS Initializer", Description = "Initializes the necessary data structures for the LM-BFGS algorithm.")]
  public sealed class LbfgsInitializer : SingleSuccessorOperator {
    private const string PointParameterName = "Point";
    private const string StateParameterName = "State";
    private const string IterationsParameterName = "Iterations";
    private const string ApproximateGradientsParameterName = "ApproximateGradients";
    private const string GradientCheckStepSizeParameterName = "GradientCheckStepSize";

    #region Parameter Properties
    // in
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    public ILookupParameter<RealVector> PointParameter {
      get { return (ILookupParameter<RealVector>)Parameters[PointParameterName]; }
    }
    // out
    public ILookupParameter<LbfgsState> StateParameter {
      get { return (ILookupParameter<LbfgsState>)Parameters[StateParameterName]; }
    }
    public ILookupParameter<BoolValue> ApproximateGradientsParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[ApproximateGradientsParameterName]; }
    }
    public ILookupParameter<DoubleValue> GradientStepSizeParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[GradientCheckStepSizeParameterName]; }
    }


    #endregion

    #region Properties
    private RealVector Point { get { return PointParameter.ActualValue; } }
    private IntValue Iterations { get { return IterationsParameter.ActualValue; } }
    private BoolValue ApproximateGradients { get { return ApproximateGradientsParameter.ActualValue; } }
    #endregion

    [StorableConstructor]
    private LbfgsInitializer(bool deserializing) : base(deserializing) { }
    private LbfgsInitializer(LbfgsInitializer original, Cloner cloner) : base(original, cloner) { }
    public LbfgsInitializer()
      : base() {
      // in
      Parameters.Add(new LookupParameter<RealVector>(PointParameterName, "The initial point for the LM-BFGS algorithm."));
      Parameters.Add(new LookupParameter<IntValue>(IterationsParameterName, "The maximal number of iterations for the LM-BFGS algorithm."));
      Parameters.Add(new LookupParameter<BoolValue>(ApproximateGradientsParameterName,
                                                    "Flag that indicates if gradients should be approximated."));
      Parameters.Add(new LookupParameter<DoubleValue>(GradientCheckStepSizeParameterName, "Step size for the gradient check (should be used for debugging the gradient calculation only)."));
      // out
      Parameters.Add(new LookupParameter<LbfgsState>(StateParameterName, "The state of the LM-BFGS algorithm."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LbfgsInitializer(this, cloner);
    }

    public override IOperation Apply() {
      double[] initialPoint = Point.ToArray();
      int n = initialPoint.Length;
      alglib.minlbfgs.minlbfgsstate state = new alglib.minlbfgs.minlbfgsstate();
      if (ApproximateGradients.Value) {
        alglib.minlbfgs.minlbfgscreatef(n, Math.Min(n, 10), initialPoint, 1E-5, state);
      } else {
        alglib.minlbfgs.minlbfgscreate(n, Math.Min(n, 10), initialPoint, state);
      }
      alglib.minlbfgs.minlbfgssetcond(state, 0.0, 0, 0, Iterations.Value);
      alglib.minlbfgs.minlbfgssetxrep(state, true);
      if (GradientStepSizeParameter.ActualValue != null && GradientStepSizeParameter.ActualValue.Value > 0)
        alglib.minlbfgs.minlbfgssetgradientcheck(state, GradientStepSizeParameter.ActualValue.Value);

      PointParameter.ActualValue = new RealVector(initialPoint);
      StateParameter.ActualValue = new LbfgsState(state);
      return base.Apply();
    }
  }
}
