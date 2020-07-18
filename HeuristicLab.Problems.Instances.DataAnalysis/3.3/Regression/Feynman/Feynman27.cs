using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman27 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman27() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman27(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman27(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.27.6 1/(1/d1+n/d2) | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "foc" : "foc_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"d1", "d2", "n", noiseRatio == null ? "foc" : "foc_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"d1", "d2", "n"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var d1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var d2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var n    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var foc = new List<double>();

      data.Add(d1);
      data.Add(d2);
      data.Add(n);
      data.Add(foc);

      for (var i = 0; i < d1.Count; i++) {
        var res = 1.0 / (1.0 / d1[i] + n[i] / d2[i]);
        foc.Add(res);
      }

      if (noiseRatio != null) {
        var foc_noise   = new List<double>();
        var sigma_noise = (double) noiseRatio * foc.StandardDeviationPop();
        foc_noise.AddRange(foc.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(foc);
        data.Add(foc_noise);
      }

      return data;
    }
  }
}