//using System;
//using System.Linq;
//using HeuristicLab.Core;
//using HeuristicLab.Optimization;

//namespace HeuristicLab.JsonInterface {
//  public class ResultParameterConverter : BaseConverter {
//    public override int Priority => 5;

//    public override bool CanConvertType(Type t) =>
//      t.GetInterfaces().Any(x => x == typeof(IResultParameter));

//    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
//      return new UnsupportedJsonItem();
//    }

//    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {}
//  }
//}
