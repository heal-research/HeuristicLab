//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using HEAL.Attic;
//using HeuristicLab.Collections;
//using HeuristicLab.Core;

//namespace HeuristicLab.JsonInterface {
//  public class ItemCollectionConverter : BaseConverter {
//    public override int Priority => 5;

//    public override bool CanConvertType(Type t) =>
//      t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IItemCollection<>));

//    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
//      var collection = (dynamic)value;
//      var item = new TypedListJsonItem();
//      IList<IJsonItem> valueItems = new List<IJsonItem>();

//      foreach(IItem i in collection) {
//        var tmp = root.Extract(i, root);
//        valueItems.Add(tmp);
//      }
//      if (valueItems.Count > 0) {
//        var targetType = valueItems.First().GetType();
//        item.TargetTypeGUID = StorableTypeAttribute.GetStorableTypeAttribute(targetType).Guid.ToString();
//      }

//      item.Value = valueItems.ToArray();
//      return item;
//    }

//    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
//      var collection = (dynamic)item;
//      var listItem = data as TypedListJsonItem;
//      var genericType = item.GetType().GetGenericArguments().First();

//      foreach (var i in listItem.Value) {
//        var tmp = Instantiate(genericType);
//        if (data.Active) {
//          i.Active = true;
//          root.Inject(tmp, i, root);
//          collection.Add((dynamic)tmp);
//        }
//      }
//    }
//  }
//}
