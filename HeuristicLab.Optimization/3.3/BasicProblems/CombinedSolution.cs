#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  [Item("CombinedSolution", "A solution that consists of other solutions.")]
  [StorableType("B5ED00CB-E533-4ED6-AB2D-95BF7A654AAD")]
  public sealed class CombinedSolution : Item, IEncodedSolution {

    private CombinedEncoding Encoding { get; set; }
    private IScope Scope { get; set; }

    [StorableConstructor]
    private CombinedSolution(StorableConstructorFlag _) : base(_) { }
    private CombinedSolution(CombinedSolution original, Cloner cloner)
      : base(original, cloner) {
      Encoding = cloner.Clone(original.Encoding);
      Scope = cloner.Clone(original.Scope);
    }
    public CombinedSolution(IScope scope, CombinedEncoding encoding) {
      Encoding = encoding;
      Scope = scope;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CombinedSolution(this, cloner);
    }

    public IEncodedSolution this[string name] {
      get { return ScopeUtil.GetEncodedSolution(Scope, name); }
      set { ScopeUtil.CopyEncodedSolutionToScope(Scope, name, value); }
    }

    public TEncoding GetEncoding<TEncoding>() where TEncoding : IEncoding {
      TEncoding encoding;
      try {
        encoding = (TEncoding)Encoding.Encodings.SingleOrDefault(e => e is TEncoding);
      } catch (InvalidOperationException) {
        throw new InvalidOperationException(string.Format("The solution uses multiple {0} .", typeof(TEncoding).GetPrettyName()));
      }
      if (encoding == null) throw new InvalidOperationException(string.Format("The solution does not use a {0}.", typeof(TEncoding).GetPrettyName()));
      return encoding;
    }

    public TEncodedSolution GetEncodedSolution<TEncodedSolution>(string name) where TEncodedSolution : class, IEncodedSolution {
      return (TEncodedSolution)ScopeUtil.GetEncodedSolution(Scope, name);
    }
  }
}
