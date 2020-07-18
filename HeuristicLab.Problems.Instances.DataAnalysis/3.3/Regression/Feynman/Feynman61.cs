using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman61 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman61() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman61(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman61(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.11.3 q*Ef/(m*(omega_0**2-omega**2)) | {0} samples | {1}",
          trainingSamples, noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "x" : "x_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"q", "Ef", "m", "omega_0", "omega", noiseRatio == null ? "x" : "x_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"q", "Ef", "m", "omega_0", "omega"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var q       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var Ef      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var m       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 3).ToList();
      var omega_0 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 3, 5).ToList();
      var omega   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();

      var x = new List<double>();

      data.Add(q);
      data.Add(Ef);
      data.Add(m);
      data.Add(omega_0);
      data.Add(omega);
      data.Add(x);

      for (var i = 0; i < q.Count; i++) {
        var res = q[i] * Ef[i] / (m[i] * (Math.Pow(omega_0[i], 2) - Math.Pow(omega[i], 2)));
        x.Add(res);
      }

      if (noiseRatio != null) {
        var x_noise     = new List<double>();
        var sigma_noise = (double) noiseRatio * x.StandardDeviationPop();
        x_noise.AddRange(x.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(x);
        data.Add(x_noise);
      }

      return data;
    }
  }
}