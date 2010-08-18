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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public sealed class SymbolicExpressionTreeGrammar : DefaultSymbolicExpressionGrammar {
    public SymbolicExpressionTreeGrammar(ISymbolicExpressionGrammar grammar)
      : base(grammar) {
    }
    [StorableConstructor]
    private SymbolicExpressionTreeGrammar(bool deserializing) : base(deserializing) { }
    //default ctor for cloning
    private SymbolicExpressionTreeGrammar() : base(false) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      SymbolicExpressionTreeGrammar clone = new SymbolicExpressionTreeGrammar();
      cloner.RegisterClonedObject(this, clone);
      InitializeShallowClone(clone);
      return clone;
    }
  }
}
