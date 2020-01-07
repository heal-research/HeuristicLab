using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.JsonInterface.Converters {
  public class AlgorithmConverter : ParameterizedItemConverter {
    public override int Priority => 30;

    public override Type ConvertableType => typeof(IAlgorithm);

    public override void InjectData(IItem item, JsonItem data, IJsonItemConverter root) {
      base.InjectData(item, data, root);
      IAlgorithm algorithm = item as IAlgorithm;
      JsonItem problemData = data.Children.Where(x => x.Name == algorithm.Problem.Name).First();
      root.Inject(algorithm.Problem, problemData, root);

    }

    public override void Populate(IItem value, JsonItem item, IJsonItemConverter root) {
      base.Populate(value, item, root);
      IAlgorithm algorithm = value as IAlgorithm;
      item.AddChilds(root.Extract(algorithm.Problem, root));
    }
  }
}
