#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.ArtificialAnt {
  /// <summary>
  /// An operator for visualizing the best ant trail of an artificial ant problem.
  /// </summary>
  [Item("BestAntTrailVisualizer", "An operator for visualizing the best ant trail of an artificial ant problem.")]
  [StorableClass]
  public sealed class BestAntTrailVisualizer : SingleSuccessorOperator, IAntTrailVisualizer {
    public ILookupParameter<BoolMatrix> WorldParameter {
      get { return (ILookupParameter<BoolMatrix>)Parameters["World"]; }
    }
    public ILookupParameter<ItemArray<SymbolicExpressionTree>> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ItemArray<SymbolicExpressionTree>>)Parameters["SymbolicExpressionTree"]; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["Quality"]; }
    }
    public ILookupParameter<IntValue> MaxTimeStepsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["MaxTimeSteps"]; }
    }

    public ILookupParameter<AntTrail> AntTrailParameter {
      get { return (ILookupParameter<AntTrail>)Parameters["AntTrail"]; }
    }
    ILookupParameter ISolutionsVisualizer.VisualizationParameter {
      get { return AntTrailParameter; }
    }

    public BestAntTrailVisualizer()
      : base() {
      Parameters.Add(new LookupParameter<BoolMatrix>("World", "The world with food items for the artificial ant."));
      Parameters.Add(new SubScopesLookupParameter<SymbolicExpressionTree>("SymbolicExpressionTree", "The artificial ant solutions from which the best solution should be visualized."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The qualities of the artificial ant solutions which should be visualized."));
      Parameters.Add(new LookupParameter<AntTrail>("AntTrail", "The visual representation of the best ant trail."));
      Parameters.Add(new LookupParameter<IntValue>("MaxTimeSteps", "The maximal time steps that the artificial ant has available to collect all food items."));
    }

    public override IOperation Apply() {
      ItemArray<SymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      BoolMatrix world = WorldParameter.ActualValue;
      IntValue maxTimeSteps = MaxTimeStepsParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => -x.Value).First().index;

      AntTrail antTrail = AntTrailParameter.ActualValue;
      if (antTrail == null) AntTrailParameter.ActualValue = new AntTrail(world, expressions[i], maxTimeSteps);
      else {
        antTrail.World = world;
        antTrail.SymbolicExpressionTree = expressions[i];
        antTrail.MaxTimeSteps = maxTimeSteps;
      }
      return base.Apply();
    }
  }
}
