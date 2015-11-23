using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  public interface ISymbolicDataAnalysisSolutionImpactValuesCalculator : IItem {
    double CalculateReplacementValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows);
    double CalculateImpactValue(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData, IEnumerable<int> rows, double qualityForImpactsCalculation = double.NaN);
    void CalculateImpactAndReplacementValues(ISymbolicDataAnalysisModel model, ISymbolicExpressionTreeNode node, IDataAnalysisProblemData problemData,
      IEnumerable<int> rows, out double impactValue, out double replacementValue, out double newQualityForImpactsCalculation, double qualityForImpactsCalculation = double.NaN);
  }
}
