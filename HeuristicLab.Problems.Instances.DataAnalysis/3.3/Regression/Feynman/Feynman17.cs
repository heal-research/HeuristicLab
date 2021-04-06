using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman17 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman17() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman17(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman17(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.15.3x (x-u*t)/sqrt(1-u**2/c**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "x1" : "x1_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"x", "u", "c", "t", noiseRatio == null ? "x1" : "x1_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"x", "u", "c", "t"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var x    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 5, 10).ToList();
      var u    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var c    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 3, 20).ToList();
      var t    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();

      var x1 = new List<double>();

      data.Add(x);
      data.Add(u);
      data.Add(c);
      data.Add(t);
      data.Add(x1);

      for (var i = 0; i < x.Count; i++) {
        var res = (x[i] - u[i] * t[i]) / Math.Sqrt(1 - Math.Pow(u[i], 2) / Math.Pow(c[i], 2));
        x1.Add(res);
      }

      if (noiseRatio != null) {
        var x1_noise    = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * x1.StandardDeviationPop();
        x1_noise.AddRange(x1.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(x1);
        data.Add(x1_noise);
      }

      return data;
    }
  }
}