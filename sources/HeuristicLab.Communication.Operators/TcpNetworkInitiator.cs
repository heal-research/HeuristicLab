using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class TcpNetworkInitiator : OperatorBase {

    public override string Description {
      get { return @"Initiates a TCP socket, it opens one socket for sending and one socket for listening as specified in the configuration"; }
    }

    public TcpNetworkInitiator() {
      AddVariableInfo(new VariableInfo("DriverConfiguration", "A configuration object", typeof(TcpNetworkDriverConfiguration), VariableKind.In));
      AddVariableInfo(new VariableInfo("NetworkConnection", "A connection object", typeof(SocketData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      TcpNetworkDriverConfiguration config = GetVariableValue<TcpNetworkDriverConfiguration>("DriverConfiguration", scope, true);
      SocketData socket = new SocketData();
      socket.Initialize(config);
      while (!socket.Connect()) ;

      IVariableInfo info = GetVariableInfo("NetworkConnection");
      if (info.Local)
        AddVariable(new Variable(info.ActualName, socket));
      else
        scope.AddVariable(new Variable(scope.TranslateName(info.FormalName), socket));

      return null;
    }
  }
}
