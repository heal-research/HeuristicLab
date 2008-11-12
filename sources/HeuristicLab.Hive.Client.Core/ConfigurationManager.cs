using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Core {
  /// <summary>
  /// accesses the Server and sends his data (uuid, uptimes, hardware config) 
  /// </summary>
  public class ConfigurationManager {
    private static ConfigurationManager instance = null;
    private Guid guid;

    public static ConfigurationManager GetInstance() {
      if (instance == null) {
        instance = new ConfigurationManager();
      }
      return instance;
    }

    /// <summary>
    /// Constructor for the singleton, must recover Guid, Calendar, ...
    /// </summary>
    private ConfigurationManager() {
      //retrive GUID from XML file, or burn in hell. as in hell. not heaven.
      //this won't work this way. We need a plugin for XML Handling.
      if (Properties.Settings.Default.Guid == Guid.Empty) {
        guid = Guid.NewGuid();
        Properties.Settings.Default.Guid = guid;
      } else {
        guid = Properties.Settings.Default.Guid;
      }
    }

    public void Connect(Guid guid) {

    }

  }
}
