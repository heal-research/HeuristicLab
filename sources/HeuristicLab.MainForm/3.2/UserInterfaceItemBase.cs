using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.MainForm {
  public abstract class UserInterfaceItemBase : IUserInterfaceItem{
    public abstract string Name { get; }

    #region IAction Members
    public abstract void Execute(IMainForm mainform);
    #endregion
  }
}
