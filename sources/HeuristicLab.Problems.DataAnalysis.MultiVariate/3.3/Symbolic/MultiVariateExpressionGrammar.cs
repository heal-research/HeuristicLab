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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Symbolic {
  [StorableClass]
  [Item("MultiVariateExpressionGrammar", "Represents a grammar for multi-variate expressions using all available functions.")]
  public class MultiVariateExpressionGrammar : DefaultSymbolicExpressionGrammar {

    [Storable]
    private int dimension;
    public int Dimension {
      get { return dimension; }
    }

    public MultiVariateExpressionGrammar() : this(2) { }

    public MultiVariateExpressionGrammar(int dimension)
      : base() {
      this.dimension = dimension;
    }

    protected MultiVariateExpressionGrammar(MultiVariateExpressionGrammar original)
      : base(original) {
      this.dimension = original.dimension;
    }

    public void SetDimension(int n) {
      dimension = n;
      SetMinSubtreeCount(StartSymbol, n);
      SetMaxSubtreeCount(StartSymbol, n);

      foreach (Symbol s in Symbols) {
        if (s != StartSymbol)
          for (int i = 0; i < n; i++) {
            SetAllowedChild(StartSymbol, s, i);
          }
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      var clone = new MultiVariateExpressionGrammar(this);
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }
  }
}
