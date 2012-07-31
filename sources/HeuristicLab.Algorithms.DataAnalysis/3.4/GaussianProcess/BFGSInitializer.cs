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
    private const string NumberOfHyperparameterParameterName = "NumberOfHyperparameter";
    private const string HyperparameterParameterName = "Hyperparameter";
    private const string BFGSStateParameterName = "BFGSState";
    private const string IterationsParameterName = "Iterations";

    #region Parameter Properties
    // in
    public ILookupParameter<IntValue> NumberOfHyperparameterParameter {
      get { return (ILookupParameter<IntValue>)Parameters[NumberOfHyperparameterParameterName]; }
    }
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    // out
    public ILookupParameter<DoubleArray> HyperparameterParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters[HyperparameterParameterName]; }
    }
    public ILookupParameter<BFGSState> BFGSStateParameter {
      get { return (ILookupParameter<BFGSState>)Parameters[BFGSStateParameterName]; }
    }


    #endregion

    #region Properties
    public IntValue NumberOfHyperparameter { get { return NumberOfHyperparameterParameter.ActualValue; } }
    public IntValue Iterations { get { return IterationsParameter.ActualValue; } }
    #endregion

    [StorableConstructor]
    private BFGSInitializer(bool deserializing) : base(deserializing) { }
    private BFGSInitializer(BFGSInitializer original, Cloner cloner) : base(original, cloner) { }
    public BFGSInitializer()
      : base() {
      // in
      Parameters.Add(new LookupParameter<IntValue>(NumberOfHyperparameterParameterName, "The number of parameters to optimize."));
      Parameters.Add(new LookupParameter<IntValue>(IterationsParameterName, "The maximal number of iterations for the BFGS algorithm."));
      // out
      Parameters.Add(new LookupParameter<DoubleArray>(HyperparameterParameterName, "The hyperparameters for the Gaussian process model."));
      Parameters.Add(new LookupParameter<BFGSState>(BFGSStateParameterName, "The state of the BFGS algorithm."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BFGSInitializer(this, cloner);
    }

    public override IOperation Apply() {
      int n = NumberOfHyperparameter.Value;
      double[] initialHyp = Enumerable.Repeat(0.0, n).ToArray();
      alglib.minlbfgs.minlbfgsstate state = new alglib.minlbfgs.minlbfgsstate();
      alglib.minlbfgs.minlbfgscreate(n, Math.Min(n, 5), initialHyp, state);
      alglib.minlbfgs.minlbfgssetcond(state, 0, 0, 0, Iterations.Value);

      HyperparameterParameter.ActualValue = new DoubleArray(initialHyp);
      BFGSStateParameter.ActualValue = new BFGSState(state);
      return base.Apply();
    }
  }
}
