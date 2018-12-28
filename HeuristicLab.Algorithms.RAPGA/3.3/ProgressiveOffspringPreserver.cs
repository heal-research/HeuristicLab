#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Algorithms.RAPGA {
  /// <summary>
  /// An operator that progressively selects offspring by adding it to a scope list.
  /// </summary>
  /// <remarks>
  /// The operator also performs duplication control.
  /// </remarks>
  [Item("ProgressiveOffspringPreserver", "An operator that progressively selects offspring by adding it to a scope list. The operator also performs duplication control.")]
  [StorableType("36A99B15-7DF3-481A-8D76-24BF4ED7B6F8")]
  public sealed class ProgressiveOffspringPreserver : SingleSuccessorOperator {
    #region Parameter Properties
    public ScopeParameter CurrentScopeParameter {
      get { return (ScopeParameter)Parameters["CurrentScope"]; }
    }
    public ILookupParameter<ScopeList> OffspringListParameter {
      get { return (ILookupParameter<ScopeList>)Parameters["OffspringList"]; }
    }
    public ILookupParameter<IntValue> ElitesParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Elites"]; }
    }
    public ILookupParameter<IntValue> MaximumPopulationSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["MaximumPopulationSize"]; }
    }
    public IValueLookupParameter<ISolutionSimilarityCalculator> SimilarityCalculatorParameter {
      get { return (IValueLookupParameter<ISolutionSimilarityCalculator>)Parameters["SimilarityCalculator"]; }
    }
    #endregion

    #region Properties
    private IScope CurrentScope {
      get { return CurrentScopeParameter.ActualValue; }
    }
    private ScopeList OffspringList {
      get { return OffspringListParameter.ActualValue; }
    }
    private IntValue Elites {
      get { return ElitesParameter.ActualValue; }
    }
    private IntValue MaximumPopulationSize {
      get { return MaximumPopulationSizeParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    private ProgressiveOffspringPreserver(StorableConstructorFlag _) : base(_) { }
    private ProgressiveOffspringPreserver(ProgressiveOffspringPreserver original, Cloner cloner) : base(original, cloner) { }
    public ProgressiveOffspringPreserver()
      : base() {
      #region Create parameters
      Parameters.Add(new ScopeParameter("CurrentScope", "The current scope that contains the offspring."));
      Parameters.Add(new LookupParameter<ScopeList>("OffspringList", "The list that contains the offspring."));
      Parameters.Add(new LookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new LookupParameter<IntValue>("MaximumPopulationSize", "The maximum size of the population of solutions."));
      Parameters.Add(new ValueLookupParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The similarity calculator that should be used to calculate solution similarity."));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProgressiveOffspringPreserver(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("SimilarityCalculator"))
        Parameters.Add(new ValueLookupParameter<ISolutionSimilarityCalculator>("SimilarityCalculator", "The similarity calculator that should be used to calculate solution similarity."));
      #endregion
    }

    public override IOperation Apply() {
      if (CurrentScope.SubScopes.Any()) { // offspring created
        if (!OffspringList.Any()) OffspringList.AddRange(CurrentScope.SubScopes);
        else { // stored offspring exists
          var storedOffspringScope = new Scope();
          storedOffspringScope.SubScopes.AddRange(OffspringList);
          var similarityMatrix = SimilarityCalculatorParameter.ActualValue.CalculateSolutionCrowdSimilarity(CurrentScope, storedOffspringScope);

          var createdOffspring = CurrentScope.SubScopes.ToArray();

          int i = 0;
          // as long as offspring is available and not enough offspring has been preserved
          while (i < createdOffspring.Length && OffspringList.Count < MaximumPopulationSize.Value - Elites.Value) {
            if (similarityMatrix[i].Any(x => x.IsAlmost(1.0))) createdOffspring[i] = null; // discard duplicates
            else OffspringList.Add(createdOffspring[i]);
            i++;
          }

          // discard remaining offspring
          while (i < createdOffspring.Length) createdOffspring[i++] = null;

          // clean current scope
          CurrentScope.SubScopes.Replace(createdOffspring.Where(x => x != null));
        }
      }
      return base.Apply();
    }
  }
}