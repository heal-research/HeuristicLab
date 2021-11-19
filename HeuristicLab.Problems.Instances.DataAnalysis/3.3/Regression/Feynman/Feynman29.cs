using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman29 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman29() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman29(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman29(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.29.16 sqrt(x1**2+x2**2 - 2*x1*x2*cos(theta1 - theta2)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "x" : "x_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "x1", "x2", "theta1", "theta2", "x" } : new[] { "x1", "x2", "theta1", "theta2", "x", "x_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"x1", "x2", "theta1", "theta2"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data   = new List<List<double>>();
      var x1     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var x2     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var theta1 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var theta2 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var x = new List<double>();

      data.Add(x1);
      data.Add(x2);
      data.Add(theta1);
      data.Add(theta2);
      data.Add(x);

      for (var i = 0; i < x1.Count; i++) {
        var res = Math.Sqrt(Math.Pow(x1[i], 2) + Math.Pow(x2[i], 2) -
                            2 * x1[i] * x2[i] * Math.Cos(theta1[i] - theta2[i]));
        x.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(x, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}