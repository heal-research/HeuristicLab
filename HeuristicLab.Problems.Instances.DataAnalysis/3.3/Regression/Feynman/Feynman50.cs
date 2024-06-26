using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman50 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman50() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman50(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman50(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.48.2 m*c**2/sqrt(1-v**2/c**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "E_n" : "E_n_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "m", "v", "c", "E_n" } : new[] { "m", "v", "c", "E_n", "E_n_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"m", "v", "c"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var m    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var v    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var c    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 3, 10).ToList();

      var E_n = new List<double>();

      data.Add(m);
      data.Add(v);
      data.Add(c);
      data.Add(E_n);

      for (var i = 0; i < m.Count; i++) {
        var res = m[i] * Math.Pow(c[i], 2) / Math.Sqrt(1 - Math.Pow(v[i], 2) / Math.Pow(c[i], 2));
        E_n.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(E_n, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}