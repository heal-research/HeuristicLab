#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [StorableClass]
  [Item("AccuracyMaximizingThresholdsModelCreator", "")]
  public sealed class AccuracyMaximizingThresholdsModelCreator : Item, ISymbolicDiscriminantFunctionClassificationModelCreator {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Method; }
    }
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Method; }
    }
    [StorableConstructor]
    private AccuracyMaximizingThresholdsModelCreator(bool deserializing) : base(deserializing) { }
    private AccuracyMaximizingThresholdsModelCreator(AccuracyMaximizingThresholdsModelCreator original, Cloner cloner) : base(original, cloner) { }
    public AccuracyMaximizingThresholdsModelCreator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) { return new AccuracyMaximizingThresholdsModelCreator(this, cloner); }


    public ISymbolicClassificationModel CreateSymbolicClassificationModel(string targetVariable, ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue) {
      return CreateSymbolicDiscriminantFunctionClassificationModel(targetVariable, tree, interpreter, lowerEstimationLimit, upperEstimationLimit);
    }
    public ISymbolicDiscriminantFunctionClassificationModel CreateSymbolicDiscriminantFunctionClassificationModel(string targetVariable, ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter, double lowerEstimationLimit = double.MinValue, double upperEstimationLimit = double.MaxValue) {
      return new SymbolicDiscriminantFunctionClassificationModel(targetVariable, tree, interpreter, new AccuracyMaximizationThresholdCalculator(), lowerEstimationLimit, upperEstimationLimit);
    }

  }
}
