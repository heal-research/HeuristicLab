//using HEAL.Attic;
//using HeuristicLab.Common;
//using HeuristicLab.Core;
//using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
//using HeuristicLab.Problems.DataAnalysis.Symbolic;

//namespace HeuristicLab.JsonInterface {
//  [Item(Name = "RunCollection SymbolicRegressionSolution Latex Formatter")]
//  [StorableType("89EDF59C-1CD6-4314-8504-EB3A28E6B1C3")]
//  public class RunCollectionSRSolutionLatexFormatter : RunCollectionSRSolutionFormatter {
//    protected override ISymbolicExpressionTreeStringFormatter Formatter =>
//      new SymbolicDataAnalysisExpressionLatexFormatter();

//    #region Constructors & Cloning
//    [StorableConstructor]
//    protected RunCollectionSRSolutionLatexFormatter(StorableConstructorFlag _) : base(_) { }
//    public RunCollectionSRSolutionLatexFormatter() { Suffix = "Latex"; }
//    public RunCollectionSRSolutionLatexFormatter(RunCollectionSRSolutionLatexFormatter original, Cloner cloner) : base(original, cloner) { }

//    public override IDeepCloneable Clone(Cloner cloner) {
//      return new RunCollectionSRSolutionLatexFormatter(this, cloner);
//    }
//    #endregion
//  }
//}
