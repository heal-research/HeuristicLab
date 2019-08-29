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
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization {
  [Item("CombinedEncoding", "Describes a combined encoding consisting of multiple simpler encodings.")]
  [StorableType("359E2173-4D0C-40E5-A2F3-E42E59840345")]
  public sealed class CombinedEncoding : Encoding<CombinedSolution> {

    private ItemCollection<IEncoding> encodings;

    [Storable]
    private IEnumerable<IEncoding> StorableEncodings {
      get { return encodings; }
      set { encodings = new ItemCollection<IEncoding>(value); }
    }

    public ReadOnlyItemCollection<IEncoding> Encodings {
      get { return encodings.AsReadOnly(); }
    }

    [StorableConstructor]
    private CombinedEncoding(StorableConstructorFlag _) : base(_) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new CombinedEncoding(this, cloner); }
    private CombinedEncoding(CombinedEncoding original, Cloner cloner)
      : base(original, cloner) {
      encodings = new ItemCollection<IEncoding>(original.Encodings.Select(cloner.Clone));
    }
    public CombinedEncoding()
      : base("CombinedEncoding") {
      encodings = new ItemCollection<IEncoding>();
      SolutionCreator = new MultiEncodingCreator() { SolutionParameter = { ActualName = Name } };
      foreach (var @operator in ApplicationManager.Manager.GetInstances<IMultiEncodingOperator>()) {
        @operator.SolutionParameter.ActualName = Name;
        AddOperator(@operator);
      }
    }

    public CombinedEncoding Add(IEncoding encoding) {
      if (encoding is CombinedEncoding) throw new InvalidOperationException("Nesting of CombinedEncodings is not supported.");
      if (Encodings.Any(e => e.Name == encoding.Name)) throw new ArgumentException("Encoding name must be unique", "encoding.Name");
      encodings.Add(encoding);
      Parameters.AddRange(encoding.Parameters);

      foreach (var @operator in Operators.OfType<IMultiEncodingOperator>())
        @operator.AddEncoding(encoding);

      return this;
    }

    public bool Remove(IEncoding encoding) {
      var success = encodings.Remove(encoding);
      Parameters.RemoveRange(encoding.Parameters);

      foreach (var @operator in Operators.OfType<IMultiEncodingOperator>())
        @operator.RemoveEncoding(encoding);

      return success;
    }

    public void Clear() {
      foreach (var enc in encodings.ToList()) Remove(enc);
    }

    public override void ConfigureOperators(IEnumerable<IItem> operators) {
      foreach (var encOp in operators.OfType<IMultiEncodingOperator>())
        encOp.SolutionParameter.ActualName = Name;
    }
  }
}
