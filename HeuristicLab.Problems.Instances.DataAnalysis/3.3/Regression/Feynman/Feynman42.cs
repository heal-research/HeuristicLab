using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman42 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman42() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman42(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman42(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.39.22 n*kb*T/V | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "pr" : "pr_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "n", "T", "V", "kb", "pr" } : new[] { "n", "T", "V", "kb", "pr", "pr_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"n", "T", "V", "kb"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var n    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var T    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var V    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var kb   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var pr = new List<double>();

      data.Add(n);
      data.Add(T);
      data.Add(V);
      data.Add(kb);
      data.Add(pr);

      for (var i = 0; i < n.Count; i++) {
        var res = n[i] * kb[i] * T[i] / V[i];
        pr.Add(res);
      }

      var targetNoise = GetNoisyTarget(pr, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}