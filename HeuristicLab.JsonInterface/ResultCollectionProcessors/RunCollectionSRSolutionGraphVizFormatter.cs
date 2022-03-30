//using HEAL.Attic;
//using HeuristicLab.Common;
//using HeuristicLab.Core;
//using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
//using HeuristicLab.Problems.DataAnalysis.Symbolic;

//namespace HeuristicLab.JsonInterface {
//  [Item(Name = "RunCollection SymbolicRegressionSolution GraphViz Formatter")]
//  [StorableType("55A9B87B-65AC-4160-ACA0-53FD1EBB1AB7")]
//  public class RunCollectionSRSolutionGraphVizFormatter : RunCollectionSRSolutionFormatter {
//    protected override ISymbolicExpressionTreeStringFormatter Formatter =>
//      new SymbolicExpressionTreeGraphvizFormatter();

//    #region Constructors & Cloning
//    [StorableConstructor]
//    protected RunCollectionSRSolutionGraphVizFormatter(StorableConstructorFlag _) : base(_) { }
//    public RunCollectionSRSolutionGraphVizFormatter() { Suffix = "GraphViz"; }
//    public RunCollectionSRSolutionGraphVizFormatter(RunCollectionSRSolutionGraphVizFormatter original, Cloner cloner) : base(original, cloner) { }

//    public override IDeepCloneable Clone(Cloner cloner) {
//      return new RunCollectionSRSolutionGraphVizFormatter(this, cloner);
//    }
//    #endregion
//  }
//}
