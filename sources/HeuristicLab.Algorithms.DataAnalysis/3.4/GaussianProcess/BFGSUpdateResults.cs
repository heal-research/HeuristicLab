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
  [Item(Name = "BFGSUpdateResults", Description = "Sets the results (function value and gradients) for the next optimization step in the BFGS algorithm.")]
  public sealed class BFGSUpdateResults : SingleSuccessorOperator {
    private const string QualityGradientsParameterName = "QualityGradients";
    private const string QualityParameterName = "Quality";
    private const string BFGSStateParameterName = "BFGSState";

    #region Parameter Properties
    public ILookupParameter<DoubleArray> QualityGradientsParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[QualityGradientsParameterName]; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<BFGSState> BFGSStateParameter {
      get { return (ILookupParameter<BFGSState>)Parameters[BFGSStateParameterName]; }
    }
    #endregion

    #region Properties
    private DoubleArray QualityGradients { get { return QualityGradientsParameter.ActualValue; } }
    private DoubleValue Quality { get { return QualityParameter.ActualValue; } }
    private BFGSState BFGSState { get { return BFGSStateParameter.ActualValue; } }
    #endregion

    [StorableConstructor]
    private BFGSUpdateResults(bool deserializing) : base(deserializing) { }
    private BFGSUpdateResults(BFGSUpdateResults original, Cloner cloner) : base(original, cloner) { }
    public BFGSUpdateResults()
      : base() {
      // in
      Parameters.Add(new LookupParameter<DoubleArray>(QualityGradientsParameterName, "The gradients at the evaluated point of the function to optimize."));
      Parameters.Add(new LookupParameter<DoubleValue>(QualityParameterName, "The value at the evaluated point of the function to optimize."));
      // in & out
      Parameters.Add(new LookupParameter<BFGSState>(BFGSStateParameterName, "The state of the BFGS algorithm."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BFGSUpdateResults(this, cloner);
    }

    public override IOperation Apply() {
      var state = BFGSState;
      var f = Quality.Value;
      var g = QualityGradients.ToArray();
      state.State.f = f;
      state.State.g = g;
      return base.Apply();
    }
  }
}
