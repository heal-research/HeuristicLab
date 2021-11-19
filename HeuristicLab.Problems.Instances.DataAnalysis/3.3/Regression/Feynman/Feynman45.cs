using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman45 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman45() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman45(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman45(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.43.16 mu_drift*q*Volt/d | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "v" : "v_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "mu_drift", "q", "Volt", "d", "v" } : new[] { "mu_drift", "q", "Volt", "d", "v", "v_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"mu_drift", "q", "Volt", "d"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data     = new List<List<double>>();
      var mu_drift = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var q        = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var Volt     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var d        = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var v = new List<double>();

      data.Add(mu_drift);
      data.Add(q);
      data.Add(Volt);
      data.Add(d);
      data.Add(v);

      for (var i = 0; i < mu_drift.Count; i++) {
        var res = mu_drift[i] * q[i] * Volt[i] / d[i];
        v.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(v, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}