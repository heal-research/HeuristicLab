using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman30 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman30() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman30(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman30(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.30.3 Int_0*sin(n*theta/2)**2/sin(theta/2)**2 | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Int" : "Int_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "Int_0", "theta", "n", "Int" } : new[] { "Int_0", "theta", "n", "Int", "Int_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"Int_0", "theta", "n"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var Int_0 = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var theta = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var n     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var Int = new List<double>();

      data.Add(Int_0);
      data.Add(theta);
      data.Add(n);
      data.Add(Int);

      for (var i = 0; i < Int_0.Count; i++) {
        var res = Int_0[i] * Math.Pow(Math.Sin(n[i] * theta[i] / 2), 2) / Math.Pow(Math.Sin(theta[i] / 2), 2);
        Int.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(Int, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}