using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus6 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus6() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus6(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus6(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "N-slit diffraction: I_0*(sin(alpha/2)*sin(n*delta/2)/(alpha/2*sin(delta/2)))**2 | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "I" : "I_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "I_0", "alpha", "delta", "n", "I" } : new[] { "I_0", "alpha", "delta", "n", "I", "I_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"I_0", "alpha", "delta", "n"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var I_0   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var alpha = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var delta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var n     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();

      var I = new List<double>();

      data.Add(I_0);
      data.Add(alpha);
      data.Add(delta);
      data.Add(n);
      data.Add(I);

      for (var i = 0; i < I_0.Count; i++) {
        var res = I_0[i] * Math.Pow(
                    Math.Sin(alpha[i] / 2) * Math.Sin(n[i] * delta[i] / 2) / (alpha[i] / 2 * Math.Sin(delta[i] / 2)),
                    2);
        I.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(I, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}