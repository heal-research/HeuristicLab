using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman58 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman58() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman58(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman58(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.8.7 3/5*q**2/(4*pi*epsilon*d) | {0} samples | {1}", trainingSamples,
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "E_n" : "E_n_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"q", "epsilon", "d", noiseRatio == null ? "E_n" : "E_n_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"q", "epsilon", "d"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var q       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var d       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var E_n = new List<double>();

      data.Add(q);
      data.Add(epsilon);
      data.Add(d);
      data.Add(E_n);

      for (var i = 0; i < q.Count; i++) {
        var res = 3.0 / 5 * Math.Pow(q[i], 2) / (4 * Math.PI * epsilon[i] * d[i]);
        E_n.Add(res);
      }

      if (noiseRatio != null) {
        var E_n_noise   = new List<double>();
        var sigma_noise = (double) noiseRatio * E_n.StandardDeviationPop();
        E_n_noise.AddRange(E_n.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(E_n);
        data.Add(E_n_noise);
      }

      return data;
    }
  }
}