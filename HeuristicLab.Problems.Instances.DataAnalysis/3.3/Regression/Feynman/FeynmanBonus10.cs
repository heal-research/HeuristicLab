using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class FeynmanBonus10 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public FeynmanBonus10() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public FeynmanBonus10(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public FeynmanBonus10(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("Goldstein 3.74: 2*pi*d**(3/2)/sqrt(G*(m1+m2)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "t" : "t_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"d", "G", "m1", "m2", noiseRatio == null ? "t" : "t_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"d", "G", "m1", "m2"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var d    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var G    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var m1   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var m2   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();

      var t = new List<double>();

      data.Add(d);
      data.Add(G);
      data.Add(m1);
      data.Add(m2);
      data.Add(t);

      for (var i = 0; i < d.Count; i++) {
        var res = 2 * Math.PI * Math.Pow(d[i], 3.0 / 2) / Math.Sqrt(G[i] * (m1[i] + m2[i]));
        t.Add(res);
      }

      if (noiseRatio != null) {
        var t_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * t.StandardDeviationPop();
        t_noise.AddRange(t.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(t);
        data.Add(t_noise);
      }

      return data;
    }
  }
}