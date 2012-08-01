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

using System;
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
  [Item(Name = "BFGSInitializer", Description = "Initializes the necessary data structures for the BFGS algorithm.")]
  public sealed class BFGSInitializer : SingleSuccessorOperator {
    private const string DimensionParameterName = "Dimension";
    private const string PointParameterName = "Point";
    private const string BFGSStateParameterName = "BFGSState";
    private const string IterationsParameterName = "Iterations";

    #region Parameter Properties
    // in
    public ILookupParameter<IntValue> DimensionParameter {
      get { return (ILookupParameter<IntValue>)Parameters[DimensionParameterName]; }
    }
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    // out
    public ILookupParameter<DoubleArray> PointParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[PointParameterName]; }
    }
    public ILookupParameter<BFGSState> BFGSStateParameter {
      get { return (ILookupParameter<BFGSState>)Parameters[BFGSStateParameterName]; }
    }


    #endregion

    #region Properties
    private IntValue Dimension { get { return DimensionParameter.ActualValue; } }
    private IntValue Iterations { get { return IterationsParameter.ActualValue; } }
    #endregion

    [StorableConstructor]
    private BFGSInitializer(bool deserializing) : base(deserializing) { }
    private BFGSInitializer(BFGSInitializer original, Cloner cloner) : base(original, cloner) { }
    public BFGSInitializer()
      : base() {
      // in
      Parameters.Add(new LookupParameter<IntValue>(DimensionParameterName, "The length of the vector to optimize."));
      Parameters.Add(new LookupParameter<IntValue>(IterationsParameterName, "The maximal number of iterations for the BFGS algorithm."));
      // out
      Parameters.Add(new LookupParameter<DoubleArray>(PointParameterName, "The initial point for the BFGS algorithm."));
      Parameters.Add(new LookupParameter<BFGSState>(BFGSStateParameterName, "The state of the BFGS algorithm."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BFGSInitializer(this, cloner);
    }

    public override IOperation Apply() {
      int n = Dimension.Value;
      double[] initialPoint = Enumerable.Repeat(0.0, n).ToArray();
      alglib.minlbfgs.minlbfgsstate state = new alglib.minlbfgs.minlbfgsstate();
      alglib.minlbfgs.minlbfgscreate(n, Math.Min(n, 7), initialPoint, state);
      alglib.minlbfgs.minlbfgssetcond(state, 0, 0, 0, Iterations.Value);
      alglib.minlbfgs.minlbfgssetxrep(state, true);

      PointParameter.ActualValue = new DoubleArray(initialPoint);
      BFGSStateParameter.ActualValue = new BFGSState(state);
      return base.Apply();
    }
  }
}
