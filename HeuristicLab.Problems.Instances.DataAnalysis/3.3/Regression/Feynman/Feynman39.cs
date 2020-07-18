using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman39 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman39() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman39(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman39(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.38.12 4*pi*epsilon*h**2/(m*q**2) | {0} samples | {1}",
          trainingSamples, noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "r" : "r_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"m", "q", "h", "epsilon", noiseRatio == null ? "r" : "r_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"m", "q", "h", "epsilon"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var m       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var q       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var h       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var r = new List<double>();

      data.Add(m);
      data.Add(q);
      data.Add(h);
      data.Add(epsilon);
      data.Add(r);

      for (var i = 0; i < m.Count; i++) {
        var res = 4 * Math.PI * epsilon[i] * Math.Pow(h[i], 2) / (m[i] * Math.Pow(q[i], 2));
        r.Add(res);
      }

      if (noiseRatio != null) {
        var r_noise     = new List<double>();
        var sigma_noise = (double) noiseRatio * r.StandardDeviationPop();
        r_noise.AddRange(r.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(r);
        data.Add(r_noise);
      }

      return data;
    }
  }
}