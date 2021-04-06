using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus20 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus20() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus20(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus20(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Schwarz 13.132 (Klein-Nishina): pi*alpha**2*h**2/(m**2*c**2)*(omega_0/omega)**2*(omega_0/omega+omega/omega_0-sin(beta)**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "A" : "A_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"omega", "omega_0", "alpha", "h", "m", "c", "beta", noiseRatio == null ? "A" : "A_noise"}; }
    }

    protected override string[] AllowedInputVariables {
      get { return new[] {"omega", "omega_0", "alpha", "h", "m", "c", "beta"}; }
    }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var omega   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var omega_0 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var alpha   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var h       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var m       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var c       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var beta    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 6).ToList();

      var A = new List<double>();

      data.Add(omega);
      data.Add(omega_0);
      data.Add(alpha);
      data.Add(h);
      data.Add(m);
      data.Add(c);
      data.Add(beta);
      data.Add(A);

      for (var i = 0; i < omega.Count; i++) {
        var res = Math.PI * Math.Pow(alpha[i], 2) * Math.Pow(h[i], 2) /
                  (Math.Pow(m[i], 2) * Math.Pow(c[i], 2)) * Math.Pow(omega_0[i] / omega[i], 2) *
                  (omega_0[i] / omega[i] + omega[i] / omega_0[i] - Math.Pow(Math.Sin(beta[i]), 2));
        A.Add(res);
      }

      if (noiseRatio != null) {
        var A_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * A.StandardDeviationPop();
        A_noise.AddRange(A.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(A);
        data.Add(A_noise);
      }

      return data;
    }
  }
}