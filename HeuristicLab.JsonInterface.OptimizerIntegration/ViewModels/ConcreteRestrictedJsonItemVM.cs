using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  /// <summary>
  /// Abstract VM class for concreted restricted JsonItems
  /// </summary>
  /// <typeparam name="T">JsonItemType</typeparam>
  /// <typeparam name="X">Type of the range array (ConcreteRestrictedItemArray)</typeparam>
  /// <typeparam name="V">Type of the value property</typeparam>
  public abstract class ConcreteRestrictedJsonItemVM<T, X, V> : JsonItemVMBase<T>
    where T : class, IConcreteRestrictedJsonItem<X>, IValueJsonItem<V> {
    public override UserControl Control {
      get {
        var control = ConcreteItemsRestrictor.Create(Item.ConcreteRestrictedItems);
        control.OnChecked += AddComboOption;
        control.OnUnchecked += RemoveComboOption;
        return control;
      }
    }

    public IEnumerable<X> Range {
      get => Item.ConcreteRestrictedItems;
      set {
        Item.ConcreteRestrictedItems = value;
        //check if value is still in range

        if (!RangeContainsValue()) {
          Item.Value = GetDefaultValue();

          //if no elements exists -> deselect item
          if (Range.Count() == 0)
            base.Selected = false;
        }

        OnPropertyChange(this, nameof(Range));
      }
    }

    /// <summary>
    /// Method to check if the value exists in the allowed range array.
    /// </summary>
    /// <returns></returns>
    protected abstract bool RangeContainsValue();

    /// <summary>
    /// Method to get or generate a default value for property "Value".
    /// </summary>
    /// <returns></returns>
    protected abstract V GetDefaultValue();

    private void AddComboOption(object opt) {
      var items = new List<X>(Range);
      items.Add((X)opt);
      Range = items;
    }

    private void RemoveComboOption(object opt) {
      var items = new List<X>(Range);
      items.Remove((X)opt);
      Range = items;
    }
  }
}
