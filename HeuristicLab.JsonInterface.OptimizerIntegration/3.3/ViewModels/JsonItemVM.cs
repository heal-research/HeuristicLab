using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.JsonInterface;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class JsonItemVM: INotifyPropertyChanged
  {
    public JsonItem Item { get; }

    private bool selected;
    public bool Selected {
      get => selected;
      set {
        selected = value;
        OnPropertyChange(this, nameof(Selected));
      }
    }

    public string Name {
      get => Item.Name;
      set {
        Item.Name = value;
        OnPropertyChange(this, nameof(Name));
      }
    }

    public string Description {
      get => Item.Description;
      set {
        Item.Description = value;
        OnPropertyChange(this, nameof(Description));
      }
    }

    private IEnumerable<KeyValuePair<string, object>> cachedProperties;
    public IEnumerable<KeyValuePair<string, string>> Properties => Item.Properties
        .Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value.GetType().Name));

    public void ActivateProperty(string propertyName) {
      if(!Item.Properties.Any(x => x.Key == propertyName)) {
        Item.AddProperty(cachedProperties.Where(x => x.Key == propertyName).Single());
        OnPropertyChange(this, nameof(Properties));
      }
    }
    public void DeactivateProperty(string propertyName) {
      if (Item.Properties.Any(x => x.Key == propertyName)) {
        Item.RemoveProperty(propertyName);
        OnPropertyChange(this, nameof(Properties));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public JsonItemVM(JsonItem item) {
      Item = item;
      cachedProperties = Item.Properties.ToArray();
    }

    protected void OnPropertyChange(object sender, string propertyName) {
      // Make a temporary copy of the event to avoid possibility of
      // a race condition if the last subscriber unsubscribes
      // immediately after the null check and before the event is raised.
      // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-raise-base-class-events-in-derived-classes
      
      PropertyChangedEventHandler tmp = PropertyChanged;
      tmp?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
