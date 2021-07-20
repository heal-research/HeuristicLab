using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman40 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman40() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman40(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman40(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("I.39.10 3/2*pF*V | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "E_n" : "E_n_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "pF", "V", "E_n" } : new[] { "pF", "V", "E_n", "E_n_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"pF", "V"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data = new List<List<double>>();
      var pF   = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var V    = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var E_n = new List<double>();

      data.Add(pF);
      data.Add(V);
      data.Add(E_n);

      for (var i = 0; i < pF.Count; i++) {
        var res = 3.0 / 2 * pF[i] * V[i];
        E_n.Add(res);
      }

      var targetNoise = GetNoisyTarget(E_n, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}