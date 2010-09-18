using Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests {
  /// <summary>
  ///This is a abstract test class for all HeuristicLab unit tests
  ///</summary>
  public abstract class AbstractHeuristicLabTest {

    /// <summary>
    /// shared database for all test methods
    /// </summary>
    protected Persistence.DataClassesDataContext db;

    /// <summary>
    /// constructor creates database connection for all other test methods
    /// </summary>
    public AbstractHeuristicLabTest() {
      Persistence.DatabaseUtil.ProductionDatabase = false;
      db = Persistence.DatabaseUtil.createDataClassesDataContext();
    }

    /// <summary>
    /// creates and opens a local database out of the DataClasses and checks connection state
    /// </summary>
    [TestInitialize()]
    public virtual void updateDBConnection() {
      Assert.IsNotNull(db);
      DatabaseUtil.createDatabase(db);
      if (db.Connection.State != System.Data.ConnectionState.Open) {
        db.Connection.Open();
      }
      Assert.AreEqual<System.Data.ConnectionState>(System.Data.ConnectionState.Open, db.Connection.State);
    }

    /// <summary>
    /// closes db connection
    /// </summary>
    [TestCleanup()]
    public virtual void closeDBConnection() {
      if (db.Connection.State == System.Data.ConnectionState.Open) {
        db.Connection.Close();
      }
      Assert.AreEqual<System.Data.ConnectionState>(System.Data.ConnectionState.Closed, db.Connection.State);
    }
  }
}
