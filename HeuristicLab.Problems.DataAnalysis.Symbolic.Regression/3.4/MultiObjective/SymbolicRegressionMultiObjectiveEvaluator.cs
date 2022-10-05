#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("1BEEE472-95E7-45C3-BD47-883B5E3A672D")]
  public abstract class SymbolicRegressionMultiObjectiveEvaluator : SymbolicDataAnalysisMultiObjectiveEvaluator<IRegressionProblemData>, ISymbolicRegressionMultiObjectiveEvaluator {
    private const string DecimalPlacesParameterName = "Decimal Places";
    private const string UseParameterOptimizationParameterName = "Use parameter optimization";
    private const string ParameterOptimizationIterationsParameterName = "Parameter optimization iterations";

    private const string ParameterOptimizationUpdateVariableWeightsParameterName =
      "Parameter optimization update variable weights";

    public IFixedValueParameter<IntValue> DecimalPlacesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[DecimalPlacesParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UseParameterOptimizationParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UseParameterOptimizationParameterName]; }
    }

    public IFixedValueParameter<IntValue> ParameterOptimizationIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[ParameterOptimizationIterationsParameterName]; }
    }

    public IFixedValueParameter<BoolValue> ParameterOptimizationUpdateVariableWeightsParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[ParameterOptimizationUpdateVariableWeightsParameterName]; }
    }

    public int DecimalPlaces {
      get { return DecimalPlacesParameter.Value.Value; }
      set { DecimalPlacesParameter.Value.Value = value; }
    }
    public bool UseParameterOptimization {
      get { return UseParameterOptimizationParameter.Value.Value; }
      set { UseParameterOptimizationParameter.Value.Value = value; }
    }
    public int ParameterOptimizationIterations {
      get { return ParameterOptimizationIterationsParameter.Value.Value; }
      set { ParameterOptimizationIterationsParameter.Value.Value = value; }
    }
    public bool ParameterOptimizationUpdateVariableWeights {
      get { return ParameterOptimizationUpdateVariableWeightsParameter.Value.Value; }
      set { ParameterOptimizationUpdateVariableWeightsParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected SymbolicRegressionMultiObjectiveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicRegressionMultiObjectiveEvaluator(SymbolicRegressionMultiObjectiveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SymbolicRegressionMultiObjectiveEvaluator()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(DecimalPlacesParameterName, "The number of decimal places used for rounding the quality values.", new IntValue(5)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<BoolValue>(UseParameterOptimizationParameterName, "", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>(ParameterOptimizationIterationsParameterName, "The number of iterations parameter optimization should be applied.", new IntValue(5)));
      Parameters.Add(new FixedValueParameter<BoolValue>(ParameterOptimizationUpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be optimized during parameter optimization.", new BoolValue(true)) { Hidden = true });
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(UseParameterOptimizationParameterName)) {
        if (Parameters.ContainsKey("Use constant optimization")) {
          Parameters.Add(new FixedValueParameter<BoolValue>(UseParameterOptimizationParameterName, "", (BoolValue)Parameters["Use constant optimization"].ActualValue));
          Parameters.Remove("Use constant optimization");
        } else {
          Parameters.Add(new FixedValueParameter<BoolValue>(UseParameterOptimizationParameterName, "", new BoolValue(false)));
        }
      }

      if (!Parameters.ContainsKey(DecimalPlacesParameterName)) {
        Parameters.Add(new FixedValueParameter<IntValue>(DecimalPlacesParameterName, "The number of decimal places used for rounding the quality values.", new IntValue(-1)) { Hidden = true });
      }
      if (!Parameters.ContainsKey(ParameterOptimizationIterationsParameterName)) {
        if (Parameters.ContainsKey("Constant optimization iterations")) {
          Parameters.Add(new FixedValueParameter<IntValue>(ParameterOptimizationIterationsParameterName, "The number of iterations parameter optimization should be applied.", (IntValue)Parameters["Constant optimization iterations"].ActualValue));
          Parameters.Remove("Constant optimization iterations");
        } else {
          Parameters.Add(new FixedValueParameter<IntValue>(ParameterOptimizationIterationsParameterName, "The number of iterations parameter optimization should be applied.", new IntValue(5)));
        }
      }
      if (!Parameters.ContainsKey(ParameterOptimizationUpdateVariableWeightsParameterName)) {
        if (Parameters.ContainsKey("Constant optimization update variable weights")) {
          Parameters.Add(new FixedValueParameter<BoolValue>(ParameterOptimizationUpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be optimized during parameter optimization.",
            (BoolValue)Parameters["Constant optimization update variable weights"].ActualValue));
          Parameters.Remove("Constant optimization update variable weights");
        } else {
          Parameters.Add(new FixedValueParameter<BoolValue>(ParameterOptimizationUpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be optimized during parameter optimization.", new BoolValue(true)));
        }
      }
    }
  }
}
