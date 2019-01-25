using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableType("b00d5149-fa66-4096-ba46-3258cbd1f42b")]
  public interface ISymbolicDataAnalysisSolutionImpactValuesCalculator : IItem {
    void CalculateImpactAndReplacementValues(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData,
      IEnumerable<int> rows, out double impactValue, out double replacementValue, out double newQualityForImpactsCalculation, double qualityForImpactsCalculation = double.NaN);
  }
}
