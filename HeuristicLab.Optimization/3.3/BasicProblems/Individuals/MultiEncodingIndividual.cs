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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  public sealed class MultiEncodingIndividual : Individual {
    private new MultiEncoding Encoding {
      get { return (MultiEncoding)base.Encoding; }
    }

    private readonly IEnumerable<Individual> individuals;

    public MultiEncodingIndividual(MultiEncoding encoding, IScope scope)
      : base(encoding, scope) {
      individuals = encoding.Encodings.Select(e => e.GetIndividual(scope)).ToArray();
    }

    private MultiEncodingIndividual(MultiEncoding encoding, IScope scope, IEnumerable<Individual> individuals)
      : base(encoding, scope) {
      this.individuals = individuals;
    }


    public override IItem this[string name] {
      get {
        var individual = individuals.SingleOrDefault(i => i.Name == name);
        if (individual == null) throw new ArgumentException(string.Format("{0} is not part of the specified encoding.", name));
        return individual[name];
      }
      set {
        var individual = individuals.SingleOrDefault(i => i.Name == name);
        if (individual == null) throw new ArgumentException(string.Format("{0} is not part of the specified encoding.", name));
        individual[name] = value;
      }
    }

    public override TEncoding GetEncoding<TEncoding>() {
      TEncoding encoding;
      try {
        encoding = (TEncoding)Encoding.Encodings.SingleOrDefault(e => e is TEncoding);
      } catch (InvalidOperationException) {
        throw new InvalidOperationException(string.Format("The individual uses multiple {0} .", typeof(TEncoding).GetPrettyName()));
      }
      if (encoding == null) throw new InvalidOperationException(string.Format("The individual does not use a {0}.", typeof(TEncoding).GetPrettyName()));
      return encoding;
    }

    public override Individual CopyToScope(IScope scope) {
      var copies = individuals.Select(i => i.CopyToScope(scope)).ToArray();
      return new MultiEncodingIndividual(Encoding, scope, copies);
    }
  }
}
