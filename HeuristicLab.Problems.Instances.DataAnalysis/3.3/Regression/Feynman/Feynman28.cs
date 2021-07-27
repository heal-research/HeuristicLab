using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman28 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman28() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman28(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman28(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.29.4 omega/c | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "k" : "k_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "omega", "c", "k" } : new[] { "omega", "c", "k", "k_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"omega", "c"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var omega = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 10).ToList();
      var c     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 10).ToList();

      var k = new List<double>();

      data.Add(omega);
      data.Add(c);
      data.Add(k);

      for (var i = 0; i < omega.Count; i++) {
        var res = omega[i] / c[i];
        k.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(k, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}