using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.JsonInterface {
  public class MatlabResultFormatter : ResultFormatter {
    public override int Priority => 5;

    public override bool CanFormatType(Type t) {
      var interfaces = t.GetInterfaces();
      return t.GetInterfaces().Any(x => x == typeof(ISymbolicRegressionSolution));
    }
      
    private ISymbolicExpressionTreeStringFormatter MatlabFormatter => new SymbolicDataAnalysisExpressionMATLABFormatter();

    public override string Format(object o) => MatlabFormatter.Format((ISymbolicExpressionTree)((ISymbolicRegressionSolution)o).Model.SymbolicExpressionTree);
  }
}
