using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman80 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman80() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman80(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman80(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.35.18 n_0/(exp(mom*B/(kb*T))+exp(-mom*B/(kb*T))) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "n" : "n_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "n_0", "kb", "T", "mom", "B", "n" } : new[] { "n_0", "kb", "T", "mom", "B", "n", "n_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"n_0", "kb", "T", "mom", "B"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var n_0  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var kb   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var T    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var mom  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var B    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var n = new List<double>();

      data.Add(n_0);
      data.Add(kb);
      data.Add(T);
      data.Add(mom);
      data.Add(B);
      data.Add(n);

      for (var i = 0; i < n_0.Count; i++) {
        var res = n_0[i] / (Math.Exp(mom[i] * B[i] / (kb[i] * T[i])) + Math.Exp(-mom[i] * B[i] / (kb[i] * T[i])));
        n.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(n, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}