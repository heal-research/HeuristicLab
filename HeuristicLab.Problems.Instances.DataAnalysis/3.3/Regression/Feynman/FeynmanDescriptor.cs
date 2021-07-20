using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public abstract class FeynmanDescriptor : ArtificialRegressionDataDescriptor {
    protected double? noiseRatio;
    public override string Description {
      get {
        return "Feynman instances ... descriptions follows: " + Environment.NewLine;
      }
    }

    public List<double> GetNoisyTarget(List<double> target, IRandom rand) {
      if (noiseRatio == null) return null;

      var targetNoise = new List<double>();
      var sigmaNoise = Math.Sqrt(noiseRatio.Value) * target.StandardDeviationPop();
      targetNoise.AddRange(target.Select(md => md + NormalDistributedRandomPolar.NextDouble(rand, 0, sigmaNoise)));
      return targetNoise;

    }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 100; } }
    protected override int TestPartitionStart { get { return 100; } }
    protected override int TestPartitionEnd { get { return 200; } }
  }
}
