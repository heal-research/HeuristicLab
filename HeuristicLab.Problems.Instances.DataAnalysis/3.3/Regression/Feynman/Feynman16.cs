using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman16 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman16() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman16(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman16(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.14.4 1/2*k_spring*x**2 | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "U" : "U_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"k_spring", "x", noiseRatio == null ? "U" : "U_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"k_spring", "x"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data     = new List<List<double>>();
      var k_spring = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var x        = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var U = new List<double>();

      data.Add(k_spring);
      data.Add(x);
      data.Add(U);

      for (var i = 0; i < k_spring.Count; i++) {
        var res = 1.0 / 2 * k_spring[i] * Math.Pow(x[i], 2);
        U.Add(res);
      }

      if (noiseRatio != null) {
        var U_noise     = new List<double>();
        var sigma_noise = (double) noiseRatio * U.StandardDeviationPop();
        U_noise.AddRange(U.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(U);
        data.Add(U_noise);
      }

      return data;
    }
  }
}