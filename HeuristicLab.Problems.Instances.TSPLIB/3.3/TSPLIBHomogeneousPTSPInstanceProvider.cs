#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.Linq;

namespace HeuristicLab.Problems.Instances.TSPLIB {
  public class TSPLIBHomogeneousPTSPInstanceProvider : TSPLIBPTSPInstanceProvider {
    private static readonly double[] Probabilities = new[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };

    public override string Name {
      get { return "TSPLIB (homogeneous symmetric PTSP)"; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      var tspDescriptors = base.GetDataDescriptors().OfType<TSPLIBDataDescriptor>();
      foreach (var desc in tspDescriptors) {
        foreach (var p in Probabilities) {
          yield return new TSPLIBHomogeneousPTSPDataDescriptor(
            desc.Name + "-" + p.ToString(CultureInfo.InvariantCulture.NumberFormat),
            desc.Description,
            desc.InstanceIdentifier,
            p == 1.0 ? desc.SolutionIdentifier : null,
            p);
        }
      }
    }

    protected override double[] GetProbabilities(IDataDescriptor descriptor, PTSPData instance) {
      var ptspDesc = descriptor as TSPLIBHomogeneousPTSPDataDescriptor;
      return ptspDesc != null ? Enumerable.Range(0, instance.Dimension).Select(_ => ptspDesc.Probability).ToArray()
                              : Enumerable.Range(0, instance.Dimension).Select(_ => 0.5).ToArray();
    }
  }
}
