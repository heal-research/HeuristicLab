using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {

  public class IntArrayConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(IntArray);

    public override bool CanConvertType(Type t) =>
      ConvertableType.IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IntArray arr = item as IntArray;
      IntArrayJsonItem intArrayItem = data as IntArrayJsonItem;
      bool resizeTmp = arr.Resizable;
      arr.Resizable = true;
      arr.Length = intArrayItem.Value.Length;
      for (int i = 0; i < intArrayItem.Value.Length; ++i)
        arr[i] = intArrayItem.Value[i];
      arr.Resizable = resizeTmp;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new IntArrayJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((IntArray)value).CloneAsArray(),
        Minimum = int.MinValue,
        Maximum = int.MaxValue
      };
  }

  public class DoubleArrayConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(DoubleArray);

    public override bool CanConvertType(Type t) =>
      ConvertableType.IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      DoubleArray arr = item as DoubleArray;
      DoubleArrayJsonItem doubleArrayItem = data as DoubleArrayJsonItem;
      bool resizeTmp = arr.Resizable;
      arr.Resizable = true;
      arr.Length = doubleArrayItem.Value.Length;
      for (int i = 0; i < doubleArrayItem.Value.Length; ++i)
        arr[i] = doubleArrayItem.Value[i];
      arr.Resizable = resizeTmp;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DoubleArrayJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((DoubleArray)value).CloneAsArray(),
        Minimum = double.MinValue,
        Maximum = double.MaxValue
      };
  }

  public class PercentArrayConverter : BaseConverter {
    public override int Priority => 2;
    public override Type ConvertableType => typeof(PercentArray);

    public override bool CanConvertType(Type t) =>
      ConvertableType.IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      PercentArray arr = item as PercentArray;
      DoubleArrayJsonItem doubleArrayItem = data as DoubleArrayJsonItem;
      bool resizeTmp = arr.Resizable;
      arr.Resizable = true;
      arr.Length = doubleArrayItem.Value.Length;
      for (int i = 0; i < doubleArrayItem.Value.Length; ++i)
        arr[i] = doubleArrayItem.Value[i];
      arr.Resizable = resizeTmp;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DoubleArrayJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((PercentArray)value).CloneAsArray(),
        Minimum = 0.0d,
        Maximum = 1.0d
      };
  }

  public class BoolArrayConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(BoolArray);

    public override bool CanConvertType(Type t) =>
      ConvertableType.IsAssignableFrom(t);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      BoolArray arr = item as BoolArray;
      BoolArrayJsonItem boolArrayItem = data as BoolArrayJsonItem;
      bool resizeTmp = arr.Resizable;
      arr.Resizable = true;
      arr.Length = boolArrayItem.Value.Length;
      for(int i = 0; i < boolArrayItem.Value.Length; ++i)
        arr[i] = boolArrayItem.Value[i];
      arr.Resizable = resizeTmp;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new BoolArrayJsonItem() {
        Name = value.ItemName,
        Description = value.ItemDescription,
        Value = ((BoolArray)value).CloneAsArray()
      };
  }
}
