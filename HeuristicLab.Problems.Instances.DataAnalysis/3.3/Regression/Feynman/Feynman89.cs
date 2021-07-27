using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman89 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman89() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman89(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman89(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("III.8.54 sin(E_n*t/h)**2 | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "prob" : "prob_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "E_n", "t", "h", "prob" } : new[] { "E_n", "t", "h", "prob", "prob_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"E_n", "t", "h"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var E_n  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var t    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var h    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 4).ToList();

      var prob = new List<double>();

      data.Add(E_n);
      data.Add(t);
      data.Add(h);
      data.Add(prob);

      for (var i = 0; i < E_n.Count; i++) {
        var res = Math.Pow(Math.Sin(E_n[i] * t[i] / h[i]), 2);
        prob.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(prob, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}