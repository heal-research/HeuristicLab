using System;
using System.Linq;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.JsonInterface {
  public abstract class SymbolicRegressionSolutionFormatterBase : ResultFormatter {
    public override int Priority => 5;

    public override bool CanFormatType(Type t) {
      var interfaces = t.GetInterfaces();
      return t.GetInterfaces().Any(x => x == typeof(ISymbolicRegressionSolution));
    }

    protected abstract ISymbolicExpressionTreeStringFormatter SymbolicExpressionTreeStringFormatter { get; }

    public override string Format(object o) => SymbolicExpressionTreeStringFormatter.Format((ISymbolicExpressionTree)((ISymbolicRegressionSolution)o).Model.SymbolicExpressionTree);
  }
}
