using System;
using System.Diagnostics;
using System.Data.Common;
using Persistence.Properties;

namespace Persistence {
  /// <summary>
  /// combines multiple used static methods into one class
  /// </summary>
  public class DatabaseUtil {

    public static bool ProductionDatabase { get; set; }

    /// <summary>
    /// creates and returns a database connection, if possible
    /// </summary>
    /// <returns>database connection (could be null)</returns>
    public static DataClassesDataContext createDataClassesDataContext() {
      return new DataClassesDataContext(
        ProductionDatabase ? new Settings().DatabaseConnectionString : new Settings().DatabaseConnectionStringTesting);
    }

    /// <summary>
    /// creates a new database out of the LINQ to SQL classes
    /// </summary>
    /// <param name="db">DataClassesDataContext</param>
    public static void createDatabase(DataClassesDataContext db) {
      // delete old database
      if (db.DatabaseExists()) {
        db.DeleteDatabase();
      }

      // create new database
      db.CreateDatabase();

      DbCommand command = db.Connection.CreateCommand();
      command.CommandText = "CREATE UNIQUE NONCLUSTERED INDEX [IDXRoleName] ON [dbo].[HeuristicLabRole]( 	[roleName] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]";
      command.ExecuteNonQuery();
      command.CommandText = "CREATE UNIQUE NONCLUSTERED INDEX [IDXUserName] ON [dbo].[HeuristicLabUser]( 	[UserName] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]";
      command.ExecuteNonQuery();
      command.CommandText = "CREATE UNIQUE NONCLUSTERED INDEX [IDXUserEmail] ON [dbo].[HeuristicLabUser]( 	[Email] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]";
      command.ExecuteNonQuery();
    }
  }
}
