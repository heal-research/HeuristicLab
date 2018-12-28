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

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("1BEEE472-95E7-45C3-BD47-883B5E3A672D")]
  public abstract class SymbolicRegressionMultiObjectiveEvaluator : SymbolicDataAnalysisMultiObjectiveEvaluator<IRegressionProblemData>, ISymbolicRegressionMultiObjectiveEvaluator {
    private const string DecimalPlacesParameterName = "Decimal Places";
    private const string UseConstantOptimizationParameterName = "Use constant optimization";
    private const string ConstantOptimizationIterationsParameterName = "Constant optimization iterations";

    private const string ConstantOptimizationUpdateVariableWeightsParameterName =
      "Constant optimization update variable weights";

    public IFixedValueParameter<IntValue> DecimalPlacesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[DecimalPlacesParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UseConstantOptimizationParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UseConstantOptimizationParameterName]; }
    }

    public IFixedValueParameter<IntValue> ConstantOptimizationIterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[ConstantOptimizationIterationsParameterName]; }
    }

    public IFixedValueParameter<BoolValue> ConstantOptimizationUpdateVariableWeightsParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[ConstantOptimizationUpdateVariableWeightsParameterName]; }
    }

    public int DecimalPlaces {
      get { return DecimalPlacesParameter.Value.Value; }
      set { DecimalPlacesParameter.Value.Value = value; }
    }
    public bool UseConstantOptimization {
      get { return UseConstantOptimizationParameter.Value.Value; }
      set { UseConstantOptimizationParameter.Value.Value = value; }
    }
    public int ConstantOptimizationIterations {
      get { return ConstantOptimizationIterationsParameter.Value.Value; }
      set { ConstantOptimizationIterationsParameter.Value.Value = value; }
    }
    public bool ConstantOptimizationUpdateVariableWeights {
      get { return ConstantOptimizationUpdateVariableWeightsParameter.Value.Value; }
      set { ConstantOptimizationUpdateVariableWeightsParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected SymbolicRegressionMultiObjectiveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected SymbolicRegressionMultiObjectiveEvaluator(SymbolicRegressionMultiObjectiveEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }

    protected SymbolicRegressionMultiObjectiveEvaluator()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(DecimalPlacesParameterName, "The number of decimal places used for rounding the quality values.", new IntValue(5)) { Hidden = true });
      Parameters.Add(new FixedValueParameter<BoolValue>(UseConstantOptimizationParameterName, "", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>(ConstantOptimizationIterationsParameterName, "The number of iterations constant optimization should be applied.", new IntValue(5)));
      Parameters.Add(new FixedValueParameter<BoolValue>(ConstantOptimizationUpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be optimized during constant optimization.", new BoolValue(true)) { Hidden = true });
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(UseConstantOptimizationParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(UseConstantOptimizationParameterName, "", new BoolValue(false)));
      }
      if (!Parameters.ContainsKey(DecimalPlacesParameterName)) {
        Parameters.Add(new FixedValueParameter<IntValue>(DecimalPlacesParameterName, "The number of decimal places used for rounding the quality values.", new IntValue(-1)) { Hidden = true });
      }
      if (!Parameters.ContainsKey(ConstantOptimizationIterationsParameterName)) {
        Parameters.Add(new FixedValueParameter<IntValue>(ConstantOptimizationIterationsParameterName, "The number of iterations constant optimization should be applied.", new IntValue(5)));
      }
      if (!Parameters.ContainsKey(ConstantOptimizationUpdateVariableWeightsParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(ConstantOptimizationUpdateVariableWeightsParameterName, "Determines if the variable weights in the tree should be optimized during constant optimization.", new BoolValue(true)));
      }
    }
  }
}
