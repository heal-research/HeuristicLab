using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman86 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman86() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman86(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman86(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("III.4.32 1/(exp(h*omega/(kb*T))-1) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "n" : "n_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"h", "omega", "kb", "T", noiseRatio == null ? "n" : "n_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"h", "omega", "kb", "T"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var h     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var omega = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var kb    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var T     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var n = new List<double>();

      data.Add(h);
      data.Add(omega);
      data.Add(kb);
      data.Add(T);
      data.Add(n);

      for (var i = 0; i < h.Count; i++) {
        var res = 1.0 / (Math.Exp(h[i] * omega[i] / (kb[i] * T[i])) - 1);
        n.Add(res);
      }

      if (noiseRatio != null) {
        var n_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * n.StandardDeviationPop();
        n_noise.AddRange(n.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(n);
        data.Add(n_noise);
      }

      return data;
    }
  }
}