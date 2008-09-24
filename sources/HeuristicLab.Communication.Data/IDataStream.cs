using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;

namespace HeuristicLab.Communication.Data {
  public interface IDataStream : IItem {
    void Initialize(IDriverConfiguration configuration);
    bool Connect();
    void Close();

    void Write(string s);
    string Read();
  }
}
