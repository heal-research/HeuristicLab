using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman25 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman25() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman25(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman25(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.25.13 q/C | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Volt" : "Volt_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"q", "C", noiseRatio == null ? "Volt" : "Volt_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"q", "C"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var q    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var C    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var Volt = new List<double>();

      data.Add(q);
      data.Add(C);
      data.Add(Volt);

      for (var i = 0; i < q.Count; i++) {
        var res = q[i] / C[i];
        Volt.Add(res);
      }

      if (noiseRatio != null) {
        var Volt_noise  = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * Volt.StandardDeviationPop();
        Volt_noise.AddRange(Volt.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(Volt);
        data.Add(Volt_noise);
      }

      return data;
    }
  }
}