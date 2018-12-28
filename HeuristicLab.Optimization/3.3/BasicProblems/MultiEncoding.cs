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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization {
  [Item("MultiEncoding", "Describes a combined encoding consisting of multiple simpler encodings.")]
  [StorableType("359E2173-4D0C-40E5-A2F3-E42E59840345")]
  public sealed class MultiEncoding : Encoding<MultiEncodingCreator> {

    private readonly List<IEncoding> encodings;
    [Storable]
    public IEnumerable<IEncoding> Encodings {
      get { return encodings; }
      private set { encodings.AddRange(value); }
    }

    [StorableConstructor]
    private MultiEncoding(StorableConstructorFlag _) : base(_) {
      encodings = new List<IEncoding>();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new MultiEncoding(this, cloner); }
    private MultiEncoding(MultiEncoding original, Cloner cloner)
      : base(original, cloner) {
      encodings = new List<IEncoding>(original.Encodings.Select(cloner.Clone));
    }
    public MultiEncoding()
      : base("MultiEncoding") {
      encodings = new List<IEncoding>();
      SolutionCreator = new MultiEncodingCreator();

      foreach (var @operator in ApplicationManager.Manager.GetInstances<IMultiEncodingOperator>())
        AddOperator(@operator);
    }

    public override Individual GetIndividual(IScope scope) {
      return new MultiEncodingIndividual(this, scope);
    }

    public MultiEncoding Add(IEncoding encoding) {
      if (encoding is MultiEncoding) throw new InvalidOperationException("Nesting of MultiEncodings is not supported.");
      if (Encodings.Any(e => e.Name == encoding.Name)) throw new ArgumentException("Encoding name must be unique", "encoding.Name");
      encodings.Add(encoding);

      Parameters.AddRange(encoding.Parameters);

      foreach (var @operator in Operators.OfType<IMultiEncodingOperator>()) {
        @operator.AddEncoding(encoding);
      }
      OnEncodingsChanged();
      return this;
    }

    public bool Remove(IEncoding encoding) {
      var success = encodings.Remove(encoding);
      Parameters.RemoveRange(encoding.Parameters);
      foreach (var @operator in Operators.OfType<IMultiEncodingOperator>()) {
        @operator.RemoveEncoding(encoding);
      }
      OnEncodingsChanged();
      return success;
    }

    public override void ConfigureOperators(IEnumerable<IOperator> operators) {
    }

    public event EventHandler EncodingsChanged;
    private void OnEncodingsChanged() {
      var handler = EncodingsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
