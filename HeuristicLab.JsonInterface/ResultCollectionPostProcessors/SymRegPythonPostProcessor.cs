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

namespace HeuristicLab.JsonInterface {
  [StorableType("844F2887-B7A0-4BD4-89B8-F9155C65D214")]
  public class SymRegPythonPostProcessor : Item, IResultCollectionPostProcessor {

    [StorableConstructor]
    protected SymRegPythonPostProcessor(StorableConstructorFlag _) : base(_) {
    }

    public SymRegPythonPostProcessor() { }
    public SymRegPythonPostProcessor(SymRegPythonPostProcessor old, Cloner cloner) { }

    public void Apply(IObservableDictionary<string, IItem> results, IDictionary<string, string> output) {
      var formatter = new SymbolicDataAnalysisExpressionPythonFormatter();
      foreach (var kvp in results) {
        if (kvp.Value is ISymbolicRegressionSolution sol) {
          output.Add(kvp.Key, formatter.Format(sol.Model.SymbolicExpressionTree));
        }
      }
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymRegPythonPostProcessor(this, cloner);
    }
  }
}
