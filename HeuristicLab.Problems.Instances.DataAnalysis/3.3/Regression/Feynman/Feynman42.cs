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
        return string.Format("I.39.22 n*kb*T/V | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "pr" : "pr_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"n", "T", "V", "kb", noiseRatio == null ? "pr" : "pr_noise"}; }
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

      if (noiseRatio != null) {
        var pr_noise    = new List<double>();
        var sigma_noise = (double) noiseRatio * pr.StandardDeviationPop();
        pr_noise.AddRange(pr.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(pr);
        data.Add(pr_noise);
      }

      return data;
    }
  }
}