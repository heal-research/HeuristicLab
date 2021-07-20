using System;
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.JsonInterface {
  public abstract class SymbolicRegressionSolutionFormatterBase : ResultFormatter {
    public override int Priority => 5;

    public override bool CanFormatType(Type t) {
      var interfaces = t.GetInterfaces();
      var symRegSolutionType = typeof(ISymbolicRegressionSolution);
      return t == symRegSolutionType || interfaces.Any(x => x == symRegSolutionType);
    }

    protected abstract ISymbolicExpressionTreeStringFormatter SymbolicExpressionTreeStringFormatter { get; }

    public override string Format(object o) => SymbolicExpressionTreeStringFormatter.Format(((ISymbolicRegressionSolution)o).Model.SymbolicExpressionTree);
  }
}
