using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman94 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman94() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman94(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman94(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("III.14.14 I_0*(exp(q*Volt/(kb*T))-1) | {0} samples | {1}",
          trainingSamples, noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "I" : "I_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"I_0", "q", "Volt", "kb", "T", noiseRatio == null ? "I" : "I_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"I_0", "q", "Volt", "kb", "T"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var I_0  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var q    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var Volt = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var kb   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();
      var T    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 2).ToList();

      var I = new List<double>();

      data.Add(I_0);
      data.Add(q);
      data.Add(Volt);
      data.Add(kb);
      data.Add(T);
      data.Add(I);

      for (var i = 0; i < I_0.Count; i++) {
        var res = I_0[i] * (Math.Exp(q[i] * Volt[i] / (kb[i] * T[i])) - 1);
        I.Add(res);
      }

      if (noiseRatio != null) {
        var I_noise     = new List<double>();
        var sigma_noise = (double) noiseRatio * I.StandardDeviationPop();
        I_noise.AddRange(I.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(I);
        data.Add(I_noise);
      }

      return data;
    }
  }
}