using HeuristicLab.Common;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HEAL.Attic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  [Item(Name = "RunCollection SymbolicRegressionSolution Python Formatter")]
  [StorableType("844F2887-B7A0-4BD4-89B8-F9155C65D214")]
  public class RunCollectionSRSolutionPythonFormatter : RunCollectionSRSolutionFormatter {
    protected override ISymbolicExpressionTreeStringFormatter Formatter =>
      new SymbolicDataAnalysisExpressionPythonFormatter();

    #region Constructors & Cloning
    [StorableConstructor]
    protected RunCollectionSRSolutionPythonFormatter(StorableConstructorFlag _) : base(_) { }
    public RunCollectionSRSolutionPythonFormatter() { Suffix = "Python"; }
    public RunCollectionSRSolutionPythonFormatter(RunCollectionSRSolutionPythonFormatter original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionSRSolutionPythonFormatter(this, cloner);
    }
    #endregion
  }
}
