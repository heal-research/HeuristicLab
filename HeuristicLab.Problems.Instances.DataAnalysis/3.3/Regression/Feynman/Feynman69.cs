using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman69 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman69() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman69(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman69(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.15.4 -mom*B*cos(theta) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "E_n" : "E_n_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "mom", "B", "theta", "E_n" } : new[] { "mom", "B", "theta", "E_n", "E_n_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"mom", "B", "theta"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var mom   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var B     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var E_n = new List<double>();

      data.Add(mom);
      data.Add(B);
      data.Add(theta);
      data.Add(E_n);

      for (var i = 0; i < mom.Count; i++) {
        var res = -mom[i] * B[i] * Math.Cos(theta[i]);
        E_n.Add(res);
      }

      var targetNoise = GetNoisyTarget(E_n, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}