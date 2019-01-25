#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HEAL.Attic;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [StorableType("B9D90B52-04E2-4B18-A293-160061DAD57F")]
  public sealed class GroupSymbol : Symbol {
    private const int minimumArity = 0;
    private const int maximumArity = 0;

    public override int MinimumArity {
      get { return minimumArity; }
    }
    public override int MaximumArity {
      get { return maximumArity; }
    }

    private ObservableSet<ISymbol> symbols;
    public IObservableSet<ISymbol> SymbolsCollection {
      get { return symbols; }
    }
    [Storable]
    public IEnumerable<ISymbol> Symbols {
      get { return symbols; }
      private set { symbols = new ObservableSet<ISymbol>(value); }
    }

    [StorableConstructor]
    private GroupSymbol(StorableConstructorFlag _) : base(_) { }
    private GroupSymbol(GroupSymbol original, Cloner cloner)
      : base(original, cloner) {
      symbols = new ObservableSet<ISymbol>(original.Symbols.Select(s => cloner.Clone(s)));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GroupSymbol(this, cloner);
    }

    public GroupSymbol() : this("Group Symbol", Enumerable.Empty<ISymbol>()) { }
    public GroupSymbol(string name, IEnumerable<ISymbol> symbols)
      : base(name, "A symbol which groups other symbols") {
      this.symbols = new ObservableSet<ISymbol>(symbols);
      InitialFrequency = 0.0;
    }

    public override IEnumerable<ISymbol> Flatten() {
      return base.Flatten().Union(symbols.SelectMany(s => s.Flatten()));
    }
  }
}
