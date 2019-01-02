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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Optimization {
  [StorableType("13a86161-9e5c-442b-ab1c-18018b1d9708")]
  public sealed class SingleEncodingIndividual : Individual {
    [StorableConstructor]
    private SingleEncodingIndividual(StorableConstructorFlag _) : base(_) {
    }

    public SingleEncodingIndividual(IEncoding encoding, IScope scope)
      : base(encoding, scope) {
      if (encoding is MultiEncoding) throw new ArgumentException("A MultiEncoding must not be used for the creation of a SingleEncodingIndividual");
      if (!scope.Variables.ContainsKey(encoding.Name)) throw new ArgumentException("The provided scope does not contain an individual.");
    }

    private SingleEncodingIndividual(SingleEncodingIndividual copy) : base(copy.Encoding, new Scope()) {
      copy.CopyToScope(Scope);
    }
    public override Individual Copy() {
      return new SingleEncodingIndividual(this);
    }

    public override TEncoding GetEncoding<TEncoding>() {
      TEncoding encoding = Encoding as TEncoding;
      if (encoding == null) throw new InvalidOperationException(string.Format("The individual does not use a {0}.", typeof(TEncoding).GetPrettyName()));
      return encoding;
    }
  }

}
