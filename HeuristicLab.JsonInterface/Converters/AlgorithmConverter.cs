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

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      base.Inject(item, data, root);
      IAlgorithm algorithm = item as IAlgorithm;
      IJsonItem problemData = data.Children.Where(x => x.Name == algorithm.Problem.Name).First();
      root.Inject(algorithm.Problem, problemData, root);

    }
    
    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IJsonItem item = base.Extract(value, root);
      IAlgorithm algorithm = value as IAlgorithm;
      foreach (var res in algorithm.Results) {
        item.AddChildren(new ResultItem() {
          Name = res.Name,
          Description = value.ItemDescription,
          Value = true,
          Range = new object[] { true, false }
        });
      }
      item.AddChildren(root.Extract(algorithm.Problem, root));
      return item;
    }
  }
}
