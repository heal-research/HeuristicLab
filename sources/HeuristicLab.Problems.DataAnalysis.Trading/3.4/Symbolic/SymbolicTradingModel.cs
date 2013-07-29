#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Trading {
  /// <summary>
  /// Represents a symbolic trading model
  /// </summary>
  [StorableClass]
  [Item(Name = "SymbolicTradingModel", Description = "Represents a symbolic trading model.")]
  public class SymbolicTradingModel : SymbolicDataAnalysisModel, ISymbolicTradingModel {

    [StorableConstructor]
    protected SymbolicTradingModel(bool deserializing) : base(deserializing) { }
    protected SymbolicTradingModel(SymbolicTradingModel original, Cloner cloner)
      : base(original, cloner) { }
    public SymbolicTradingModel(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter)
      : base(tree, interpreter) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicTradingModel(this, cloner);
    }

    public IEnumerable<double> GetSignals(Dataset dataset, IEnumerable<int> rows) {
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter = Interpreter;
      ISymbolicExpressionTree tree = SymbolicExpressionTree;
      double signal = 0;
      double prevSignal = 0;
      foreach (var x in interpreter.GetSymbolicExpressionTreeValues(tree, dataset, rows)) {
        if (prevSignal == 1) {
          signal = x > -1 ? 1 : x > -2 ? 0 : -1; // 0, -1, -2
        } else if (prevSignal == 0) {
          signal = x > 1 ? 1 : x < -1 ? -1 : 0; // 1, 0 , -1
        } else if (prevSignal == -1) {
          signal = x < 1 ? -1 : x < 2 ? 0 : 1; // 2, 1, 0
        }
        yield return signal;
        prevSignal = signal;
      }
    }
  }
}
