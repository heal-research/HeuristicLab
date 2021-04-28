using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman81 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman81() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman81(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman81(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("II.35.21 n_rho*mom*tanh(mom*B/(kb*T)) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "M" : "M_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"n_rho", "mom", "B", "kb", "T", noiseRatio == null ? "M" : "M_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"n_rho", "mom", "B", "kb", "T"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var n_rho = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var mom   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var B     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var kb    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var T     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var M = new List<double>();

      data.Add(n_rho);
      data.Add(mom);
      data.Add(B);
      data.Add(kb);
      data.Add(T);
      data.Add(M);

      for (var i = 0; i < n_rho.Count; i++) {
        var res = n_rho[i] * mom[i] * Math.Tanh(mom[i] * B[i] / (kb[i] * T[i]));
        M.Add(res);
      }

      if (noiseRatio != null) {
        var M_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * M.StandardDeviationPop();
        M_noise.AddRange(M.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(M);
        data.Add(M_noise);
      }

      return data;
    }
  }
}