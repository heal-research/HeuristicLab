using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Tracing;

namespace HeuristicLab.Persistence.Core {
  
  public class ConfigurationService {

    private static ConfigurationService instance;
    private readonly Dictionary<IFormat, Configuration> customConfigurations;
    public readonly Dictionary<IFormat, List<IFormatter>> Formatters;
    public readonly List<IDecomposer> Decomposers;
    
    public static ConfigurationService Instance {
      get {
        if (instance == null)
          instance = new ConfigurationService();
        return instance;
      }
    }

    public ConfigurationService() {
      Formatters = new Dictionary<IFormat, List<IFormatter>>();
      Decomposers = new List<IDecomposer>();
      customConfigurations = new Dictionary<IFormat, Configuration>();      
      Reset();
      LoadSettings();
    }

    public void LoadSettings() {
      try {
        if (String.IsNullOrEmpty(Properties.Settings.Default.customConfigurations) ||
          String.IsNullOrEmpty(Properties.Settings.Default.customConfigurationsTypeCache))
          return;
        DeSerializer deSerializer = new DeSerializer(
          XmlParser.ParseTypeCache(
          new StringReader(
            Properties.Settings.Default.customConfigurationsTypeCache)));
        XmlParser parser = new XmlParser(
          new StringReader(
            Properties.Settings.Default.customConfigurations));
        var newCustomConfigurations = (Dictionary<IFormat, Configuration>)
          deSerializer.DeSerialize(parser);
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
      Properties.Settings.Default.customConfigurations =
        configurationString.ToString();
      Properties.Settings.Default.customConfigurationsTypeCache =
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
          } catch (MissingMethodException e) {
            Logger.Warn("Could not instantiate " + t.VersionInvariantName(), e);            
          }          
        }
        if (t.GetInterface(typeof (IDecomposer).FullName) != null) {
          try {
            Decomposers.Add((IDecomposer) Activator.CreateInstance(t, true));
          } catch (MissingMethodException e) {
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