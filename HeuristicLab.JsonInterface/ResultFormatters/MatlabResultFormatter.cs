using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.JsonInterface {
  public class MatlabResultFormatter : SymbolicRegressionSolutionFormatterBase {
    protected override ISymbolicExpressionTreeStringFormatter SymbolicExpressionTreeStringFormatter 
      => new SymbolicDataAnalysisExpressionMATLABFormatter();
  }
}
