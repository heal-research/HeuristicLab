#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

/* 
 * C# port of the freeware implementation of the Mersenne Twister
 * originally developed by M. Matsumoto and T. Nishimura
 * 
 * M. Matsumoto and T. Nishimura,
 * "Mersenne Twister: A 623-Dimensionally Equidistributed Uniform
 * Pseudo-Random Number Generator",
 * ACM Transactions on Modeling and Computer Simulation,
 * Vol. 8, No. 1, January 1998, pp 3-30.
 * 
 */

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Random {
  /// <summary>
  /// A 623-Dimensionally Equidistributed Uniform Pseudo-Random Number Generator.
  /// </summary>
  [Item("MersenneTwister", "A high-quality pseudo random number generator which creates uniformly distributed random numbers.")]
  [StorableClass]
  public sealed class MersenneTwister : Item, IRandom {
    private const int n = 624, m = 397;

    private object locker = new object();
    [Storable]
    private uint[] state = new uint[n];
    [Storable]
    private int p = 0;
    [Storable]
    private bool init = false;

    /// <summary>
    /// Used by HeuristicLab.Persistence to initialize new instances during deserialization.
    /// </summary>
    /// <param name="deserializing">true, if the constructor is called during deserialization.</param>
    [StorableConstructor]
    private MersenneTwister(bool deserializing) : base(deserializing) { }
    /// <summary>
    /// Initializes a new instance from an existing one (copy constructor).
    /// </summary>
    /// <param name="original">The original <see cref="MersenneTwister"/> instance which is used to initialize the new instance.</param>
    /// <param name="cloner">A <see cref="Cloner"/> which is used to track all already cloned objects in order to avoid cycles.</param>
    private MersenneTwister(MersenneTwister original, Cloner cloner)
      : base(original, cloner) {
      state = (uint[])original.state.Clone();
      p = original.p;
      init = original.init;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="MersenneTwister"/>.
    /// </summary>
    public MersenneTwister() {
      if (!init) Seed((uint)DateTime.Now.Ticks);
      init = true;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="MersenneTwister"/> 
    /// with the given seed <paramref name="s"/>.
    /// </summary>
    /// <param name="s">The seed with which to initialize the random number generator.</param>
    public MersenneTwister(uint s) {
      Seed(s);
      init = true;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="MersenneTwister"/> with the given seed array.
    /// </summary>
    /// <param name="array">The seed array with which to initialize the random number generator.</param>
    public MersenneTwister(uint[] array) {
      Seed(array);
      init = true;
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="MersenneTwister"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MersenneTwister(this, cloner);
    }

    /// <summary>
    /// Resets the current random number generator.
    /// </summary>
    public void Reset() {
      lock (locker)
        Seed((uint)DateTime.Now.Ticks);
    }
    /// <summary>
    /// Resets the current random number generator with the given seed <paramref name="s"/>.
    /// </summary>
    /// <param name="s">The seed with which to reset the current instance.</param>
    public void Reset(int s) {
      lock (locker)
        Seed((uint)s);
    }

    /// <summary>
    /// Gets a new random number.
    /// </summary>
    /// <returns>A new int random number.</returns>
    public int Next() {
      lock (locker) {
        return (int)(rand_int32() >> 1);
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
        int limit = (Int32.MaxValue / maxVal) * maxVal;
        int value = Next();
        while (value >= limit) value = Next();
        return value % maxVal;
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
        return Next(maxVal - minVal) + minVal;
      }
    }
    /// <summary>
    /// Gets a new double random variable.
    /// </summary>
    /// <returns></returns>
    public double NextDouble() {
      lock (locker) {
        return rand_double53();
      }
    }

    #region Seed Methods
    /// <summary>
    /// Initializes current instance with random seed.
    /// </summary>
    /// <param name="s">A starting seed.</param>
    public void Seed(uint s) {
      state[0] = s & 0xFFFFFFFFU;
      for (int i = 1; i < n; ++i) {
        state[i] = 1812433253U * (state[i - 1] ^ (state[i - 1] >> 30)) + (uint)i;
        state[i] &= 0xFFFFFFFFU;
      }
      p = n;
    }
    /// <summary>
    /// Initializes current instance with random seed.
    /// </summary>
    /// <param name="array">A starting seed array.</param>
    public void Seed(uint[] array) {
      Seed(19650218U);
      int i = 1, j = 0;
      for (int k = ((n > array.Length) ? n : array.Length); k > 0; --k) {
        state[i] = (state[i] ^ ((state[i - 1] ^ (state[i - 1] >> 30)) * 1664525U))
          + array[j] + (uint)j;
        state[i] &= 0xFFFFFFFFU;
        ++j;
        j %= array.Length;
        if ((++i) == n) { state[0] = state[n - 1]; i = 1; }
      }
      for (int k = n - 1; k > 0; --k) {
        state[i] = (state[i] ^ ((state[i - 1] ^ (state[i - 1] >> 30)) * 1566083941U)) - (uint)i;
        state[i] &= 0xFFFFFFFFU;
        if ((++i) == n) { state[0] = state[n - 1]; i = 1; }
      }
      state[0] = 0x80000000U;
      p = n;
    }
    #endregion

    #region Random Number Generation Methods
    private uint rand_int32() {
      if (p == n) gen_state();
      uint x = state[p++];
      x ^= (x >> 11);
      x ^= (x << 7) & 0x9D2C5680U;
      x ^= (x << 15) & 0xEFC60000U;
      return x ^ (x >> 18);
    }
    private double rand_double() { // interval [0, 1)
      return ((double)rand_int32()) * (1.0 / 4294967296.0);
    }
    private double rand_double_closed() { // interval [0, 1]
      return ((double)rand_int32()) * (1.0 / 4294967295.0);
    }
    private double rand_double_open() { // interval (0, 1)
      return (((double)rand_int32()) + 0.5) * (1.0 / 4294967296.0);
    }
    private double rand_double53() { // 53 bit resolution, interval [0, 1)
      return (((double)(rand_int32() >> 5)) * 67108864.0 +
        ((double)(rand_int32() >> 6))) * (1.0 / 9007199254740992.0);
    }
    #endregion

    #region Private Helper Methods
    private uint twiddle(uint u, uint v) {
      return (((u & 0x80000000U) | (v & 0x7FFFFFFFU)) >> 1)
        ^ (((v & 1U) != 0) ? 0x9908B0DFU : 0x0U);
    }
    private void gen_state() {
      for (int i = 0; i < (n - m); ++i)
        state[i] = state[i + m] ^ twiddle(state[i], state[i + 1]);
      for (int i = n - m; i < (n - 1); ++i)
        state[i] = state[i + m - n] ^ twiddle(state[i], state[i + 1]);
      state[n - 1] = state[m - 1] ^ twiddle(state[n - 1], state[0]);
      p = 0; // reset position
    }
    #endregion
  }
}
