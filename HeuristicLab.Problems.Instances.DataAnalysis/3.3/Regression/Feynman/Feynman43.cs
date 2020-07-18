using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman43 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman43() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman43(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman43(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.40.1 n_0*exp(-m*g*x/(kb*T)) | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "n" : "n_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"n_0", "m", "x", "T", "g", "kb", noiseRatio == null ? "n" : "n_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"n_0", "m", "x", "T", "g", "kb"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var n_0  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var m    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var x    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var T    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var g    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var kb   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var n = new List<double>();

      data.Add(n_0);
      data.Add(m);
      data.Add(x);
      data.Add(T);
      data.Add(g);
      data.Add(kb);
      data.Add(n);

      for (var i = 0; i < n_0.Count; i++) {
        var res = n_0[i] * Math.Exp(-m[i] * g[i] * x[i] / (kb[i] * T[i]));
        n.Add(res);
      }

      if (noiseRatio != null) {
        var n_noise     = new List<double>();
        var sigma_noise = (double) noiseRatio * n.StandardDeviationPop();
        n_noise.AddRange(n.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(n);
        data.Add(n_noise);
      }

      return data;
    }
  }
}