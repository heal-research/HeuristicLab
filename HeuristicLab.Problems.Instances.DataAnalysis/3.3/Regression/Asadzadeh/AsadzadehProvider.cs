using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Regression.Asadzadeh {
  public class AsadzadehProvider : ArtificialRegressionInstanceProvider {
    public override string Name => "Asadzadeh";

    public override string Description => "Asadzadeh";

    public override Uri WebLink => new Uri("https://www.google.com/");

    public override string ReferencePublication => "";

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      return new IDataDescriptor[] { new Asadzadeh1() };
    }
  }
}
