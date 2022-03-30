using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman26 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman26() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman26(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman26(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.26.2 arcsin(n*sin(theta2)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "theta1" : "theta1_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "n", "theta2", "theta1" } : new[] { "n", "theta2", "theta1", "theta1_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"n", "theta2"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data   = new List<List<double>>();
      var n      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 0, 1).ToList();
      var theta2 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var theta1 = new List<double>();

      data.Add(n);
      data.Add(theta2);
      data.Add(theta1);

      for (var i = 0; i < n.Count; i++) {
        var res = Math.Asin(n[i] * Math.Sin(theta2[i]));
        theta1.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(theta1, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}