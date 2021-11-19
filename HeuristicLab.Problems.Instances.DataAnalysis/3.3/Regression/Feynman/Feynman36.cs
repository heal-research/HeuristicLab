using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman36 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman36() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman36(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman36(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.34.14 (1+v/c)/sqrt(1-v**2/c**2)*omega_0 | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "omega" : "omega_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "c", "v", "omega_0", "omega" } : new[] { "c", "v", "omega_0", "omega", "omega_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"c", "v", "omega_0"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var c       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 3, 10).ToList();
      var v       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var omega_0 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var omega = new List<double>();

      data.Add(c);
      data.Add(v);
      data.Add(omega_0);
      data.Add(omega);

      for (var i = 0; i < c.Count; i++) {
        var res = (1 + v[i] / c[i]) / Math.Sqrt(1 - Math.Pow(v[i], 2) / Math.Pow(c[i], 2)) * omega_0[i];
        omega.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(omega, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}