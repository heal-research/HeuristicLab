using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public abstract class FeynmanDescriptor : ArtificialRegressionDataDescriptor {
    protected double? noiseRatio;
    public override string Description {
      get {
        return "Feynman instances ... descriptions follows: " + Environment.NewLine;
      }
    }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 100; } }
    protected override int TestPartitionStart { get { return 100; } }
    protected override int TestPartitionEnd { get { return 200; } }
  }
}
