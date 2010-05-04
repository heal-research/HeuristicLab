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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.ArtificialAnt {
  /// <summary>
  /// An operator for visualizing the best symbolic expression tree of an artificial ant problem.
  /// </summary>
  [Item("BestSymbolicExpressionTreeVisualizer", "An operator for visualizing the best symbolic expression tree of an artificial ant problem.")]
  [StorableClass]
  public sealed class BestSymbolicExpressionTreeVisualizer : SingleSuccessorOperator, IAnalyzer {
    public ILookupParameter<ItemArray<SymbolicExpressionTree>> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<ItemArray<SymbolicExpressionTree>>)Parameters["SymbolicExpressionTree"]; }
    }
    public ILookupParameter<ItemArray<DoubleValue>> QualityParameter {
      get { return (ILookupParameter<ItemArray<DoubleValue>>)Parameters["Quality"]; }
    }
    public ILookupParameter<SymbolicExpressionTree> BestSymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters["BestSymbolicExpressionTree"]; }
    }

    public BestSymbolicExpressionTreeVisualizer()
      : base() {
      Parameters.Add(new SubScopesLookupParameter<SymbolicExpressionTree>("SymbolicExpressionTree", "The artificial ant solutions from which the best solution should be visualized."));
      Parameters.Add(new SubScopesLookupParameter<DoubleValue>("Quality", "The qualities of the artificial ant solutions which should be visualized."));
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>("BestSymbolicExpressionTree", "The best symbolic expression tree of the artificial ant problem."));
    }

    public override IOperation Apply() {
      ItemArray<SymbolicExpressionTree> expressions = SymbolicExpressionTreeParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => -x.Value).First().index;

      SymbolicExpressionTree bestTree = BestSymbolicExpressionTreeParameter.ActualValue;
      if (bestTree == null) BestSymbolicExpressionTreeParameter.ActualValue = (SymbolicExpressionTree)expressions[i].Clone();
      else {
        bestTree.Root = expressions[i].Root;
      }
      return base.Apply();
    }
  }
}
