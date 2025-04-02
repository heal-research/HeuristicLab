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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("SymbolicRegressionMultiObjectivePruningAnalyzer", "An analyzer that prunes introns from the population.")]
  [StorableType("3BD49806-7B8B-46A9-89A2-DA3C5499CF6A")]
  public sealed class SymbolicRegressionMultiObjectivePruningAnalyzer : SymbolicDataAnalysisMultiObjectivePruningAnalyzer {
    private const string PruningOperatorParameterName = "PruningOperator";
    public IValueParameter<SymbolicRegressionMultiObjectivePruningOperator> PruningOperatorParameter {
      get { return (IValueParameter<SymbolicRegressionMultiObjectivePruningOperator>)Parameters[PruningOperatorParameterName]; }
    }

    protected override SymbolicDataAnalysisMultiObjectiveExpressionPruningOperator PruningOperator {
      get { return PruningOperatorParameter.Value; }
    }

    private SymbolicRegressionMultiObjectivePruningAnalyzer(SymbolicRegressionMultiObjectivePruningAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicRegressionMultiObjectivePruningAnalyzer(this, cloner); }

    [StorableConstructor]
    private SymbolicRegressionMultiObjectivePruningAnalyzer(StorableConstructorFlag _) : base(_) { }

    public SymbolicRegressionMultiObjectivePruningAnalyzer() {
      var op = new SymbolicRegressionMultiObjectivePruningOperator(new SymbolicRegressionSolutionImpactValuesCalculator());
      Parameters.Add(new ValueParameter<SymbolicRegressionMultiObjectivePruningOperator>(PruningOperatorParameterName, "The operator used to prune trees", op));
      //Parameters.Add(op.QualityParameter); //this is to expose the parameter to the operator wiring

    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3

      #region Backwards compatible code, remove with 3.4
      if (Parameters.ContainsKey(PruningOperatorParameterName)) {
        var oldParam = Parameters[PruningOperatorParameterName] as ValueParameter<SymbolicDataAnalysisMultiObjectiveExpressionPruningOperator>;
        if (oldParam != null) {
          Parameters.Remove(oldParam);
          Parameters.Add(new ValueParameter<SymbolicRegressionMultiObjectivePruningOperator>(PruningOperatorParameterName, "The operator used to prune trees", new SymbolicRegressionMultiObjectivePruningOperator(new SymbolicRegressionSolutionImpactValuesCalculator())));
        }
      } else {
        // not yet contained
        Parameters.Add(new ValueParameter<SymbolicRegressionMultiObjectivePruningOperator>(PruningOperatorParameterName, "The operator used to prune trees", new SymbolicRegressionMultiObjectivePruningOperator(new SymbolicRegressionSolutionImpactValuesCalculator())));
      }


      if (Parameters.ContainsKey("PruneOnlyZeroImpactNodes")) {
        PruningOperator.PruneOnlyZeroImpactNodes = ((IFixedValueParameter<BoolValue>)Parameters["PruneOnlyZeroImpactNodes"]).Value.Value;
        Parameters.Remove(Parameters["PruneOnlyZeroImpactNodes"]);
      }
      if (Parameters.ContainsKey("ImpactThreshold")) {
        PruningOperator.NodeImpactThreshold = ((IFixedValueParameter<DoubleValue>)Parameters["ImpactThreshold"]).Value.Value;
        Parameters.Remove(Parameters["ImpactThreshold"]);
      }
      if (Parameters.ContainsKey("ImpactValuesCalculator")) {
        PruningOperator.ImpactValuesCalculator = ((ValueParameter<SymbolicDataAnalysisSolutionImpactValuesCalculator>)Parameters["ImpactValuesCalculator"]).Value;
        Parameters.Remove(Parameters["ImpactValuesCalculator"]);
      }

      #endregion
    }
  }
}
