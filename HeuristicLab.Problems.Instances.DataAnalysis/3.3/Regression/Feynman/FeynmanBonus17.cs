using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus17 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus17() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus17(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus17(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Jackson 11.38: sqrt(1-v**2/c**2)*omega/(1+v/c*cos(theta)) | {0} samples | {1}",
          trainingSamples, noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "omega_0" : "omega_0_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"c", "v", "omega", "theta", noiseRatio == null ? "omega_0" : "omega_0_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"c", "v", "omega", "theta"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var c     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 5, 20).ToList();
      var v     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var omega = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 6).ToList();

      var omega_0 = new List<double>();

      data.Add(c);
      data.Add(v);
      data.Add(omega);
      data.Add(theta);
      data.Add(omega_0);

      for (var i = 0; i < c.Count; i++) {
        var res = Math.Sqrt(1 - Math.Pow(v[i], 2) / Math.Pow(c[i], 2)) * omega[i] /
                  (1 + v[i] / c[i] * Math.Cos(theta[i]));
        omega_0.Add(res);
      }

      if (noiseRatio != null) {
        var omega_0_noise = new List<double>();
        var sigma_noise   = (double) noiseRatio * omega_0.StandardDeviationPop();
        omega_0_noise.AddRange(omega_0.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(omega_0);
        data.Add(omega_0_noise);
      }

      return data;
    }
  }
}