#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    public ILookupParameter<IScope> BestSolutionParameter {
      get { return (ILookupParameter<IScope>)Parameters["BestSolution"]; }
    }
    public ILookupParameter<IScope> BestKnownSolutionParameter {
      get { return (ILookupParameter<IScope>)Parameters["BestKnownSolution"]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    #region Storing & Cloning
    [StorableConstructor]
    protected BestScopeSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected BestScopeSolutionAnalyzer(BestScopeSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestScopeSolutionAnalyzer(this, cloner);
    }
    #endregion
    public BestScopeSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the solutions."));
      Parameters.Add(new LookupParameter<IScope>("BestSolution", "The best solution."));
      Parameters.Add(new LookupParameter<IScope>("BestKnownSolution", "The best known solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the solution should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;

      int i = -1;
      if (!max)
        i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      else i = qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      IEnumerable<IScope> scopes = new IScope[] { ExecutionContext.Scope };
      for (int j = 0; j < QualityParameter.Depth; j++)
        scopes = scopes.Select(x => (IEnumerable<IScope>)x.SubScopes).Aggregate((a, b) => a.Concat(b));
      IScope currentBestScope = scopes.ToList()[i];

      if (bestKnownQuality == null ||
          max && qualities[i].Value > bestKnownQuality.Value
          || !max && qualities[i].Value < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
        BestKnownSolutionParameter.ActualValue = (IScope)currentBestScope.Clone();
      }

      IScope solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = (IScope)currentBestScope.Clone();
        BestSolutionParameter.ActualValue = solution;
        results.Add(new Result("Best Solution", solution));
      } else {
        string qualityName = QualityParameter.TranslatedName;
        if (solution.Variables.ContainsKey(qualityName)) {
          double bestSoFarQuality = (solution.Variables[qualityName].Value as DoubleValue).Value;
          if (max && qualities[i].Value > bestSoFarQuality
            || !max && qualities[i].Value < bestSoFarQuality) {
            solution = (IScope)currentBestScope.Clone();
            BestSolutionParameter.ActualValue = solution;
            results["Best Solution"].Value = solution;
          }
        }
      }

      return base.Apply();
    }
  }
}
