using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman23 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman23() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman23(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman23(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.18.16 m*r*v*sin(theta) | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "L" : "L_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"m", "r", "v", "theta", noiseRatio == null ? "L" : "L_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"m", "r", "v", "theta"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var m     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var v     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var L = new List<double>();

      data.Add(m);
      data.Add(r);
      data.Add(v);
      data.Add(theta);
      data.Add(L);

      for (var i = 0; i < m.Count; i++) {
        var res = m[i] * r[i] * v[i] * Math.Sin(theta[i]);
        L.Add(res);
      }

      if (noiseRatio != null) {
        var L_noise     = new List<double>();
        var sigma_noise = (double) noiseRatio * L.StandardDeviationPop();
        L_noise.AddRange(L.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(L);
        data.Add(L_noise);
      }

      return data;
    }
  }
}