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
    public Dictionary<Type, List<IPrimitiveSerializer>> PrimitiveSerializers { get; private set; }
    public List<ICompositeSerializer> CompositeSerializers { get; private set; }
    public List<IFormat> Formats { get; private set; }

    public static ConfigurationService Instance {
      get {
        if (instance == null)
          instance = new ConfigurationService();
        return instance;
      }
    }

    private ConfigurationService() {
      PrimitiveSerializers = new Dictionary<Type, List<IPrimitiveSerializer>>();
      CompositeSerializers = new List<ICompositeSerializer>();
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
      PrimitiveSerializers.Clear();
      CompositeSerializers.Clear();
      Assembly defaultAssembly = Assembly.GetExecutingAssembly();
      DiscoverFrom(defaultAssembly);
      try {
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
          if (a != defaultAssembly)
            DiscoverFrom(a);
      } catch (AppDomainUnloadedException x) {
        Logger.Warn("could not get list of assemblies, AppDomain has already been unloaded", x);
      }
      SortCompositeSerializers();
    }

    class PriortiySorter : IComparer<ICompositeSerializer> {
      public int Compare(ICompositeSerializer x, ICompositeSerializer y) {
        return y.Priority - x.Priority;
      }
    }

    protected void SortCompositeSerializers() {
      CompositeSerializers.Sort(new PriortiySorter());
    }

    protected void DiscoverFrom(Assembly a) {
      try {
        foreach (Type t in a.GetTypes()) {
          if (t.GetInterface(typeof(IPrimitiveSerializer).FullName) != null) {
            try {
              IPrimitiveSerializer primitiveSerializer =
                (IPrimitiveSerializer)Activator.CreateInstance(t, true);
              if (!PrimitiveSerializers.ContainsKey(primitiveSerializer.SerialDataType)) {
                PrimitiveSerializers.Add(primitiveSerializer.SerialDataType, new List<IPrimitiveSerializer>());
              }
              PrimitiveSerializers[primitiveSerializer.SerialDataType].Add(primitiveSerializer);
              Logger.Debug(String.Format("discovered primitive serializer {0} ({1} -> {2})",
                t.VersionInvariantName(),
                primitiveSerializer.SourceType.AssemblyQualifiedName,
                primitiveSerializer.SerialDataType.AssemblyQualifiedName));
            } catch (MissingMethodException e) {
              Logger.Warn("Could not instantiate " + t.AssemblyQualifiedName, e);
            } catch (ArgumentException e) {
              Logger.Warn("Could not instantiate " + t.AssemblyQualifiedName, e);
            }
          }
          if (t.GetInterface(typeof(ICompositeSerializer).FullName) != null) {
            try {
              CompositeSerializers.Add((ICompositeSerializer)Activator.CreateInstance(t, true));
              Logger.Debug("discovered composite serializer " + t.AssemblyQualifiedName);
            } catch (MissingMethodException e) {
              Logger.Warn("Could not instantiate " + t.AssemblyQualifiedName, e);
            } catch (ArgumentException e) {
              Logger.Warn("Could not instantiate " + t.AssemblyQualifiedName, e);
            }
          }
          if (t.GetInterface(typeof(IFormat).FullName) != null) {
            try {
              IFormat format = (IFormat)Activator.CreateInstance(t, true);
              Formats.Add(format);
              Logger.Debug(String.Format("discovered format {0} ({2}) with serial data {1}.",
                format.Name,
                format.SerialDataType,
                t.AssemblyQualifiedName));
            } catch (MissingMethodException e) {
              Logger.Warn("Could not instantiate " + t.AssemblyQualifiedName, e);
            } catch (ArgumentException e) {
              Logger.Warn("Could not instantiate " + t.AssemblyQualifiedName, e);
            }
          }
        }
      } catch (ReflectionTypeLoadException e) {
        Logger.Warn("could not analyse assembly: " + a.FullName, e);
      }
    }

    public Configuration GetDefaultConfig(IFormat format) {
      Dictionary<Type, IPrimitiveSerializer> primitiveConfig = new Dictionary<Type, IPrimitiveSerializer>();
      if (PrimitiveSerializers.ContainsKey(format.SerialDataType)) {
        foreach (IPrimitiveSerializer f in PrimitiveSerializers[format.SerialDataType]) {
          if (!primitiveConfig.ContainsKey(f.SourceType))
            primitiveConfig.Add(f.SourceType, f);
        }
      } else {
        Logger.Warn(String.Format(
          "No primitive serializers found for format {0} with serial data type {1}",
          format.GetType().AssemblyQualifiedName,
          format.SerialDataType.AssemblyQualifiedName));
      }
      return new Configuration(
        format,
        primitiveConfig.Values,
        CompositeSerializers.Where((d) => d.Priority > 0));
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