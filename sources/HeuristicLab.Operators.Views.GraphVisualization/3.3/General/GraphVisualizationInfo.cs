using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Collections;
using HeuristicLab.Common;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  [StorableClass]
  public class GraphVisualizationInfo : DeepCloneable, IGraphVisualizationInfo {
    public GraphVisualizationInfo() {
      this.shapeInfos = new ObservableSet<IShapeInfo>();
      this.connectionInfos = new ObservableSet<IConnectionInfo>();
    }
    #region IGraphVisualizationInfo Members
    [Storable]
    private IShapeInfo initialShape;
    public virtual IShapeInfo InitialShape {
      get { return this.initialShape; }
      set {
        if (this.initialShape != value) {
          this.initialShape = value;
          this.OnInitialShapeChanged();
        }
      }
    }

    [Storable]
    protected ObservableSet<IShapeInfo> shapeInfos;
    public INotifyObservableCollectionItemsChanged<IShapeInfo> ObserveableShapeInfos {
      get { return shapeInfos; }
    }
    public IEnumerable<IShapeInfo> ShapeInfos {
      get { return shapeInfos; }
    }

    [Storable]
    protected ObservableSet<IConnectionInfo> connectionInfos;
    public INotifyObservableCollectionItemsChanged<IConnectionInfo> ObservableConnectionInfos {
      get { return connectionInfos; }
    }
    public IEnumerable<IConnectionInfo> ConnectionInfos {
      get { return connectionInfos; }
    }

    public virtual void AddShapeInfo(IShapeInfo shapeInfo) {
      this.shapeInfos.Add(shapeInfo);
    }
    public virtual void RemoveShapeInfo(IShapeInfo shapeInfo) {
      this.shapeInfos.Remove(shapeInfo);
      this.connectionInfos.RemoveWhere(c => c.From == shapeInfo); 
      this.connectionInfos.RemoveWhere(c => c.To == shapeInfo);
    }

    public virtual void AddConnectionInfo(IConnectionInfo connectionInfo) {
      this.connectionInfos.Add(connectionInfo);
    }
    public virtual void RemoveConnectionInfo(IConnectionInfo connectionInfo) {
      this.connectionInfos.Remove(connectionInfo);
    }

    public IEnumerable<IConnectionInfo> GetConnectionInfos(IShapeInfo shapeInfo) {
      return this.ConnectionInfos.Where(c => c.From == shapeInfo || c.To == shapeInfo);
    }
    public IEnumerable<IConnectionInfo> GetConnectionInfos(IShapeInfo shapeInfo, string connector) {
      return this.ConnectionInfos.Where(c => (c.From == shapeInfo && c.ConnectorFrom == connector) ||
        (c.To == shapeInfo && c.ConnectorTo == connector));
    }

    public event EventHandler InitialShapeChanged;
    protected virtual void OnInitialShapeChanged() {
      EventHandler handler = this.InitialShapeChanged;
      if (handler != null)
        this.InitialShapeChanged(this, EventArgs.Empty);
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      GraphVisualizationInfo clone = (GraphVisualizationInfo)base.Clone(cloner);
      clone.shapeInfos = new ObservableSet<IShapeInfo>(this.shapeInfos.Select(x => (IShapeInfo)cloner.Clone(x)));
      clone.connectionInfos = new ObservableSet<IConnectionInfo>(this.connectionInfos.Select(x => (IConnectionInfo)cloner.Clone(x)));
      if (this.initialShape != null)
        clone.initialShape = (IShapeInfo)this.initialShape.Clone(cloner);
      return clone;
    }

    #region IContent Members
    [Storable]
    private bool readOnlyView;
    public virtual bool ReadOnlyView {
      get { return readOnlyView; }
      set {
        if (readOnlyView != value) {
          readOnlyView = value;
          OnReadOnlyViewChanged();
        }
      }
    }
    public event EventHandler ReadOnlyViewChanged;
    protected virtual void OnReadOnlyViewChanged() {
      EventHandler handler = ReadOnlyViewChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
