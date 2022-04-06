using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HEAL.Attic;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;

namespace HeuristicLab.JsonInterface {
  [StorableType("806BB409-E50C-4827-ABEF-0376D65F7414")]
  public abstract class RunCollectionSRSolutionFormatter : ParameterizedNamedItem, IRunCollectionModifier {

    #region Constants
    private const string SuffixParameterName = "Suffix";
    #endregion

    #region Parameter Properties
    public IFixedValueParameter<StringValue> SuffixParameter =>
      (IFixedValueParameter<StringValue>)Parameters[SuffixParameterName];
    #endregion

    #region Properties
    public string Suffix {
      get => SuffixParameter.Value.Value;
      set => SuffixParameter.Value.Value = value;
    }
    protected abstract ISymbolicExpressionTreeStringFormatter Formatter { get; }
    #endregion

    #region Constructors & Cloning
    [StorableConstructor]
    protected RunCollectionSRSolutionFormatter(StorableConstructorFlag _) : base(_) { }
    public RunCollectionSRSolutionFormatter() {
      Parameters.Add(new FixedValueParameter<StringValue>(SuffixParameterName, "Appends a suffix to the original name.", new StringValue()));
    }
    public RunCollectionSRSolutionFormatter(RunCollectionSRSolutionFormatter original, Cloner cloner) : base(original, cloner) { }
    #endregion

    public void Modify(List<IRun> runs) {
      foreach (var run in runs) {
        var resultCopy = new ObservableDictionary<string, IItem>(run.Results);
        foreach (var kvp in resultCopy) {
          if (kvp.Value is ISymbolicRegressionSolution sol) {
            run.Results.Add(
              $"{kvp.Key} - {SuffixParameter.Value.Value}",
              new StringValue(Formatter.Format(sol.Model.SymbolicExpressionTree)));
          }
        }
      }
    }
  }
}
