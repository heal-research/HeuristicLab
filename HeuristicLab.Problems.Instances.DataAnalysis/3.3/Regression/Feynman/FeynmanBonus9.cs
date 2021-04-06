using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus9 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus9() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus9(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus9(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Goldstein 3.64: d*(1-alpha**2)/(1+alpha*cos(theta1-theta2)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "r" : "r_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"d", "alpha", "theta1", "theta2", noiseRatio == null ? "r" : "r_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"d", "alpha", "theta1", "theta2"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data   = new List<List<double>>();
      var d      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var alpha  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 2, 4).ToList();
      var theta1 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 4, 5).ToList();
      var theta2 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 4, 5).ToList();

      var r = new List<double>();

      data.Add(d);
      data.Add(alpha);
      data.Add(theta1);
      data.Add(theta2);
      data.Add(r);

      for (var i = 0; i < d.Count; i++) {
        var res = d[i] * (1 - Math.Pow(alpha[i], 2)) / (1 + alpha[i] * Math.Cos(theta1[i] - theta2[i]));
        r.Add(res);
      }

      if (noiseRatio != null) {
        var r_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * r.StandardDeviationPop();
        r_noise.AddRange(r.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(r);
        data.Add(r_noise);
      }

      return data;
    }
  }
}