using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus5 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus5() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus5(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus5(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Relativistic aberation: arccos((cos(theta2)-v/c)/(1-v/c*cos(theta2))) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "theta1" : "theta1_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"c", "v", "theta2", noiseRatio == null ? "theta1" : "theta1_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"c", "v", "theta2"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data   = new List<List<double>>();
      var c      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 4, 6).ToList();
      var v      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var theta2 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var theta1 = new List<double>();

      data.Add(c);
      data.Add(v);
      data.Add(theta2);
      data.Add(theta1);

      for (var i = 0; i < c.Count; i++) {
        var res = Math.Acos((Math.Cos(theta2[i]) - v[i] / c[i]) / (1 - v[i] / c[i] * Math.Cos(theta2[i])));
        theta1.Add(res);
      }

      if (noiseRatio != null) {
        var theta1_noise = new List<double>();
        var sigma_noise  = (double) Math.Sqrt(noiseRatio.Value) * theta1.StandardDeviationPop();
        theta1_noise.AddRange(theta1.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(theta1);
        data.Add(theta1_noise);
      }

      return data;
    }
  }
}