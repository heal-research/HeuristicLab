using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman93 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman93() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman93(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman93(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("III.13.18 2*E_n*d**2*k/h | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "v" : "v_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"E_n", "d", "k", "h", noiseRatio == null ? "v" : "v_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"E_n", "d", "k", "h"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var E_n  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var d    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var k    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var h    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var v = new List<double>();

      data.Add(E_n);
      data.Add(d);
      data.Add(k);
      data.Add(h);
      data.Add(v);

      for (var i = 0; i < E_n.Count; i++) {
        var res = 2 * E_n[i] * Math.Pow(d[i], 2) * k[i] / h[i];
        v.Add(res);
      }

      if (noiseRatio != null) {
        var v_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * v.StandardDeviationPop();
        v_noise.AddRange(v.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(v);
        data.Add(v_noise);
      }

      return data;
    }
  }
}