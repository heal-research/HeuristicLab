using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman5 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman5() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman5(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman5(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.9.18 G*m1*m2/((x2-x1)**2+(y2-y1)**2+(z2-z1)**2) | {0} samples | {1}",
          trainingSamples, noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "F" : "F_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"m1", "m2", "G", "x1", "x2", "y1", "y2", "z1", "z2", noiseRatio == null ? "F" : "F_noise"}; }
    }

    protected override string[] AllowedInputVariables {
      get { return new[] {"m1", "m2", "G", "x1", "x2", "y1", "y2", "z1", "z2"}; }
    }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var m1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var m2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var G    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var x1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 3, 4).ToList();
      var x2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var y1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 3, 4).ToList();
      var y2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var z1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 3, 4).ToList();
      var z2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();

      var F = new List<double>();

      data.Add(m1);
      data.Add(m2);
      data.Add(G);
      data.Add(x1);
      data.Add(x2);
      data.Add(y1);
      data.Add(y2);
      data.Add(z1);
      data.Add(z2);
      data.Add(F);

      for (var i = 0; i < m1.Count; i++) {
        var res = G[i] * m1[i] * m2[i] /
                  (Math.Pow(x2[i] - x1[i], 2) + Math.Pow(y2[i] - y1[i], 2) + Math.Pow(z2[i] - z1[i], 2));
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