using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman34 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman34() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman34(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman34(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.34.8 q*v*B/p | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "omega" : "omega_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "q", "v", "B", "p", "omega" } : new[] { "q", "v", "B", "p", "omega", "omega_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"q", "v", "B", "p"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var q    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var v    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var B    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var p    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var omega = new List<double>();

      data.Add(q);
      data.Add(v);
      data.Add(B);
      data.Add(p);
      data.Add(omega);

      for (var i = 0; i < q.Count; i++) {
        var res = q[i] * v[i] * B[i] / p[i];
        omega.Add(res);
      }

      var targetNoise = GetNoisyTarget(omega, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}