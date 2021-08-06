using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.JsonInterface {
  public class PythonResultFormatter : SymbolicRegressionSolutionFormatterBase {
    protected override ISymbolicExpressionTreeStringFormatter SymbolicExpressionTreeStringFormatter
      => new SymbolicDataAnalysisExpressionPythonFormatter();
  }
}
