#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Trading.Symbolic {
  /// <summary>
  /// Represents a symbolic trading model
  /// </summary>
  [StorableClass]
  [Item(Name = "Model (symbolic trading)", Description = "Represents a symbolic trading model.")]
  public class Model : SymbolicDataAnalysisModel, IModel {

    [StorableConstructor]
    protected Model(bool deserializing) : base(deserializing) { }
    protected Model(Model original, Cloner cloner)
      : base(original, cloner) { }
    public Model(ISymbolicExpressionTree tree, ISymbolicDataAnalysisExpressionTreeInterpreter interpreter)
      : base(tree, interpreter, -10, 10) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Model(this, cloner);
    }

    public IEnumerable<double> GetSignals(Dataset dataset, IEnumerable<int> rows) {
      ISymbolicDataAnalysisExpressionTreeInterpreter interpreter = Interpreter;
      ISymbolicExpressionTree tree = SymbolicExpressionTree;
      return GetSignals(interpreter.GetSymbolicExpressionTreeValues(tree, dataset, rows));
    }

    // Transforms an enumerable of real values to an enumerable of trading signals (buy(1) / hold(0) / sell(-1))
    public static IEnumerable<double> GetSignals(IEnumerable<double> xs) {
      // two iterations over xs
      // 1) determine min / max to calculate the mid-range value
      // 2) range is split into three thirds
      double max = double.NegativeInfinity;
      double min = double.PositiveInfinity;
      foreach (var x in xs) {
        if (x > max) max = x;
        if (x < min) min = x;
      }
      if (double.IsInfinity(max) || double.IsNaN(max) || double.IsInfinity(min) || double.IsNaN(min))
        return xs.Select(x => 0.0);

      double range = (max - min);
      double midRange = range / 2.0 + min;
      double offset = range / 6.0;
      return from x in xs
             select x > midRange + offset ? 1.0 : x < midRange - offset ? -1.0 : 0.0;
    }
  }
}
