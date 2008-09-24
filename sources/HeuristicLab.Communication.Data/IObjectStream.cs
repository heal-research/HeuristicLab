using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.Communication.Data {
  public interface IObjectStream : IItem {
    void Initialize(IDriverConfiguration configuration);
    bool Connect();
    void Close();

    void Write(IItem item);
    IItem Read();
  }
}
