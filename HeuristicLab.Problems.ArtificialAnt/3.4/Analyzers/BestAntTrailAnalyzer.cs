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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ArtificialAnt.Analyzers {
  /// <summary>
  /// An operator for analyzing the best ant trail of an artificial ant problem.
  /// </summary>
  [Item("BestAntTrailAnalyzer", "An operator for analyzing the best ant trail of an artificial ant problem.")]
  [StorableClass]
  public sealed class BestAntTrailAnalyzer : SingleSuccessorOperator, IAntTrailAnalyzer {
    public bool EnabledByDefault {
      get { return true; }
    }

    public ILookupParameter<BoolMatrix> WorldParameter {
      get { return (ILookupParameter<BoolMatrix>)Parameters["World"]; }
    }
    public ScopeTreeLookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ScopeTreeLookupParameter<SymbolicExpressionTree>)Parameters["SymbolicExpressionTree"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<IntValue> MaxTimeStepsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["MaxTimeSteps"]; }
    }
    public ILookupParameter<AntTrail> BestSolutionParameter {
      get { return (ILookupParameter<AntTrail>)Parameters["BestSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public BestAntTrailAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolMatrix>("World", "The world with food items for the artificial ant."));
      Parameters.Add(new ScopeTreeLookupParameter<SymbolicExpressionTree>("SymbolicExpressionTree", "The artificial ant solutions from which the best solution should be visualized."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the artificial ant solutions which should be visualized."));
      Parameters.Add(new LookupParameter<AntTrail>("BestSolution", "The visual representation of the best ant trail."));
      Parameters.Add(new LookupParameter<IntValue>("MaxTimeSteps", "The maximal time steps that the artificial ant has available to collect all food items."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best artificial ant solution should be stored."));
    }

    [StorableConstructor]
    private BestAntTrailAnalyzer(bool deserializing) : base(deserializing) { }
    private BestAntTrailAnalyzer(BestAntTrailAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAntTrailAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      ItemArray<SymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      BoolMatrix world = WorldParameter.ActualValue;
      IntValue maxTimeSteps = MaxTimeStepsParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => -x.Value).First().index;

      AntTrail antTrail = BestSolutionParameter.ActualValue;
      if (antTrail == null) {
        var bestAntTrail = new AntTrail(world, expressions[i], maxTimeSteps);
        BestSolutionParameter.ActualValue = bestAntTrail;
        results.Add(new Result("Best Artificial Ant Solution", bestAntTrail));
      } else {
        antTrail.World = world;
        antTrail.SymbolicExpressionTree = expressions[i];
        antTrail.MaxTimeSteps = maxTimeSteps;
        results["Best Artificial Ant Solution"].Value = antTrail;
      }
      return base.Apply();
    }
  }
}
