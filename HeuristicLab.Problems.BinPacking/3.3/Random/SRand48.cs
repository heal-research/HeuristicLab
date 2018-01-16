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


using HeuristicLab.Core;
using System;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.BinPacking.Random {
  /* 
   * C# port of the implementation of the srand48 random generator.
   * First implemented in test3dbpp.c by S. Martello, D. Pisinger, D. Vigo, E. den Boef, J. Korst (2003) 
   */
  [Item("SRand", "A pseudo random number generator which creates uniformly distributed random numbers.")]
  [StorableClass]
  public sealed class SRand48 : Item, IRandom {
    private object locker = new object();

    [Storable]
    private bool init = false;
    [Storable]
    private uint _h48;
    [Storable]
    private uint _l48;
    

    /// <summary>
    /// Used by HeuristicLab.Persistence to initialize new instances during deserialization.
    /// </summary>
    /// <param name="deserializing">true, if the constructor is called during deserialization.</param>
    [StorableConstructor]
    private SRand48(bool deserializing) : base(deserializing) { }

    /// <summary>
    /// Initializes a new instance from an existing one (copy constructor).
    /// </summary>
    /// <param name="original">The original <see cref="SRand48"/> instance which is used to initialize the new instance.</param>
    /// <param name="cloner">A <see cref="Cloner"/> which is used to track all already cloned objects in order to avoid cycles.</param>
    private SRand48(SRand48 original, Cloner cloner)
      : base(original, cloner) {
      _h48 = original._h48;
      _l48 = original._l48;
      init = original.init;
    }


    /// <summary>
    /// Initializes a new instance of <see cref="SRand48"/>.
    /// </summary>
    public SRand48() {
      if (!init) {
        Seed((uint)DateTime.Now.Ticks);
        init = true;
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SRand48"/> 
    /// with the given seed <paramref name="seed"/>.
    /// </summary>
    /// <param name="seed">The seed with which to initialize the random number generator.</param>
    public SRand48(uint seed) {
      if (!init) {
        Seed(seed);
        init = true;
      }
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="cloner">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="SRand48"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SRand48(this, cloner);
    }

    /// <summary>
    /// Gets a new random number.
    /// </summary>
    /// <returns>A new int random number.</returns>
    public int Next() {
      lock (locker) {
        return (int)LRand48x();
      }
    }

    /// <summary>
    /// Gets a new random number being smaller than the given <paramref name="maxVal"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the given maximum value is 
    /// smaller or equal to zero.</exception>
    /// <param name="maxVal">The maximum value of the generated random number.</param>
    /// <returns>A new int random number.</returns>
    public int Next(int maxVal) {
      lock (locker) {
        if (maxVal <= 0)
          throw new ArgumentException("The interval [0, " + maxVal + ") is empty");
        return (int)(LRand48x() % maxVal);
      }
    }

    /// <summary>
    /// Gets a new random number being in the given interval <paramref name="minVal"/> and 
    /// <paramref name="maxVal"/>.
    /// </summary>
    /// <param name="minVal">The minimum value of the generated random number.</param>
    /// <param name="maxVal">The maximum value of the generated random number.</param>
    /// <returns>A new int random number.</returns>
    public int Next(int minVal, int maxVal) {
      lock (locker) {
        if (maxVal <= minVal)
          throw new ArgumentException("The interval [" + minVal + ", " + maxVal + ") is empty");
        return Next(maxVal - minVal + 1) + minVal;
      }
    }

    /// <summary>
    /// Gets a new double random variable.
    /// </summary>
    /// <returns></returns>
    public double NextDouble() {
      lock (locker) {
        return ((double)Next()) * (1.0 / 4294967296.0);
      }
    }

    /// <summary>
    /// Resets the current random number generator.
    /// </summary>
    public void Reset() {
      lock (locker) {
        Seed((uint)DateTime.Now.Ticks);
      }
    }

    /// <summary>
    /// Resets the current random number generator with the given seed <paramref name="s"/>.
    /// </summary>
    /// <param name="seed">The seed with which to reset the current instance.</param>
    public void Reset(int seed) {
      lock (locker) {
        Seed((uint)seed);
      }
    }

    /// <summary>
    /// Initializes current instance with random seed.
    /// </summary>
    /// <param name="seed">A starting seed.</param>
    private void Seed(uint seed) {
      _h48 = seed;
      _l48 = 0x330E;
    }

    private int LRand48x() {
      _h48 = (_h48 * 0xDEECE66D) + (_l48 * 0x5DEEC);
      _l48 = _l48 * 0xE66D + 0xB;
      _h48 = _h48 + (_l48 >> 16);
      _l48 = _l48 & 0xFFFF;
      return (int)(_h48 >> 1);
    }
    
  }
}
