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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Random {

  /// <summary>
  /// Normally distributed random variable.
  /// Uses the Ziggurat transform to efficiently generate normal distributed random numbers.
  /// See "The Ziggurat Method for Generating Random Variables" (G. Marsaglia and W.W. Tsang 2000).
  /// </summary>
  [Item("NormalDistributedRandom", "A pseudo random number generator which uses the Ziggurat method to create normally distributed random numbers.")]
  [StorableClass]
  public sealed class NormalDistributedRandom : Item, IRandom {
    [Storable]
    private double mu;
    /// <summary>
    /// Gets or sets the value for µ.
    /// </summary>
    public double Mu {
      get { return mu; }
      set { mu = value; }
    }

    [Storable]
    private double sigma;
    /// <summary>
    /// Gets or sets the value for sigma.
    /// </summary>
    public double Sigma {
      get { return sigma; }
      set { sigma = value; }
    }

    [Storable]
    private IRandom uniform;

    private static double[] w = new double[] {
      1.7290404664e-09,
      1.2680928529e-10,
      1.6897518107e-10,
      1.9862687883e-10,
      2.2232431174e-10,
      2.4244936614e-10,
      2.6016130916e-10,
      2.7611987696e-10,
      2.9073962682e-10,
      3.0429969655e-10,
      3.1699795566e-10,
      3.2898020419e-10,
      3.4035738117e-10,
      3.5121602848e-10,
      3.6162509098e-10,
      3.7164057942e-10,
      3.8130856805e-10,
      3.9066758162e-10,
      3.9975012189e-10,
      4.0858399997e-10,
      4.1719308563e-10,
      4.2559822333e-10,
      4.3381759296e-10,
      4.4186720949e-10,
      4.4976131153e-10,
      4.5751258337e-10,
      4.6513240481e-10,
      4.7263104541e-10,
      4.8001774777e-10,
      4.8730097735e-10,
      4.9448850570e-10,
      5.0158732723e-10,
      5.0860404777e-10,
      5.1554460700e-10,
      5.2241466708e-10,
      5.2921933502e-10,
      5.3596349581e-10,
      5.4265170135e-10,
      5.4928817050e-10,
      5.5587695558e-10,
      5.6242188684e-10,
      5.6892646150e-10,
      5.7539412124e-10,
      5.8182819673e-10,
      5.8823168558e-10,
      5.9460769641e-10,
      6.0095900478e-10,
      6.0728838625e-10,
      6.1359850534e-10,
      6.1989202660e-10,
      6.2617133700e-10,
      6.3243904558e-10,
      6.3869737277e-10,
      6.4494881657e-10,
      6.5119559745e-10,
      6.5744004685e-10,
      6.6368432972e-10,
      6.6993072201e-10,
      6.7618144417e-10,
      6.8243871665e-10,
      6.8870464887e-10,
      6.9498151678e-10,
      7.0127148533e-10,
      7.0757677495e-10,
      7.1389966161e-10,
      7.2024242126e-10,
      7.2660727435e-10,
      7.3299660786e-10,
      7.3941280876e-10,
      7.4585826404e-10,
      7.5233547170e-10,
      7.5884698525e-10,
      7.6539541372e-10,
      7.7198347714e-10,
      7.7861395109e-10,
      7.8528972214e-10,
      7.9201378789e-10,
      7.9878920145e-10,
      8.0561923799e-10,
      8.1250728368e-10,
      8.1945689123e-10,
      8.2647166888e-10,
      8.3355555791e-10,
      8.4071272166e-10,
      8.4794732347e-10,
      8.5526402627e-10,
      8.6266754851e-10,
      8.7016316375e-10,
      8.7775620106e-10,
      8.8545243360e-10,
      8.9325818964e-10,
      9.0117996399e-10,
      9.0922497309e-10,
      9.1740082198e-10,
      9.2571583732e-10,
      9.3417884539e-10,
      9.4279972718e-10,
      9.5158891877e-10,
      9.6055785548e-10,
      9.6971930486e-10,
      9.7908692265e-10,
      9.8867602993e-10,
      9.9850361313e-10,
      1.0085882129e-09,
      1.0189509236e-09,
      1.0296150599e-09,
      1.0406069340e-09,
      1.0519566329e-09,
      1.0636980186e-09,
      1.0758701707e-09,
      1.0885182755e-09,
      1.1016947354e-09,
      1.1154610569e-09,
      1.1298901814e-09,
      1.1450695947e-09,
      1.1611052120e-09,
      1.1781275955e-09,
      1.1962995039e-09,
      1.2158286600e-09,
      1.2369856250e-09,
      1.2601323318e-09,
      1.2857697129e-09,
      1.3146201905e-09,
      1.3477839955e-09,
      1.3870635751e-09,
      1.4357403044e-09,
      1.5008658760e-09,
      1.6030947680e-09
    };
    private static long[] k = new long[] {
      1991057938,
      0,
      1611602771,
      1826899878,
      1918584482,
      1969227037,
      2001281515,
      2023368125,
      2039498179,
      2051788381,
      2061460127,
      2069267110,
      2075699398,
      2081089314,
      2085670119,
      2089610331,
      2093034710,
      2096037586,
      2098691595,
      2101053571,
      2103168620,
      2105072996,
      2106796166,
      2108362327,
      2109791536,
      2111100552,
      2112303493,
      2113412330,
      2114437283,
      2115387130,
      2116269447,
      2117090813,
      2117856962,
      2118572919,
      2119243101,
      2119871411,
      2120461303,
      2121015852,
      2121537798,
      2122029592,
      2122493434,
      2122931299,
      2123344971,
      2123736059,
      2124106020,
      2124456175,
      2124787725,
      2125101763,
      2125399283,
      2125681194,
      2125948325,
      2126201433,
      2126441213,
      2126668298,
      2126883268,
      2127086657,
      2127278949,
      2127460589,
      2127631985,
      2127793506,
      2127945490,
      2128088244,
      2128222044,
      2128347141,
      2128463758,
      2128572095,
      2128672327,
      2128764606,
      2128849065,
      2128925811,
      2128994934,
      2129056501,
      2129110560,
      2129157136,
      2129196237,
      2129227847,
      2129251929,
      2129268426,
      2129277255,
      2129278312,
      2129271467,
      2129256561,
      2129233410,
      2129201800,
      2129161480,
      2129112170,
      2129053545,
      2128985244,
      2128906855,
      2128817916,
      2128717911,
      2128606255,
      2128482298,
      2128345305,
      2128194452,
      2128028813,
      2127847342,
      2127648860,
      2127432031,
      2127195339,
      2126937058,
      2126655214,
      2126347546,
      2126011445,
      2125643893,
      2125241376,
      2124799783,
      2124314271,
      2123779094,
      2123187386,
      2122530867,
      2121799464,
      2120980787,
      2120059418,
      2119015917,
      2117825402,
      2116455471,
      2114863093,
      2112989789,
      2110753906,
      2108037662,
      2104664315,
      2100355223,
      2094642347,
      2086670106,
      2074676188,
      2054300022,
      2010539237
   };
    private static double[] f = new double[] {
      1.0000000000e+00,
      9.6359968185e-01,
      9.3628269434e-01,
      9.1304361820e-01,
      8.9228165150e-01,
      8.7324303389e-01,
      8.5550057888e-01,
      8.3878362179e-01,
      8.2290720940e-01,
      8.0773830414e-01,
      7.9317700863e-01,
      7.7914607525e-01,
      7.6558417082e-01,
      7.5244158506e-01,
      7.3967725039e-01,
      7.2725689411e-01,
      7.1515148878e-01,
      7.0333611965e-01,
      6.9178915024e-01,
      6.8049186468e-01,
      6.6942769289e-01,
      6.5858197212e-01,
      6.4794182777e-01,
      6.3749545813e-01,
      6.2723249197e-01,
      6.1714339256e-01,
      6.0721951723e-01,
      5.9745317698e-01,
      5.8783704042e-01,
      5.7836467028e-01,
      5.6902998686e-01,
      5.5982738733e-01,
      5.5075180531e-01,
      5.4179835320e-01,
      5.3296267986e-01,
      5.2424055338e-01,
      5.1562821865e-01,
      5.0712203979e-01,
      4.9871864915e-01,
      4.9041482806e-01,
      4.8220765591e-01,
      4.7409430146e-01,
      4.6607214212e-01,
      4.5813870430e-01,
      4.5029163361e-01,
      4.4252872467e-01,
      4.3484783173e-01,
      4.2724698782e-01,
      4.1972434521e-01,
      4.1227802634e-01,
      4.0490642190e-01,
      3.9760786295e-01,
      3.9038079977e-01,
      3.8322380185e-01,
      3.7613546848e-01,
      3.6911445856e-01,
      3.6215949059e-01,
      3.5526937246e-01,
      3.4844297171e-01,
      3.4167915583e-01,
      3.3497685194e-01,
      3.2833510637e-01,
      3.2175290585e-01,
      3.1522938609e-01,
      3.0876362324e-01,
      3.0235484242e-01,
      2.9600214958e-01,
      2.8970485926e-01,
      2.8346219659e-01,
      2.7727350593e-01,
      2.7113807201e-01,
      2.6505529881e-01,
      2.5902456045e-01,
      2.5304529071e-01,
      2.4711695313e-01,
      2.4123899639e-01,
      2.3541094363e-01,
      2.2963231802e-01,
      2.2390270233e-01,
      2.1822164953e-01,
      2.1258877218e-01,
      2.0700371265e-01,
      2.0146611333e-01,
      1.9597564638e-01,
      1.9053204358e-01,
      1.8513499200e-01,
      1.7978426814e-01,
      1.7447963357e-01,
      1.6922089458e-01,
      1.6400785744e-01,
      1.5884037316e-01,
      1.5371830761e-01,
      1.4864157140e-01,
      1.4361007512e-01,
      1.3862377405e-01,
      1.3368265331e-01,
      1.2878671288e-01,
      1.2393598258e-01,
      1.1913054436e-01,
      1.1437050998e-01,
      1.0965602100e-01,
      1.0498725623e-01,
      1.0036443919e-01,
      9.5787845552e-02,
      9.1257803142e-02,
      8.6774669588e-02,
      8.2338899374e-02,
      7.7950984240e-02,
      7.3611505330e-02,
      6.9321118295e-02,
      6.5080583096e-02,
      6.0890771449e-02,
      5.6752663106e-02,
      5.2667401731e-02,
      4.8636294901e-02,
      4.4660862535e-02,
      4.0742866695e-02,
      3.6884389818e-02,
      3.3087886870e-02,
      2.9356317595e-02,
      2.5693291798e-02,
      2.2103304043e-02,
      1.8592102453e-02,
      1.5167297795e-02,
      1.1839478277e-02,
      8.6244847625e-03,
      5.5489949882e-03,
      2.6696291752e-03
    };

    /// <summary>
    /// Used by HeuristicLab.Persistence to initialize new instances during deserialization.
    /// </summary>
    /// <param name="deserializing">true, if the constructor is called during deserialization.</param>
    [StorableConstructor]
    private NormalDistributedRandom(bool deserializing) : base(deserializing) { }

    /// <summary>
    /// Initializes a new instance from an existing one (copy constructor).
    /// </summary>
    /// <param name="original">The original <see cref="NormalDistributedRandom"/> instance which is used to initialize the new instance.</param>
    /// <param name="cloner">A <see cref="Cloner"/> which is used to track all already cloned objects in order to avoid cycles.</param>
    private NormalDistributedRandom(NormalDistributedRandom original, Cloner cloner)
      : base(original, cloner) {
      uniform = cloner.Clone(original.uniform);
      mu = original.mu;
      sigma = original.sigma;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NormalDistributedRandom"/> with µ = 0 and sigma = 1
    /// and a new random number generator.
    /// </summary>
    public NormalDistributedRandom() {
      this.mu = 0.0;
      this.sigma = 1.0;
      this.uniform = new MersenneTwister();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NormalDistributedRandom"/> with the given parameters.
    /// <note type="caution"> No CopyConstructor! The random number generator is not copied!</note>
    /// </summary>    
    /// <param name="uniformRandom">The random number generator.</param>
    /// <param name="mu">The value for µ.</param>
    /// <param name="sigma">The value for sigma.</param>
    public NormalDistributedRandom(IRandom uniformRandom, double mu, double sigma) {
      this.mu = mu;
      this.sigma = sigma;
      this.uniform = uniformRandom;
    }

    #region IRandom Members

    /// <inheritdoc cref="IRandom.Reset()"/>
    public void Reset() {
      uniform.Reset();
    }

    /// <inheritdoc cref="IRandom.Reset(int)"/>
    public void Reset(int seed) {
      uniform.Reset(seed);
    }

    /// <summary>
    /// This method is not implemented.
    /// </summary>
    public int Next() {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method is not implemented.
    /// </summary>
    public int Next(int maxVal) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method is not implemented.
    /// </summary>
    public int Next(int minVal, int maxVal) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Generates a new double random number.
    /// </summary>
    /// <returns>A double random number.</returns>
    public double NextDouble() {
      return NormalDistributedRandom.NextDouble(uniform, mu, sigma);
    }

    #endregion

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="cloner.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="NormalDistributedRandom"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      return new NormalDistributedRandom(this, cloner);
    }

    public static double NextDouble(IRandom uniformRandom, double mu, double sigma) {
      double signFactor = uniformRandom.Next() % 2 == 0 ? 1.0 : -1.0;
      return sigma * signFactor * NextPositiveDouble(uniformRandom) + mu;
    }

    private static double NextPositiveDouble(IRandom uniformRandom) {
      int j = uniformRandom.Next();
      int i = (j & 127);
      if (Math.Abs(j) < k[i]) {
        return j * w[i];
      } else {
        double r = 3.442620;
        double x, y;
        x = j * w[i];
        if (i == 0) {
          do {
            x = -Math.Log(ScaledUniform(uniformRandom)) * 0.2904764;
            y = -Math.Log(ScaledUniform(uniformRandom));
          } while (y + y < x * x);
          return (j > 0) ? r + x : -r - x;
        }
        if (f[i] + ScaledUniform(uniformRandom) * (f[i - 1] - f[i]) < Math.Exp(-0.5 * x * x)) {
          return x;
        } else {
          // recurse
          return (NextPositiveDouble(uniformRandom));
        }
      }
    }

    private static double ScaledUniform(IRandom uniformRandom) {
      return 0.5 + uniformRandom.Next() * 0.2328306e-9;
    }
  }
}
