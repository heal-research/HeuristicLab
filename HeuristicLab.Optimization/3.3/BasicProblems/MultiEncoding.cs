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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization {
  [Item("MultiEncoding", "Describes a combined encoding consisting of multiple simpler encodings.")]
  [StorableClass]
  public sealed class MultiEncoding : Encoding<CombinedSolution> {

    private readonly ItemCollection<IEncoding> encodings;

    [Storable]
    private IEnumerable<IEncoding> StorableEncodings {
      get { return encodings; }
      set { encodings.AddRange(value); }
    }

    public ReadOnlyItemCollection<IEncoding> Encodings {
      get { return encodings.AsReadOnly(); }
    }

    [StorableConstructor]
    private MultiEncoding(bool deserializing)
      : base(deserializing) {
      encodings = new ItemCollection<IEncoding>();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new MultiEncoding(this, cloner); }
    private MultiEncoding(MultiEncoding original, Cloner cloner)
      : base(original, cloner) {
      encodings = new ItemCollection<IEncoding>(original.Encodings.Select(cloner.Clone));
    }
    public MultiEncoding()
      : base("MultiEncoding") {
      encodings = new ItemCollection<IEncoding>();
      SolutionCreator = new MultiEncodingCreator() { Encoding = this };
      foreach (var @operator in ApplicationManager.Manager.GetInstances<IMultiEncodingOperator>()) {
        @operator.Encoding = this;
        AddOperator(@operator);
      }
    }

    public MultiEncoding Add(IEncoding encoding) {
      if (encoding is MultiEncoding) throw new InvalidOperationException("Nesting of MultiEncodings is not supported.");
      if (Encodings.Any(e => e.Name == encoding.Name)) throw new ArgumentException("Encoding name must be unique", "encoding.Name");
      encodings.Add(encoding);
      Parameters.AddRange(encoding.Parameters);
      return this;
    }

    public bool Remove(IEncoding encoding) {
      var success = encodings.Remove(encoding);
      Parameters.RemoveRange(encoding.Parameters);
      return success;
    }

    public override void ConfigureOperators(IEnumerable<IOperator> operators) { }
  }
}
