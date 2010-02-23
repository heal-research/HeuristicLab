using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using Netron.Diagramming.Core;
using System.Drawing;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  internal class ConnectionInfo : Item, IConnectionInfo {
    #region static fields
    private static LinePenStyle penStyle;
    static ConnectionInfo() {
      ConnectionInfo.penStyle = new LinePenStyle();
      ConnectionInfo.penStyle.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
    }
    #endregion

    public ConnectionInfo()
      : base() {
    }

    private Point from;
    public Point From {
      get { return this.from; }
      set {
        if (this.from != value) {
          this.from = value;
          this.OnChanged();
        }
      }
    }

    private Point to;
    public Point To {
      get { return this.to; }
      set {
        if (this.to != value) {
          this.to = value;
          this.OnChanged();
        }
      }
    }

    public IConnection CreateConnection() {
      IConnection connection = new Connection(this.From, this.To);
      connection.PenStyle = ConnectionInfo.penStyle;
      return connection;
    }
  }
}
