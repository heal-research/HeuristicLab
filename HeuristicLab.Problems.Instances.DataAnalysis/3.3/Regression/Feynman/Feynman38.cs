using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman38 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman38() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman38(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman38(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.37.4 I1 + I2 + 2*sqrt(I1*I2)*cos(delta) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Int" : "Int_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "I1", "I2", "delta", "Int" } : new[] { "I1", "I2", "delta", "Int", "Int_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"I1", "I2", "delta"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var I1    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var I2    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var delta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var Int = new List<double>();

      data.Add(I1);
      data.Add(I2);
      data.Add(delta);
      data.Add(Int);

      for (var i = 0; i < I1.Count; i++) {
        var res = I1[i] + I2[i] + 2 * Math.Sqrt(I1[i] * I2[i]) * Math.Cos(delta[i]);
        Int.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(Int, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}