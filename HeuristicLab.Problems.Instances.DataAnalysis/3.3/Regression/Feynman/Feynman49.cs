using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman49 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman49() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman49(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman49(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.47.23 sqrt(gamma*pr/rho) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "c" : "c_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"gamma", "pr", "rho", noiseRatio == null ? "c" : "c_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"gamma", "pr", "rho"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var gamma = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var pr    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var rho   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var c = new List<double>();

      data.Add(gamma);
      data.Add(pr);
      data.Add(rho);
      data.Add(c);

      for (var i = 0; i < gamma.Count; i++) {
        var res = Math.Sqrt(gamma[i] * pr[i] / rho[i]);
        c.Add(res);
      }

      if (noiseRatio != null) {
        var c_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * c.StandardDeviationPop();
        c_noise.AddRange(c.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(c);
        data.Add(c_noise);
      }

      return data;
    }
  }
}