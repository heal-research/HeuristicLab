#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// An operator that extracts (clones) the scope containing the best quality.
  /// </summary>
  [Item("BestScopeSolutionAnalyzer", "An operator that extracts the scope containing the best quality.")]
  [StorableClass]
  public class BestScopeSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {

    public virtual bool EnabledByDefault {
      get { return true; }
    }
    public LookupParameter<BoolValue> MaximizationParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IFixedValueParameter<StringValue> BestSolutionResultNameParameter {
      get { return (IFixedValueParameter<StringValue>)Parameters["BestSolution ResultName"]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public string BestSolutionResultName {
      get { return BestSolutionResultNameParameter.Value.Value; }
      set { BestSolutionResultNameParameter.Value.Value = value; }
    }

    #region Storing & Cloning
    [StorableConstructor]
    protected BestScopeSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected BestScopeSolutionAnalyzer(BestScopeSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestScopeSolutionAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("BestSolution ResultName"))
        Parameters.Add(new FixedValueParameter<StringValue>("BestSolution ResultName", "The name of the result for storing the best solution.", new StringValue("Best Solution")));
      if (Parameters.ContainsKey("BestSolution")) Parameters.Remove("BestSolution");
      if (Parameters.ContainsKey("BestKnownSolution")) Parameters.Remove("BestKnownSolution");
      #endregion
    }
    #endregion
    public BestScopeSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the solutions."));
      Parameters.Add(new FixedValueParameter<StringValue>("BestSolution ResultName", "The name of the result for storing the best solution.", new StringValue("Best Solution")));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the solution should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;

      if (results.ContainsKey(BestSolutionResultName) && !typeof(IScope).IsAssignableFrom(results[BestSolutionResultName].DataType)) {
        throw new InvalidOperationException(string.Format("Could not add best solution result, because there is already a result with the name \"{0}\" present in the result collection.", BestSolutionResultName));
      }

      int i = -1;
      if (!max)
        i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      else i = qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      IEnumerable<IScope> scopes = new IScope[] { ExecutionContext.Scope };
      for (int j = 0; j < QualityParameter.Depth; j++)
        scopes = scopes.SelectMany(x => x.SubScopes);
      IScope currentBestScope = scopes.ToList()[i];

      if (bestKnownQuality == null ||
          max && qualities[i].Value > bestKnownQuality.Value
          || !max && qualities[i].Value < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
      }

      if (!results.ContainsKey(BestSolutionResultName)) {
        var cloner = new Cloner();
        //avoid cloning of subscopes and the results collection that the solution is put in
        cloner.RegisterClonedObject(results, new ResultCollection());
        cloner.RegisterClonedObject(currentBestScope.SubScopes, new ScopeList());
        var solution = cloner.Clone(currentBestScope);

        results.Add(new Result(BestSolutionResultName, solution));
      } else {
        var bestSolution = (IScope)results[BestSolutionResultName].Value;
        string qualityName = QualityParameter.TranslatedName;
        if (bestSolution.Variables.ContainsKey(qualityName)) {
          double bestQuality = ((DoubleValue)bestSolution.Variables[qualityName].Value).Value;
          if (max && qualities[i].Value > bestQuality
              || !max && qualities[i].Value < bestQuality) {
            var cloner = new Cloner();
            //avoid cloning of subscopes and the results collection that the solution is put in
            cloner.RegisterClonedObject(results, new ResultCollection());
            cloner.RegisterClonedObject(currentBestScope.SubScopes, new ScopeList());
            var solution = cloner.Clone(currentBestScope);

            results[BestSolutionResultName].Value = solution;
          }
        }
      }
      return base.Apply();
    }
  }
}
