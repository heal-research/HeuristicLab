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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableClass]
  [Item("SymbolicClassificationSolutionImpactValuesCalculator", "Calculate symbolic expression tree node impact values for classification problems.")]
  public class SymbolicClassificationSolutionImpactValuesCalculator : SymbolicDataAnalysisSolutionImpactValuesCalculator {
    public SymbolicClassificationSolutionImpactValuesCalculator() { }
    protected SymbolicClassificationSolutionImpactValuesCalculator(SymbolicClassificationSolutionImpactValuesCalculator original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationSolutionImpactValuesCalculator(this, cloner);
    }
    [StorableConstructor]
    protected SymbolicClassificationSolutionImpactValuesCalculator(bool deserializing) : base(deserializing) { }

    protected override double CalculateQualityForImpacts(ISymbolicDataAnalysisModel model, IDataAnalysisProblemData problemData, IEnumerable<int> rows) {
      var classificationModel = (ISymbolicClassificationModel)model;
      var classificationProblemData = (IClassificationProblemData)problemData;
      OnlineCalculatorError errorState;
      var dataset = problemData.Dataset;
      classificationModel.RecalculateModelParameters(classificationProblemData, rows);
      var targetClassValues = dataset.GetDoubleValues(classificationProblemData.TargetVariable, rows);
      var originalClassValues = classificationModel.GetEstimatedClassValues(dataset, rows);
      var qualityForImpactsCalculation = OnlineAccuracyCalculator.Calculate(targetClassValues, originalClassValues, out errorState);
      if (errorState != OnlineCalculatorError.None) qualityForImpactsCalculation = 0.0;
      return qualityForImpactsCalculation;
    }
  }
}
