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

using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using System.Collections.Generic;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.VarProMRGP {
  [Item("AlleleFrequencyAnalyser", "")]
  [StorableType("E2D97D01-10DD-4A08-9390-0A3EEB04AE9E")]
  public sealed class AlleleFrequencyAnalyzer : AlleleFrequencyAnalyzer<BinaryVector> {
    public ILookupParameter<ReadOnlyItemArray<StringValue>> FeaturesParameter => (ILookupParameter<ReadOnlyItemArray<StringValue>>)Parameters["Features"];

    [StorableConstructor]
    private AlleleFrequencyAnalyzer(StorableConstructorFlag _) : base(_) { }
    private AlleleFrequencyAnalyzer(AlleleFrequencyAnalyzer orig, Cloner cloner) : base(orig, cloner) { }
    public AlleleFrequencyAnalyzer() : base() {
      Parameters.Add(new LookupParameter<ReadOnlyItemArray<StringValue>>("Features", ""));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AlleleFrequencyAnalyzer(this, cloner);
    }

    protected override Allele[] CalculateAlleles(BinaryVector binVector) {
      var allele = new List<Allele>();
      var features = FeaturesParameter.ActualValue;
      for (int i = 0; i < binVector.Length; i++) {
        if (binVector[i])
          allele.Add(new Allele(features[i].Value, 0.0));
      }
      return allele.ToArray();
    }
  }
}
