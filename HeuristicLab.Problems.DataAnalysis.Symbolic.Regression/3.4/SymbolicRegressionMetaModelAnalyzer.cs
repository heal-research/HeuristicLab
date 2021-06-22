using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [StorableType("E5B7E058-E63F-4059-A87F-EA9247592254")]
  public abstract class SymbolicRegressionMetaModelAnalyzer<T> 
    : SymbolicDataAnalysisAnalyzer, ISymbolicExpressionTreeAnalyzer where T: class, IRegressionProblem {

    #region constants
    private const string ProblemsToAnalyzeParameterName = "Problems to Analyze";
    private const string BaseProblemParameterName = "Base Problem";
    private const string AnalyzeXIterationParameterName = "Analyze X Iteration";
    #endregion

    #region parameter properties
    //IRegressionProblem
    public IFixedValueParameter<ItemList<T>> ProblemsToAnalyzeParameter =>
      (IFixedValueParameter<ItemList<T>>)Parameters[ProblemsToAnalyzeParameterName];

    public IValueParameter<T> BaseProblemParameter =>
      (IValueParameter<T>)Parameters[BaseProblemParameterName];

    public IFixedValueParameter<IntValue> AnalyzeXIterationParameter =>
      (IFixedValueParameter<IntValue>)Parameters[AnalyzeXIterationParameterName];
    #endregion

    public static int Iterations { get; set; } = 1;

    #region constructors and cloning
    protected SymbolicRegressionMetaModelAnalyzer(SymbolicRegressionMetaModelAnalyzer<T> original, Cloner cloner) :
      base(original, cloner) { }

    [StorableConstructor]
    protected SymbolicRegressionMetaModelAnalyzer(StorableConstructorFlag _) : base(_) { }

    public SymbolicRegressionMetaModelAnalyzer() {
      Parameters.Add(new FixedValueParameter<ItemList<T>>(ProblemsToAnalyzeParameterName,
        "List of datasets, which are used to find a meta model.", new ItemList<T>()));
      Parameters.Add(new ValueParameter<T>(BaseProblemParameterName,
        "The problem which uses the algorithm (just drag&drop it)."));
      Parameters.Add(new FixedValueParameter<IntValue>(AnalyzeXIterationParameterName,
        "After every X iteration, the analyzer will perform its step.", new IntValue(1)));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(ProblemsToAnalyzeParameterName))
        Parameters.Add(new FixedValueParameter<ItemList<T>>(ProblemsToAnalyzeParameterName,
          "List of datasets, which are used to find a meta model.", new ItemList<T>()));

      if (!Parameters.ContainsKey(BaseProblemParameterName))
        Parameters.Add(new ValueParameter<T>(BaseProblemParameterName,
          "The problem which uses the algorithm (just drag&drop it)."));

      if (!Parameters.ContainsKey(AnalyzeXIterationParameterName))
        Parameters.Add(new FixedValueParameter<IntValue>(AnalyzeXIterationParameterName,
          "After every X iteration, the analyzer will perform its step.", new IntValue(1)));
    }

    #endregion

    public override void InitializeState() {
      Iterations = 1;
      base.InitializeState();
    }

    public override void ClearState() {
      Iterations = 1;
      base.ClearState();
    }

    //protected abstract bool TryFittingSolution(ISymbolicExpressionTree tree, T problem, out SymbolicRegressionSolution solution);
    protected abstract void PerformApply(T baseProblem, IEnumerable<T> problems, string targetVariable);

    public override IOperation Apply() {
      if (BaseProblemParameter.Value == null)
        throw new ArgumentNullException("BaseProblemParameter",
          "Base Problem Parameter is null! Please drag&drop the used problem into the parameter slot.");

      if (Iterations >= AnalyzeXIterationParameter.Value.Value) {
        Iterations = 1;
        var baseProblem = BaseProblemParameter.Value;
        var problems = ProblemsToAnalyzeParameter.Value;
        var targetVariable = BaseProblemParameter.Value.ProblemData.TargetVariable;

        // error handling
        var badProblems = problems.Where(x => x.ProblemData.TargetVariable != targetVariable);
        IList<Exception> errors = new List<Exception>();
        foreach (var problem in badProblems)
          errors.Add(new ArgumentException($"The target variable of the problem '{problem.Name}' does not match with the base problem."));
        if (badProblems.Any())
          throw new AggregateException(errors);

        // apply
        PerformApply(baseProblem, problems, targetVariable);
      } else {
        Iterations++;
      }
      return base.Apply();
    }
  }
}
