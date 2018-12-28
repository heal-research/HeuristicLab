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
  [Item("SymbolicRegressionPruningAnalyzer", "An analyzer that prunes introns from the population.")]
  [StorableType("F1180389-1393-4102-9EEC-E4F183A017F2")]
  public sealed class SymbolicRegressionPruningAnalyzer : SymbolicDataAnalysisSingleObjectivePruningAnalyzer {
    private const string PruningOperatorParameterName = "PruningOperator";
    public IValueParameter<SymbolicRegressionPruningOperator> PruningOperatorParameter {
      get { return (IValueParameter<SymbolicRegressionPruningOperator>)Parameters[PruningOperatorParameterName]; }
    }

    protected override SymbolicDataAnalysisExpressionPruningOperator PruningOperator {
      get { return PruningOperatorParameter.Value; }
    }

    private SymbolicRegressionPruningAnalyzer(SymbolicRegressionPruningAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicRegressionPruningAnalyzer(this, cloner); }

    [StorableConstructor]
    private SymbolicRegressionPruningAnalyzer(StorableConstructorFlag _) : base(_) { }

    public SymbolicRegressionPruningAnalyzer() {
      Parameters.Add(new ValueParameter<SymbolicRegressionPruningOperator>(PruningOperatorParameterName, "The operator used to prune trees", new SymbolicRegressionPruningOperator(new SymbolicRegressionSolutionImpactValuesCalculator())));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3

      #region Backwards compatible code, remove with 3.4
      if (Parameters.ContainsKey(PruningOperatorParameterName)) {
        var oldParam = Parameters[PruningOperatorParameterName] as ValueParameter<SymbolicDataAnalysisExpressionPruningOperator>;
        if (oldParam != null) {
          Parameters.Remove(oldParam);
          Parameters.Add(new ValueParameter<SymbolicRegressionPruningOperator>(PruningOperatorParameterName, "The operator used to prune trees", new SymbolicRegressionPruningOperator(new SymbolicRegressionSolutionImpactValuesCalculator())));
        }
      } else {
        // not yet contained
        Parameters.Add(new ValueParameter<SymbolicRegressionPruningOperator>(PruningOperatorParameterName, "The operator used to prune trees", new SymbolicRegressionPruningOperator(new SymbolicRegressionSolutionImpactValuesCalculator())));
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
