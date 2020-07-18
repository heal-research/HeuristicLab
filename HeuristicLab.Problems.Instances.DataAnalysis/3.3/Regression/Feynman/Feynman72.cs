using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman72 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman72() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman72(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman72(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.24.17 sqrt(omega**2/c**2-pi**2/d**2) | {0} samples | {1}",
          trainingSamples, noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "k" : "k_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"omega", "c", "d", noiseRatio == null ? "k" : "k_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"omega", "c", "d"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var omega = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 4, 6).ToList();
      var c     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var d     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 2, 4).ToList();

      var k = new List<double>();

      data.Add(omega);
      data.Add(c);
      data.Add(d);
      data.Add(k);

      for (var i = 0; i < omega.Count; i++) {
        var res = Math.Sqrt(Math.Pow(omega[i], 2) / Math.Pow(c[i], 2) - Math.Pow(Math.PI, 2) / Math.Pow(d[i], 2));
        k.Add(res);
      }

      if (noiseRatio != null) {
        var k_noise     = new List<double>();
        var sigma_noise = (double) noiseRatio * k.StandardDeviationPop();
        k_noise.AddRange(k.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(k);
        data.Add(k_noise);
      }

      return data;
    }
  }
}