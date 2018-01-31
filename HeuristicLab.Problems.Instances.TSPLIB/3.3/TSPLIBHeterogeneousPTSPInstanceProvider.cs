#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.TSPLIB {
  public class TSPLIBHeterogeneousPTSPInstanceProvider : TSPLIBPTSPInstanceProvider {

    public override string Name {
      get { return "TSPLIB (heterogeneous symmetric PTSP)"; }
    }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      foreach (var desc in base.GetDataDescriptors().OfType<TSPLIBDataDescriptor>()) {
        desc.SolutionIdentifier = null;
        yield return desc;
      }
    }

    protected override double[] GetProbabilities(IDataDescriptor descriptor, PTSPData instance) {
      var random = new MarsagliaRandom(GetInstanceHash(instance.Name));
      return Enumerable.Range(0, instance.Dimension).Select(_ => (int)Math.Round((0.1 + 0.9 * random.NextDouble()) * 100) / 100.0).ToArray();
    }

    // Bernstein's hash function
    private uint GetInstanceHash(string name) {
      uint hash = 5381;
      var len = name.Length;
      for (var i = 0; i < len; i++)
        unchecked { hash = ((hash * 33) + name[i]); }
      return hash;
    }

    /// <summary>
    /// This class is used to randomly generate PTSP instances given the TSPLIB instances.
    /// An own implementation of a RNG was used in order to avoid possible implementation changes
    /// in future .NET versions which would result in entirely different instances.
    /// </summary>
    /// <remarks>
    /// RNG is implemented according to George Marsaglia https://en.wikipedia.org/wiki/Multiply-with-carry
    /// </remarks>
    private class MarsagliaRandom {
      /*
       * S = 2111111111*X[n-4] + 1492*X[n-3] + 1776*X[n-2] + 5115*X[n-1] + C
       * X[n] = S modulo 2^32
       * C = floor(S / 2^32)
       * 
       */
      private readonly uint[] mem = new uint[4];
      private uint c;

      public MarsagliaRandom(uint s) {
        int i;
        for (i = 0; i < mem.Length; i++) {
          unchecked { s = s * 31294061 + 1; }
          mem[i] = s;
        }
        unchecked { c = s * 31294061 + 1; }
      }

      private uint Next() {
        unchecked {
          ulong wsum = 2111111111 * mem[0]
                      + 1492 * mem[1]
                      + 1776 * mem[2]
                      + 5115 * mem[3]
                      + c;

          mem[0] = mem[1];
          mem[1] = mem[2];
          mem[2] = mem[3];
          mem[3] = (uint)wsum;
          c = (uint)(wsum >> 32);
          return mem[3];
        }
      }

      public double NextDouble() {
        return (double)Next() / uint.MaxValue;
      }
    }
  }
}
