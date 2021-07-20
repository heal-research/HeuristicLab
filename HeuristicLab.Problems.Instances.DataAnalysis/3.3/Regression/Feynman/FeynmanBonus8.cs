using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus8 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus8() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus8(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus8(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Goldstein 3.55: m*k_G/L**2*(1+sqrt(1+2*E_n*L**2/(m*k_G**2))*cos(theta1-theta2)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "k" : "k_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "m", "k_G", "L", "E_n", "theta1", "theta2", "k" } : new[] { "m", "k_G", "L", "E_n", "theta1", "theta2", "k", "k_noise" }; }
    }

    protected override string[] AllowedInputVariables {
      get { return new[] {"m", "k_G", "L", "E_n", "theta1", "theta2"}; }
    }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data   = new List<List<double>>();
      var m      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var k_G    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var L      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var E_n    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var theta1 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 6).ToList();
      var theta2 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 6).ToList();

      var k = new List<double>();

      data.Add(m);
      data.Add(k_G);
      data.Add(L);
      data.Add(E_n);
      data.Add(theta1);
      data.Add(theta2);
      data.Add(k);

      for (var i = 0; i < m.Count; i++) {
        var res = m[i] * k_G[i] / Math.Pow(L[i], 2) *
                  (1 + Math.Sqrt(1 + 2 * E_n[i] * Math.Pow(L[i], 2) / (m[i] * Math.Pow(k_G[i], 2))) *
                   Math.Cos(theta1[i] - theta2[i]));
        k.Add(res);
      }

      var targetNoise = GetNoisyTarget(k, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}