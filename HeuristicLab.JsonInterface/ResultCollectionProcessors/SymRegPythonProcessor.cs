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

namespace HeuristicLab.JsonInterface {
  [StorableType("844F2887-B7A0-4BD4-89B8-F9155C65D214")]
  public class SymRegPythonProcessor : ParameterizedNamedItem, IResultCollectionProcessor {

    #region Constructors & Cloning
    [StorableConstructor]
    protected SymRegPythonProcessor(StorableConstructorFlag _) : base(_) { }
    public SymRegPythonProcessor() { }
    public SymRegPythonProcessor(SymRegPythonProcessor original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymRegPythonProcessor(this, cloner);
    }
    #endregion

    public void Apply(IObservableDictionary<string, IItem> results) {
      var formatter = new SymbolicDataAnalysisExpressionPythonFormatter();
      var resultCopy = new ObservableDictionary<string, IItem>(results);
      foreach (var kvp in resultCopy) {
        if (kvp.Value is ISymbolicRegressionSolution sol) {
          results.Add($"{kvp.Key} - Python" , new StringValue(formatter.Format(sol.Model.SymbolicExpressionTree)));
        }
      }
    }
  }
}
