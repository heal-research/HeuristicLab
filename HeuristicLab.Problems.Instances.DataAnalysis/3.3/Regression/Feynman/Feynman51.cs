using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman51 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman51() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman51(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman51(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.50.26 x1*(cos(omega*t)+alpha*cos(omega*t)**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "x" : "x_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "x1", "omega", "t", "alpha", "x" } : new[] { "x1", "omega", "t", "alpha", "x", "x_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"x1", "omega", "t", "alpha"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var x1    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var omega = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var t     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var alpha = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var x = new List<double>();

      data.Add(x1);
      data.Add(omega);
      data.Add(t);
      data.Add(alpha);
      data.Add(x);

      for (var i = 0; i < x1.Count; i++) {
        var res = x1[i] * (Math.Cos(omega[i] * t[i]) + alpha[i] * Math.Pow(Math.Cos(omega[i] * t[i]), 2));
        x.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(x, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}