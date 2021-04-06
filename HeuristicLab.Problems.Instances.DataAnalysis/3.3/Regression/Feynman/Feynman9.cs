using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman9 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman9() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman9(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman9(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.12.2 q1*q2/(4*pi*epsilon*r**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "F" : "F_noise"; } }

    protected override string[] VariableNames {
      get { return new[] {"q1", "q2", "epsilon", "r", noiseRatio == null ? "F" : "F_noise"}; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"q1", "q2", "epsilon", "r"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var q1      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var q2      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var F = new List<double>();

      data.Add(q1);
      data.Add(q2);
      data.Add(epsilon);
      data.Add(r);
      data.Add(F);

      for (var i = 0; i < q1.Count; i++) {
        var res = q1[i] * q2[i] / (4 * Math.PI * epsilon[i] * Math.Pow(r[i], 2));
        F.Add(res);
      }

      if (noiseRatio != null) {
        var F_noise     = new List<double>();
        var sigma_noise = (double) Math.Sqrt(noiseRatio.Value) * F.StandardDeviationPop();
        F_noise.AddRange(F.Select(md => md + NormalDistributedRandom.NextDouble(rand, 0, sigma_noise)));
        data.Remove(F);
        data.Add(F_noise);
      }

      return data;
    }
  }
}