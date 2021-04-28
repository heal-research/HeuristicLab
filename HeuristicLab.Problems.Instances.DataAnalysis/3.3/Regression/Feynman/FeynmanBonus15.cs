using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus15 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus15() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus15(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus15(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Jackson 3.45: q/sqrt(r**2+d**2-2*r*d*cos(alpha)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Volt" : "Volt_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"q", "r", "d", "alpha", noiseRatio == null ? "Volt" : "Volt_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"q", "r", "d", "alpha"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var q       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var d       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 4, 6).ToList();
      var alpha   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 6).ToList();

      var Volt = new List<double>();

      data.Add(q);
      data.Add(r);
      data.Add(d);
      data.Add(alpha);
      data.Add(Volt);

      for (var i = 0; i < q.Count; i++) {
        var res = q[i] /
                  Math.Sqrt(Math.Pow(r[i], 2) + Math.Pow(d[i], 2) - 2 * r[i] * d[i] * Math.Cos(alpha[i]));
        Volt.Add(res);
      }

      if (noiseRatio != null) {
        var Volt_noise  = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * Volt.StandardDeviationPop();
        Volt_noise.AddRange(Volt.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(Volt);
        data.Add(Volt_noise);
      }

      return data;
    }
  }
}