using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus14 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus14() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus14(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus14(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Jackson 2.11: q/(4*pi*epsilon*y**2)*(4*pi*epsilon*Volt*d-q*d*y**3/(y**2-d**2)**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "F" : "F_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"q", "y", "Volt", "d", "epsilon", noiseRatio == null ? "F" : "F_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"q", "y", "Volt", "d", "epsilon"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var q       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var y       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var Volt    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var d       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 4, 6).ToList();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var F = new List<double>();

      data.Add(q);
      data.Add(y);
      data.Add(Volt);
      data.Add(d);
      data.Add(epsilon);
      data.Add(F);

      for (var i = 0; i < q.Count; i++) {
        var res = q[i] / (4 * Math.PI * epsilon[i] * Math.Pow(y[i], 2)) * (
                    4 * Math.PI * epsilon[i] * Volt[i] * d[i] - q[i] * d[i] * Math.Pow(y[i], 3) /
                    Math.Pow(Math.Pow(y[i], 2) - Math.Pow(d[i], 2), 2));
        F.Add(res);
      }

      if (noiseRatio != null) {
        var F_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * F.StandardDeviationPop();
        F_noise.AddRange(F.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(F);
        data.Add(F_noise);
      }

      return data;
    }
  }
}