using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus1 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus1() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus1(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus1(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Rutherford scattering: (Z_1*Z_2*alpha*hbar*c/(4*E_n*sin(theta/2)**2))**2 | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "A" : "A_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"Z_1", "Z_2", "alpha", "hbar", "c", "E_n", "theta", noiseRatio == null ? "A" : "A_noise"}; }
    }

    protected override string[] AllowedInputVariables {
      get { return new[] {"Z_1", "Z_2", "alpha", "hbar", "c", "E_n", "theta"}; }
    }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var Z_1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var Z_2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var alpha = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var hbar  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var c     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var E_n   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var A = new List<double>();

      data.Add(Z_1);
      data.Add(Z_2);
      data.Add(alpha);
      data.Add(hbar);
      data.Add(c);
      data.Add(E_n);
      data.Add(theta);
      data.Add(A);

      for (var i = 0; i < Z_1.Count; i++) {
        var res = Math.Pow(
          Z_1[i] * Z_2[i] * alpha[i] * hbar[i] * c[i] / (4 * E_n[i] * Math.Pow(Math.Sin(theta[i] / 2), 2)), 2);
        A.Add(res);
      }

      if (noiseRatio != null) {
        var A_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * A.StandardDeviationPop();
        A_noise.AddRange(A.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(A);
        data.Add(A_noise);
      }

      return data;
    }
  }
}