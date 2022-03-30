//using System;
//using System.Linq;
//using HeuristicLab.Core;
//using HeuristicLab.Optimization;

//namespace HeuristicLab.JsonInterface {

//  public class AlgorithmConverter : ParameterizedItemConverter {
//    public override int Priority => 30;

//    public override bool CanConvertType(Type t) => 
//      t.GetInterfaces().Any(x => x == typeof(IAlgorithm));

//    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
//      base.Inject(item, data, root);
//      IAlgorithm algorithm = item as IAlgorithm;
//      var collection = data.Children.Where(x => x.Name == algorithm.Problem.ItemName);
//      if(collection.Count() > 0) {
//        IJsonItem problemData = collection.First();
//        root.Inject(algorithm.Problem, problemData, root);
//      }
//    }
    
//    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
//      IJsonItem item = base.Extract(value, root);
//      IAlgorithm algorithm = value as IAlgorithm;
//      item.AddChildren(root.Extract(algorithm.Problem, root));
//      return item;
//    }
//  }
//}
