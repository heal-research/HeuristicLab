using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman10 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman10() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman10(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman10(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.12.4 q1/(4*pi*epsilon*r**2) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "Ef" : "Ef_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "q1", "epsilon", "r", "Ef" } : new[] { "q1", "epsilon", "r", "Ef", "Ef_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"q1", "epsilon", "r"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data    = new List<List<double>>();
      var q1      = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var epsilon = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var r       = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var Ef = new List<double>();

      data.Add(q1);
      data.Add(epsilon);
      data.Add(r);
      data.Add(Ef);

      for (var i = 0; i < q1.Count; i++) {
        var res = q1[i] / (4 * Math.PI * epsilon[i] * Math.Pow(r[i], 2));
        Ef.Add(res);
      }

      var targetNoise = ValueGenerator.GenerateNoise(Ef, rand, noiseRatio);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}