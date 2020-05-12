using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.JsonInterface {
  public class ExperimentConverter : BaseConverter {
    public override int Priority => 10;
    public override Type ConvertableType => HEAL.Attic.Mapper.StaticCache.GetType(new Guid("A8A4536B-54C1-4A17-AB58-A6006F7F394B"));

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      dynamic experiment = (dynamic)value;
      EmptyJsonItem experimentJI = new EmptyJsonItem() {
        Name = experiment.Name,
        Description = value.ItemDescription
      };

      OptimizerList optimizers = experiment.Optimizers;
      foreach(var o in optimizers) {
        var optimizerJI = root.Extract(o, root);
        if (!(optimizerJI is UnsupportedJsonItem)) {
          experimentJI.AddChildren(optimizerJI);
        }
      }

      int worker = (int)experiment.NumberOfWorkers;
      experimentJI.AddChildren(new IntJsonItem() {
        Name = "NumberOfWorkers",
        Description = "The number of workers for this experiment.",
        Value = worker,
        Minimum = 1,
        Maximum = int.MaxValue
      });

      return experimentJI;
    }

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      dynamic experiment = (dynamic)item;
      OptimizerList optimizers = experiment.Optimizers;
      if (data.Children != null) {
        foreach (var i in data.Children) {
          if (i.Path.EndsWith("NumberOfWorkers") && i is IntJsonItem worker) {
            experiment.NumberOfWorkers = worker.Value;
          } else {
            var optimizer = optimizers.Find(o => i.Path.EndsWith(o.Name));
            root.Inject(optimizer, i, root);
          }
        }
      }
    }
  }
}
