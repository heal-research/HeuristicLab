using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman4 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman4() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman4(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman4(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.8.14 sqrt((x2-x1)**2+(y2-y1)**2) | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "d" : "d_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"x1", "x2", "y1", "y2", noiseRatio == null ? "d" : "d_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"x1", "x2", "y1", "y2"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var x1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var x2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var y1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var y2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var d = new List<double>();

      data.Add(x1);
      data.Add(x2);
      data.Add(y1);
      data.Add(y2);
      data.Add(d);

      for (var i = 0; i < x1.Count; i++) {
        var res = Math.Sqrt(Math.Pow(x2[i] - x1[i], 2) + Math.Pow(y2[i] - y1[i], 2));
        d.Add(res);
      }

      if (noiseRatio != null) {
        var d_noise     = new List<double>();
        var sigma_noise = (double) noiseRatio * d.StandardDeviationPop();
        d_noise.AddRange(d.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(d);
        data.Add(d_noise);
      }

      return data;
    }
  }
}