using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Tracing;
using HeuristicLab.Persistence.Interfaces.Tokens;

namespace HeuristicLab.Persistence.Core {
  
  public class ConfigurationService {

    private static ConfigurationService instance;
    private readonly Dictionary<IFormat, Configuration> customConfigurations;
    public Dictionary<IFormat, List<IFormatter>> Formatters { get; private set; }
    public List<IDecomposer> Decomposers { get; private set; }
    
    public static ConfigurationService Instance {
      get {
        if (instance == null)
          instance = new ConfigurationService();
        return instance;
      }
    }

    private ConfigurationService() {
      Formatters = new Dictionary<IFormat, List<IFormatter>>();
      Decomposers = new List<IDecomposer>();
      customConfigurations = new Dictionary<IFormat, Configuration>();      
      Reset();
      LoadSettings();
    }

    public void LoadSettings() {
      try {
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
      } catch (Exception e) {
        Logger.Warn("Could not load settings.", e);        
      }
    }

    public void SaveSettings() {      
      Serializer serializer = new Serializer(
        customConfigurations,
        GetDefaultConfig(XmlFormat.Instance),
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
        if ( a != defaultAssembly )
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
        if (t.GetInterface(typeof (IFormatter).FullName) != null) {
          try {
            IFormatter formatter = (IFormatter) Activator.CreateInstance(t, true);
            if ( ! Formatters.ContainsKey(formatter.Format) ) {
              Formatters.Add(formatter.Format, new List<IFormatter>());
            }
            Formatters[formatter.Format].Add(formatter);
            Logger.Debug("discovered formatter " + t.VersionInvariantName());
          } catch (MissingMethodException e) {
            Logger.Warn("Could not instantiate " + t.VersionInvariantName(), e);            
          } catch (ArgumentException e) {
            Logger.Warn("Could not instantiate " + t.VersionInvariantName(), e);
          }
        }
        if (t.GetInterface(typeof (IDecomposer).FullName) != null) {
          try {
            Decomposers.Add((IDecomposer) Activator.CreateInstance(t, true));
            Logger.Debug("discovered decomposer " + t.VersionInvariantName());
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
      foreach ( IFormatter f in Formatters[format] ) {
        if ( ! formatterConfig.ContainsKey(f.Type) )
          formatterConfig.Add(f.Type, f);
      }
      return new Configuration(formatterConfig, Decomposers);
    }

    public Configuration GetConfiguration(IFormat format) {
      if (customConfigurations.ContainsKey(format))
        return customConfigurations[format];
      return GetDefaultConfig(format);
    }

    public void DefineConfiguration(IFormat format, Configuration configuration) {
      customConfigurations[format] = configuration;
      SaveSettings();
    }

  }
  
}