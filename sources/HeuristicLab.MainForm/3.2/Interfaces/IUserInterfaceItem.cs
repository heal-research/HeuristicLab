using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.MainForm {
  public interface IUserInterfaceItem : IAction {
    string Name { get; }
  }
}
