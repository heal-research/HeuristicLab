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
    private ValueParameter<IntValue> SeedParameter {
      get { return (ValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private ValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (ValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    private ValueParameter<MultiAnalyzer> AnalyzerParameter {
      get { return (ValueParameter<MultiAnalyzer>)Parameters["Analyzer"]; }
    }
    #endregion

    #region Properties
    public IntValue Seed {
      get { return SeedParameter.Value; }
      set { SeedParameter.Value = value; }
    }
    public BoolValue SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value; }
      set { SetSeedRandomlyParameter.Value = value; }
    }
    public MultiAnalyzer Analyzer {
      get { return AnalyzerParameter.Value; }
      set { AnalyzerParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private $safeitemname$(bool deserializing) : base(deserializing) { }
    public $safeitemname$()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new ValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<MultiAnalyzer>("Analyzer", "The operator used to analyze each iteration.", new MultiAnalyzer()));
      
      // TODO: Create and assign OperatorGraph.InitialOperator

      // TODO: Build operator graph
      
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
