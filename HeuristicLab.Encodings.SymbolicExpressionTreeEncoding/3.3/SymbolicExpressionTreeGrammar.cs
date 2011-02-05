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

using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public sealed class SymbolicExpressionTreeGrammar : DefaultSymbolicExpressionGrammar {
    public SymbolicExpressionTreeGrammar(ISymbolicExpressionGrammar grammar)
      : base(grammar) {
    }
    [StorableConstructor]
    private SymbolicExpressionTreeGrammar(bool deserializing) : base(deserializing) { }
    // don't call storable ctor of base class to prevent full cloning
    // instead use storable ctor to initialize an empty grammar and fill with InizializeShallowClone
    private SymbolicExpressionTreeGrammar(SymbolicExpressionTreeGrammar original, Cloner cloner)
      : base(false) {
      cloner.RegisterClonedObject(original, this);
      InitializeShallowClone(original);
    }
    private SymbolicExpressionTreeGrammar() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeGrammar(this, cloner);
    }
  }
}
