using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Tracing;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Auxiliary;

namespace HeuristicLab.Persistence.Core {

  public class ConfigurationService {

    private static ConfigurationService instance;
    private readonly Dictionary<IFormat, Configuration> customConfigurations;
    public Dictionary<Type, List<IFormatter>> Formatters { get; private set; }
    public List<IDecomposer> Decomposers { get; private set; }
    public List<IFormat> Formats { get; private set; }

    public static ConfigurationService Instance {
      get {
        if (instance == null)
          instance = new ConfigurationService();
        return instance;
      }
    }

    private ConfigurationService() {
      Formatters = new Dictionary<Type, List<IFormatter>>();
      Decomposers = new List<IDecomposer>();
      customConfigurations = new Dictionary<IFormat, Configuration>();
      Formats = new List<IFormat>();
      Reset();
      LoadSettings();
    }

    public void LoadSettings() {
      LoadSettings(false);
    }

    public void LoadSettings(bool throwOnError) {
      try {
        TryLoadSettings();
      } catch (Exception e) {
        if (throwOnError) {
          throw new PersistenceException("Could not load persistence settings.", e);
        } else {
          Logger.Warn("Could not load settings.", e);
        }
      }
    }

    protected void TryLoadSettings() {
      if (String.IsNullOrEmpty(Properties.Settings.Default.CustomConfigurations) ||
          String.IsNullOrEmpty(Properties.Settings.Default.CustomConfigurationsTypeCache))
        return;
      Deserializer deSerializer = new Deserializer(
        XmlParser.ParseTypeCache(
        new StringReader(
          Properties.Settings.Default.CustomConfigurationsTypeCache)));
      XmlParser parser = new XmlParser(
        new StringReader(
          Properties.Settings.Default.CustomConfigurations));
      var newCustomConfigurations = (Dictionary<IFormat, Configuration>)
        deSerializer.Deserialize(parser);
      foreach (var config in newCustomConfigurations) {
        customConfigurations[config.Key] = config.Value;
      }
    }

    protected void SaveSettings() {
      Serializer serializer = new Serializer(
        customConfigurations,
        GetDefaultConfig(new XmlFormat()),
        "CustomConfigurations");
      XmlGenerator generator = new XmlGenerator();
      StringBuilder configurationString = new StringBuilder();
      foreach (ISerializationToken token in serializer) {
        configurationString.Append(generator.Format(token));
      }
      StringBuilder configurationTypeCacheString = new StringBuilder();
      foreach (string s in generator.Format(serializer.TypeCache))
        configurationTypeCacheString.Append(s);
      Properties.Settings.Default.CustomConfigurations =
        configurationString.ToString();
      Properties.Settings.Default.CustomConfigurationsTypeCache =
        configurationTypeCacheString.ToString();
      Properties.Settings.Default.Save();
    }

    public void Reset() {
      customConfigurations.Clear();
      Formatters.Clear();
      Decomposers.Clear();
      Assembly defaultAssembly = Assembly.GetExecutingAssembly();
      DiscoverFrom(defaultAssembly);
      foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        if (a != defaultAssembly)
          DiscoverFrom(a);
      SortDecomposers();
    }

    class PriortiySorter : IComparer<IDecomposer> {
      public int Compare(IDecomposer x, IDecomposer y) {
        return y.Priority - x.Priority;
      }
    }

    protected void SortDecomposers() {
      Decomposers.Sort(new PriortiySorter());
    }

    protected void DiscoverFrom(Assembly a) {
      foreach (Type t in a.GetTypes()) {
        if (t.GetInterface(typeof(IFormatter).FullName) != null) {
          try {
            IFormatter formatter = (IFormatter)Activator.CreateInstance(t, true);
            if (!Formatters.ContainsKey(formatter.SerialDataType)) {
              Formatters.Add(formatter.SerialDataType, new List<IFormatter>());
            }
            Formatters[formatter.SerialDataType].Add(formatter);
            Logger.Debug(String.Format("discovered formatter {0} ({1} -> {2})",
              t.VersionInvariantName(),
              formatter.SourceType.VersionInvariantName(),
              formatter.SerialDataType.VersionInvariantName()));
          } catch (MissingMethodException e) {
            Logger.Warn("Could not instantiate " + t.VersionInvariantName(), e);
          } catch (ArgumentException e) {
            Logger.Warn("Could not instantiate " + t.VersionInvariantName(), e);
          }
        }
        if (t.GetInterface(typeof(IDecomposer).FullName) != null) {
          try {
            Decomposers.Add((IDecomposer)Activator.CreateInstance(t, true));
            Logger.Debug("discovered decomposer " + t.VersionInvariantName());
          } catch (MissingMethodException e) {
            Logger.Warn("Could not instantiate " + t.VersionInvariantName(), e);
          } catch (ArgumentException e) {
            Logger.Warn("Could not instantiate " + t.VersionInvariantName(), e);
          }
        }
        if (t.GetInterface(typeof(IFormat).FullName) != null) {
          try {
            IFormat format = (IFormat)Activator.CreateInstance(t, true);
            Formats.Add(format);
            Logger.Debug(String.Format("discovered format {0} ({2}) with serial data {1}.",
              format.Name,
              format.SerialDataType,
              t.VersionInvariantName()));
          } catch (MissingMethodException e) {
            Logger.Warn("Could not instantiate " + t.VersionInvariantName(), e);
          } catch (ArgumentException e) {
            Logger.Warn("Could not instantiate " + t.VersionInvariantName(), e);
          }
        }
      }
    }

    public Configuration GetDefaultConfig(IFormat format) {
      Dictionary<Type, IFormatter> formatterConfig = new Dictionary<Type, IFormatter>();
      if (Formatters.ContainsKey(format.SerialDataType)) {
        foreach (IFormatter f in Formatters[format.SerialDataType]) {
          if (!formatterConfig.ContainsKey(f.SourceType))
            formatterConfig.Add(f.SourceType, f);
        }
      } else {
        Logger.Warn(String.Format(
          "No formatters found for format {0} with serial data type {1}",
          format.GetType().VersionInvariantName(),
          format.SerialDataType.VersionInvariantName()));
      }
      return new Configuration(format, formatterConfig.Values, Decomposers.Where((d) => d.Priority > 0));
    }

    public Configuration GetConfiguration(IFormat format) {
      if (customConfigurations.ContainsKey(format))
        return customConfigurations[format];
      return GetDefaultConfig(format);
    }

    public void DefineConfiguration(Configuration configuration) {
      customConfigurations[configuration.Format] = configuration;
      SaveSettings();
    }

  }

}