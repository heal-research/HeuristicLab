using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman78 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman78() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman78(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman78(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.34.29a q*h/(4*pi*m) | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "mom" : "mom_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"q", "h", "m", noiseRatio == null ? "mom" : "mom_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"q", "h", "m"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var q    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var h    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var m    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var mom = new List<double>();

      data.Add(q);
      data.Add(h);
      data.Add(m);
      data.Add(mom);

      for (var i = 0; i < q.Count; i++) {
        var res = q[i] * h[i] / (4 * Math.PI * m[i]);
        mom.Add(res);
      }

      if (noiseRatio != null) {
        var mom_noise   = new List<double>();
        var sigma_noise = (double) noiseRatio * mom.StandardDeviationPop();
        mom_noise.AddRange(mom.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(mom);
        data.Add(mom_noise);
      }

      return data;
    }
  }
}