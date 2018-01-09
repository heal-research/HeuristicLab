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
using System.Linq;
using System.Security.Cryptography;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Binary;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.NK {
  [Item("NK Landscape", "Represents an NK landscape optimization problem.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 215)]
  [StorableClass]
  public sealed class NKLandscape : BinaryProblem {
    public override bool Maximization {
      get { return false; }
    }

    #region Parameters
    public IValueParameter<BoolMatrix> GeneInteractionsParameter {
      get { return (IValueParameter<BoolMatrix>)Parameters["GeneInteractions"]; }
    }
    public IValueParameter<IntValue> SeedParameter {
      get { return (IValueParameter<IntValue>)Parameters["ProblemSeed"]; }
    }
    public IValueParameter<IntValue> InteractionSeedParameter {
      get { return (IValueParameter<IntValue>)Parameters["InteractionSeed"]; }
    }
    public IValueParameter<IntValue> NrOfInteractionsParameter {
      get { return (IValueParameter<IntValue>)Parameters["NrOfInteractions"]; }
    }
    public IValueParameter<IntValue> NrOfFitnessComponentsParameter {
      get { return (IValueParameter<IntValue>)Parameters["NrOfFitnessComponents"]; }
    }
    public IValueParameter<IntValue> QParameter {
      get { return (IValueParameter<IntValue>)Parameters["Q"]; }
    }
    public IValueParameter<DoubleValue> PParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["P"]; }
    }
    public IValueParameter<DoubleArray> WeightsParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["Weights"]; }
    }
    public IConstrainedValueParameter<IInteractionInitializer> InteractionInitializerParameter {
      get { return (IConstrainedValueParameter<IInteractionInitializer>)Parameters["InteractionInitializer"]; }
    }
    public IConstrainedValueParameter<IWeightsInitializer> WeightsInitializerParameter {
      get { return (IConstrainedValueParameter<IWeightsInitializer>)Parameters["WeightsInitializer"]; }
    }
    #endregion

    #region Properties
    public IInteractionInitializer InteractionInitializer {
      get { return InteractionInitializerParameter.Value; }
    }
    public BoolMatrix GeneInteractions {
      get { return GeneInteractionsParameter.Value; }
    }
    public DoubleArray Weights {
      get { return WeightsParameter.Value; }
    }
    public IntValue InteractionSeed {
      get { return InteractionSeedParameter.Value; }
    }
    public IntValue NrOfFitnessComponents {
      get { return NrOfFitnessComponentsParameter.Value; }
    }
    public IntValue NrOfInteractions {
      get { return NrOfInteractionsParameter.Value; }
    }
    public IWeightsInitializer WeightsInitializer {
      get { return WeightsInitializerParameter.Value; }
    }
    public int Q {
      get { return QParameter.Value.Value; }
    }
    public double P {
      get { return PParameter.Value.Value; }
    }
    public IntValue Seed {
      get { return SeedParameter.Value; }
    }
    #endregion

    [Storable]
    private MersenneTwister random;

    [ThreadStatic]
    private static HashAlgorithm hashAlgorithm;

    [ThreadStatic]
    private static HashAlgorithm hashAlgorithmP;

    private static HashAlgorithm HashAlgorithm {
      get {
        if (hashAlgorithm == null) {
          hashAlgorithm = HashAlgorithm.Create("MD5");
        }
        return hashAlgorithm;
      }
    }

    private static HashAlgorithm HashAlgorithmP {
      get {
        if (hashAlgorithmP == null) {
          hashAlgorithmP = HashAlgorithm.Create("SHA1");
        }
        return hashAlgorithmP;
      }
    }

    [StorableConstructor]
    private NKLandscape(bool deserializing) : base(deserializing) { }
    private NKLandscape(NKLandscape original, Cloner cloner)
      : base(original, cloner) {
      random = (MersenneTwister)original.random.Clone(cloner);
      RegisterEventHandlers();
    }
    public NKLandscape()
      : base() {
      random = new MersenneTwister();

      Parameters.Add(new ValueParameter<BoolMatrix>("GeneInteractions", "Every column gives the participating genes for each fitness component."));
      Parameters.Add(new ValueParameter<IntValue>("ProblemSeed", "The seed used for the random number generator.", new IntValue(0)));
      random.Reset(Seed.Value);

      Parameters.Add(new ValueParameter<IntValue>("InteractionSeed", "The seed used for the hash function to generate interaction tables.", new IntValue(random.Next())));
      Parameters.Add(new ValueParameter<IntValue>("NrOfFitnessComponents", "Number of fitness component functions. (nr of columns in the interaction column)", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("NrOfInteractions", "Number of genes interacting with each other. (nr of True values per column in the interaction matrix)", new IntValue(3)));
      Parameters.Add(new ValueParameter<IntValue>("Q", "Number of allowed fitness values in the (virutal) random table, or zero.", new IntValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("P", "Probability of any entry in the (virtual) random table being zero.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<DoubleArray>("Weights", "The weights for the component functions. If shorted, will be repeated.", new DoubleArray(new[] { 1.0 })));
      Parameters.Add(new ConstrainedValueParameter<IInteractionInitializer>("InteractionInitializer", "Initialize interactions within the component functions."));
      Parameters.Add(new ConstrainedValueParameter<IWeightsInitializer>("WeightsInitializer", "Operator to initialize the weights distribution."));

      //allow just the standard NK[P,Q] formulations at the moment
      WeightsParameter.Hidden = true;
      InteractionInitializerParameter.Hidden = true;
      WeightsInitializerParameter.Hidden = true;
      EncodingParameter.Hidden = true;

      InitializeInteractionInitializerParameter();
      InitializeWeightsInitializerParameter();

      InitializeOperators();
      InitializeInteractions();
      RegisterEventHandlers();
    }

    private void InitializeInteractionInitializerParameter() {
      foreach (var initializer in ApplicationManager.Manager.GetInstances<IInteractionInitializer>())
        InteractionInitializerParameter.ValidValues.Add(initializer);
      InteractionInitializerParameter.Value = InteractionInitializerParameter.ValidValues.First(v => v is RandomInteractionsInitializer);
    }

    private void InitializeWeightsInitializerParameter() {
      foreach (var initializer in ApplicationManager.Manager.GetInstances<IWeightsInitializer>())
        WeightsInitializerParameter.ValidValues.Add(initializer);
      WeightsInitializerParameter.Value = WeightsInitializerParameter.ValidValues.First(v => v is EqualWeightsInitializer);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NKLandscape(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      NrOfInteractionsParameter.ValueChanged += InteractionParameter_ValueChanged;
      NrOfInteractionsParameter.Value.ValueChanged += InteractionParameter_ValueChanged;
      NrOfFitnessComponentsParameter.ValueChanged += InteractionParameter_ValueChanged;
      NrOfFitnessComponentsParameter.Value.ValueChanged += InteractionParameter_ValueChanged;
      InteractionInitializerParameter.ValueChanged += InteractionParameter_ValueChanged;
      WeightsInitializerParameter.ValueChanged += WeightsInitializerParameter_ValueChanged;
      SeedParameter.ValueChanged += SeedParameter_ValueChanged;
      SeedParameter.Value.ValueChanged += SeedParameter_ValueChanged;

      RegisterInteractionInitializerParameterEvents();
      RegisterWeightsParameterEvents();
    }

    private void RegisterWeightsParameterEvents() {
      foreach (var vv in WeightsInitializerParameter.ValidValues) {
        foreach (var p in vv.Parameters) {
          if (p.ActualValue != null && p.ActualValue is IStringConvertibleValue) {
            var v = (IStringConvertibleValue)p.ActualValue;
            v.ValueChanged += WeightsInitializerParameter_ValueChanged;
          }
        }
      }
    }

    private void RegisterInteractionInitializerParameterEvents() {
      foreach (var vv in InteractionInitializerParameter.ValidValues) {
        foreach (var p in vv.Parameters) {
          if (p.ActualValue != null && p.ActualValue is IStringConvertibleValue) {
            var v = (IStringConvertibleValue)p.ActualValue;
            v.ValueChanged += InteractionParameter_ValueChanged;
          } else if (p.ActualValue != null && p is IConstrainedValueParameter<IBinaryVectorComparer>) {
            ((IConstrainedValueParameter<IBinaryVectorComparer>)p).ValueChanged +=
              InteractionParameter_ValueChanged;
          }
        }
      }
    }

    protected override void LengthParameter_ValueChanged(object sender, EventArgs e) {
      NrOfFitnessComponentsParameter.Value = new IntValue(Length);
    }

    private void SeedParameter_ValueChanged(object sender, EventArgs e) {
      random.Reset(Seed.Value);
      InteractionSeed.Value = random.Next();
      InitializeInteractions();
    }

    private void WeightsInitializerParameter_ValueChanged(object sender, EventArgs e) {
      InitializeWeights();
    }

    private void InteractionParameter_ValueChanged(object sender, EventArgs e) {
      InitializeInteractions();
    }

    private void InitializeOperators() {
      NKBitFlipMoveEvaluator nkEvaluator = new NKBitFlipMoveEvaluator();
      Encoding.ConfigureOperator(nkEvaluator);
      Operators.Add(nkEvaluator);
    }

    private void InitializeInteractions() {
      if (InteractionInitializer != null)
        GeneInteractionsParameter.Value = InteractionInitializer.InitializeInterations(
          Length,
          NrOfFitnessComponents.Value,
          NrOfInteractions.Value, random);
    }

    private void InitializeWeights() {
      if (WeightsInitializerParameter.Value != null)
        WeightsParameter.Value = new DoubleArray(
          WeightsInitializer.GetWeights(NrOfFitnessComponents.Value)
          .ToArray());
    }

    #region Evaluation function
    private static long Hash(long x, HashAlgorithm hashAlg) {
      return BitConverter.ToInt64(hashAlg.ComputeHash(BitConverter.GetBytes(x), 0, 8), 0);
    }

    public static double F_i(long x, long i, long g_i, long seed, int q, double p) {
      var hash = new Func<long, long>(y => Hash(y, HashAlgorithm));
      var fi = Math.Abs((double)hash((x & g_i) ^ hash(g_i ^ hash(i ^ seed)))) / long.MaxValue;
      if (q > 0) { fi = Math.Round(fi * q) / q; }
      if (p > 0) {
        hash = y => Hash(y, HashAlgorithmP);
        var r = Math.Abs((double)hash((x & g_i) ^ hash(g_i ^ hash(i ^ seed)))) / long.MaxValue;
        fi = (r <= p) ? 0 : fi;
      }
      return fi;
    }

    private static double F(long x, long[] g, double[] w, long seed, ref double[] f_i, int q, double p) {
      double value = 0;
      for (int i = 0; i < g.Length; i++) {
        f_i[i] = F_i(x, i, g[i], seed, q, p);
        value += w[i % w.Length] * f_i[i];
      }
      return value;
    }

    public static long Encode(BinaryVector v) {
      long x = 0;
      for (int i = 0; i < 64 && i < v.Length; i++) {
        x |= (v[i] ? (long)1 : (long)0) << i;
      }
      return x;
    }

    public static long[] Encode(BoolMatrix m) {
      long[] x = new long[m.Columns];
      for (int c = 0; c < m.Columns; c++) {
        x[c] = 0;
        for (int r = 0; r < 64 && r < m.Rows; r++) {
          x[c] |= (m[r, c] ? (long)1 : (long)0) << r;
        }
      }
      return x;
    }

    public static double[] Normalize(DoubleArray weights) {
      double sum = 0;
      double[] w = new double[weights.Length];
      foreach (var v in weights) {
        sum += Math.Abs(v);
      }
      for (int i = 0; i < weights.Length; i++) {
        w[i] = Math.Abs(weights[i]) / sum;
      }
      return w;
    }

    public static double Evaluate(BinaryVector vector, BoolMatrix interactions, DoubleArray weights, int seed, out double[] f_i, int q, double p) {
      long x = Encode(vector);
      long[] g = Encode(interactions);
      double[] w = Normalize(weights);
      f_i = new double[interactions.Columns];
      return F(x, g, w, (long)seed, ref f_i, q, p);
    }

    public override double Evaluate(BinaryVector vector, IRandom random) {
      double[] f_i; //useful for debugging
      double quality = Evaluate(vector, GeneInteractions, Weights, InteractionSeed.Value, out f_i, Q, P);
      return quality;
    }
    #endregion
  }
}
