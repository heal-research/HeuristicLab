#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("ec7201c7-3c9f-4a93-966b-d848d9538202")]
  public sealed class MultiEncodingIndividual : Individual {
    private new MultiEncoding Encoding {
      get { return (MultiEncoding)base.Encoding; }
    }

    [StorableConstructor]
    private MultiEncodingIndividual(StorableConstructorFlag _) : base(_) {
    }

    public MultiEncodingIndividual(MultiEncoding encoding, IScope scope)
      : base(encoding, scope) { }

    private MultiEncodingIndividual(MultiEncodingIndividual copy) : base(copy.Encoding, new Scope()) {
      copy.CopyToScope(Scope);
    }
    public override Individual Copy() {
      return new MultiEncodingIndividual(this);
    }

    public override TEncoding GetEncoding<TEncoding>() {
      TEncoding encoding;
      try {
        encoding = (TEncoding)Encoding.Encodings.SingleOrDefault(e => e is TEncoding);
      }
      catch (InvalidOperationException) {
        throw new InvalidOperationException(string.Format("The individual uses multiple {0} .", typeof(TEncoding).GetPrettyName()));
      }
      if (encoding == null) throw new InvalidOperationException(string.Format("The individual does not use a {0}.", typeof(TEncoding).GetPrettyName()));
      return encoding;
    }
  }
}
