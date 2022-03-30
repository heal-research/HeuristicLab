using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus11 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus11() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus11(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus11(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Goldstein 3.99: sqrt(1+2*epsilon**2*E_n*L**2/(m*(Z_1*Z_2*q**2)**2)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "alpha" : "alpha_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "epsilon", "L", "m", "Z_1", "Z_2", "q", "E_n", "alpha" } : new[] { "epsilon", "L", "m", "Z_1", "Z_2", "q", "E_n", "alpha", "alpha_noise" }; }
    }

    protected override string[] AllowedInputVariables {
      get { return new[] {"epsilon", "L", "m", "Z_1", "Z_2", "q", "E_n"}; }
    }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var L       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var m       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var Z_1     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var Z_2     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var q       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var E_n     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var alpha = new List<double>();

      data.Add(epsilon);
      data.Add(L);
      data.Add(m);
      data.Add(Z_1);
      data.Add(Z_2);
      data.Add(q);
      data.Add(E_n);
      data.Add(alpha);

      for (var i = 0; i < epsilon.Count; i++) {
        var res = Math.Sqrt(1 + 2 * Math.Pow(epsilon[i], 2) * E_n[i] * Math.Pow(L[i], 2) /
                            (m[i] * Math.Pow(Z_1[i] * Z_2[i] * Math.Pow(q[i], 2), 2)));
        alpha.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(alpha, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}