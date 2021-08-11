using System;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class BatchRunConverter : BaseConverter {
    public override int Priority => 10;

    public override bool CanConvertType(Type t) => 
      HEAL.Attic.Mapper.StaticCache.GetType(new Guid("E85407E0-18EC-4198-8321-9CF030FDF6D7")).IsAssignableFrom(t);

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      dynamic batchRun = (dynamic)value;
      EmptyJsonItem batchRunJI = new EmptyJsonItem() {
        Name = batchRun.Name,
        Description = value.ItemDescription
      };

      var optimizerJI = root.Extract((IItem)batchRun.Optimizer, root);
      if(!(optimizerJI is UnsupportedJsonItem)) {
        batchRunJI.AddChildren(optimizerJI);
      }

      int reps = (int)batchRun.Repetitions;
      batchRunJI.AddChildren(new IntJsonItem() { 
        Name = "Repetitions", 
        Description = "The repetitions for this batch run.", 
        Value = reps, 
        Minimum = 0, 
        Maximum = int.MaxValue 
      });
      return batchRunJI;
    }

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      dynamic batchRun = (dynamic)item;
      if(data.Children != null) {
        foreach(var i in data.Children) {
          if(i.Path.EndsWith("Repetitions") && i is IntJsonItem reps ) {
            batchRun.Repetitions = reps.Value;
          } else {
            root.Inject((IItem)batchRun.Optimizer, i, root);
          }
        }
      }
    }
  }
}
