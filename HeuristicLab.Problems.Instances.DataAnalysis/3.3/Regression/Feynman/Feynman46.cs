using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman46 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman46() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman46(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman46(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.43.31 mob*kb*T | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "D" : "D_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"mob", "T", "kb", noiseRatio == null ? "D" : "D_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"mob", "T", "kb"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var mob  = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var T    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var kb   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var D = new List<double>();

      data.Add(mob);
      data.Add(T);
      data.Add(kb);
      data.Add(D);

      for (var i = 0; i < mob.Count; i++) {
        var res = mob[i] * kb[i] * T[i];
        D.Add(res);
      }

      if (noiseRatio != null) {
        var D_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * D.StandardDeviationPop();
        D_noise.AddRange(D.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigma_noise)));
        data.Remove(D);
        data.Add(D_noise);
      }

      return data;
    }
  }
}