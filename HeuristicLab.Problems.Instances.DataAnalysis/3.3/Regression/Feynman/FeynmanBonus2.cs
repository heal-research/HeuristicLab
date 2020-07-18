using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus2 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus2() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus2(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus2(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("Friedman Equation: sqrt(8*pi*G*rho/3-alpha*c**2/d**2) | {0} samples | {1}",
          trainingSamples, noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "H_G" : "H_G_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"G", "rho", "alpha", "c", "d", noiseRatio == null ? "H_G" : "H_G_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"G", "rho", "alpha", "c", "d"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var G     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var rho   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var alpha = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var c     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var d     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var H_G = new List<double>();

      data.Add(G);
      data.Add(rho);
      data.Add(alpha);
      data.Add(c);
      data.Add(d);
      data.Add(H_G);

      for (var i = 0; i < G.Count; i++) {
        var res = Math.Sqrt(8 * Math.PI * G[i] * rho[i] / 3 - alpha[i] * Math.Pow(c[i], 2) / Math.Pow(d[i], 2));
        H_G.Add(res);
      }

      if (noiseRatio != null) {
        var H_G_noise   = new List<double>();
        var sigma_noise = (double) noiseRatio * H_G.StandardDeviationPop();
        H_G_noise.AddRange(H_G.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(H_G);
        data.Add(H_G_noise);
      }

      return data;
    }
  }
}