using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman12 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman12() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman12(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman12(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.12.11 q*(Ef + B*v*sin(theta)) | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "F" : "F_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"q", "Ef", "B", "v", "theta", noiseRatio == null ? "F" : "F_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"q", "Ef", "B", "v", "theta"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var q     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var Ef    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var B     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var v     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var F = new List<double>();

      data.Add(q);
      data.Add(Ef);
      data.Add(B);
      data.Add(v);
      data.Add(theta);
      data.Add(F);

      for (var i = 0; i < q.Count; i++) {
        var res = q[i] * (Ef[i] + B[i] * v[i] * Math.Sin(theta[i]));
        F.Add(res);
      }

      if (noiseRatio != null) {
        var F_noise     = new List<double>();
        var sigma_noise = (double) noiseRatio * F.StandardDeviationPop();
        F_noise.AddRange(F.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(F);
        data.Add(F_noise);
      }

      return data;
    }
  }
}