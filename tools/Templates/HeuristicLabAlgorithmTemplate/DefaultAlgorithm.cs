#region License Information
/* HeuristicLab
 * Copyright (C) 2002-$year$ Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Random;

namespace $rootnamespace$ {
  /// <summary>
  /// A genetic algorithm.
  /// </summary>
  [Item("$algorithmName$", "$algorithmDescription$")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class $safeitemname$ : EngineAlgorithm {
    #region Problem Properties
    $problemType$
    #endregion

    #region Parameter Properties
    $parameterProperties$
    #endregion

    #region Properties
    $properties$
    private RandomCreator RandomCreator {
      get { return (RandomCreator)OperatorGraph.InitialOperator; }
    }
    #endregion

    [StorableConstructor]
    private $safeitemname$(bool deserializing) : base(deserializing) { }
    public $safeitemname$()
      : base() {
      $parameterInitializers$
      
      RandomCreator randomCreator = new RandomCreator();
      OperatorGraph.InitialOperator = randomCreator;

      randomCreator.RandomParameter.ActualName = "Random";
      randomCreator.SeedParameter.ActualName = SeedParameter.Name;
      randomCreator.SeedParameter.Value = null;
      randomCreator.SetSeedRandomlyParameter.ActualName = SetSeedRandomlyParameter.Name;
      randomCreator.SetSeedRandomlyParameter.Value = null;
      randomCreator.Successor = null; // TODO: 

      // TODO: Create further operators and build operator graph
      
      UpdateAnalyzers();
      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      $safeitemname$ clone = ($safeitemname$)base.Clone(cloner);
      // TODO: IMPORTANT! Clone necessary private fields here
      clone.AttachEventHandlers();
      return clone;
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    #region Events
    protected override void OnProblemChanged() {
      // TODO: Initialize and parameterize operators
      UpdateAnalyzers();
      base.OnProblemChanged();
    }

    protected override void Problem_SolutionCreatorChanged(object sender, EventArgs e) {
      // TODO: Parameterize operators
      base.Problem_SolutionCreatorChanged(sender, e);
    }
    protected override void Problem_EvaluatorChanged(object sender, EventArgs e) {
      // TODO: Parameterize operators
      base.Problem_EvaluatorChanged(sender, e);
    }
    protected override void Problem_OperatorsChanged(object sender, EventArgs e) {
      // TODO: Parameterize operators
      UpdateAnalyzers();
      base.Problem_OperatorsChanged(sender, e);
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AttachEventHandlers() {
      // TODO: Attach event handlers to local parameters
      if (Problem != null) {
        // TODO: Attach event handlers to problem parameters
      }
    }
    private void UpdateAnalyzers() {
      Analyzer.Operators.Clear();
      if (Problem != null) {
        foreach (IAnalyzer analyzer in Problem.Operators.OfType<IAnalyzer>()) {
          foreach (IScopeTreeLookupParameter param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
            param.Depth = 1; // TODO: 0 when GlobalScope = Solution, 1 when GlobalScope = Population, 2 when GlobalScope = MetaPopulation (Islands)
          Analyzer.Operators.Add(analyzer);
        }
      }
      // TODO: Add your own algorithm specific analyzer here (problem analyzer should be added/executed first)
    }
    #endregion
  }
}
