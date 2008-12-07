using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess {
  /// <summary>
  /// Transaction manager for the DB access layer
  /// </summary>
  public interface ITransactionManager {
    /// <summary>
    /// This event is fired when an update occurs
    /// </summary>
    event EventHandler OnUpdate; 

    /// <summary>
    /// Enables the auto update of the DB
    /// </summary>
    /// <param name="interval"></param>
    void EnableAutoUpdate(TimeSpan interval);

    /// <summary>
    /// Disables the auto update of the DB
    /// </summary>
    void DisableAutoUpdate();

    /// <summary>
    /// Update the DB from the cache
    /// </summary>
    void UpdateDB();
  }
}
