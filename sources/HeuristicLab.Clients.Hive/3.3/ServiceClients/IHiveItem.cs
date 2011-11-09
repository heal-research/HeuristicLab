using System;
using System.ComponentModel;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive {
  public interface IHiveItem : IItem, INotifyPropertyChanged {
    Guid Id { get; set; }
    bool Modified { get; }

    void Store();

    event EventHandler ModifiedChanged;
  }
}
