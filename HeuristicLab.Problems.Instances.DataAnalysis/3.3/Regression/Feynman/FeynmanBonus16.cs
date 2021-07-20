using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus16 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus16() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus16(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus16(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format(
          "Jackson 4.60: Ef*cos(theta)*((alpha-1)/(alpha+2)*d**3/r**2-r) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Volt" : "Volt_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "Ef", "theta", "r", "d", "alpha", "Volt" } : new[] { "Ef", "theta", "r", "d", "alpha", "Volt", "Volt_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"Ef", "theta", "r", "d", "alpha"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var Ef    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 6).ToList();
      var r     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var d     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var alpha = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var Volt = new List<double>();

      data.Add(Ef);
      data.Add(theta);
      data.Add(r);
      data.Add(d);
      data.Add(alpha);
      data.Add(Volt);

      for (var i = 0; i < Ef.Count; i++) {
        var res = Ef[i] * Math.Cos(theta[i]) *
                  ((alpha[i] - 1) / (alpha[i] + 2) * Math.Pow(d[i], 3) / Math.Pow(r[i], 2) - r[i] );
        Volt.Add(res);
      }

      var targetNoise = GetNoisyTarget(Volt, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}