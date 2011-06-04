#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Tracing;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Provides a persistable configuration of primitive and composite serializers for
  /// all registered serial formats. Custom formats can be defined and will be saved
  /// for future sessions. A default configuration can be generated through reflection.
  /// 
  /// This class has only a single instance.
  /// </summary>
  public class ConfigurationService {

    private static ConfigurationService instance;
    private static object locker = new object();
    private readonly Dictionary<IFormat, Configuration> customConfigurations;

    /// <summary>
    /// List of all available primitive serializers.
    /// </summary>
    public Dictionary<Type, List<IPrimitiveSerializer>> PrimitiveSerializers { get; private set; }

    /// <summary>
    /// List of all available composite serializers (discovered through reflection).
    /// </summary>
    public List<ICompositeSerializer> CompositeSerializers { get; private set; }

    /// <summary>
    /// List of all available formats (discovered through reflection).    
    /// </summary>
    public List<IFormat> Formats { get; private set; }

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <value>The singleton instance.</value>
    public static ConfigurationService Instance {
      get {
        lock (locker) {
          if (instance == null)
            instance = new ConfigurationService();
          return instance;
        }
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

    /// <summary>
    /// Loads the settings.
    /// </summary>
    public void LoadSettings() {
      LoadSettings(false);
    }

    /// <summary>
    /// Loads the settings.
    /// </summary>
    /// <param name="throwOnError">if set to <c>true</c> throw on error.</param>
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

    /// <summary>
    /// Tries to load the settings (i.e custom configurations).
    /// </summary>
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

    /// <summary>
    /// Saves the settings (i.e custom configurations).
    /// </summary>
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


    /// <summary>
    /// Rediscover available serializers and discard all custom configurations.
    /// </summary>
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

    private class PriortiySorter : IComparer<ICompositeSerializer> {
      public int Compare(ICompositeSerializer x, ICompositeSerializer y) {
        return y.Priority - x.Priority;
      }
    }

    /// <summary>
    /// Sorts the composite serializers according to their priority.
    /// </summary>
    protected void SortCompositeSerializers() {
      CompositeSerializers.Sort(new PriortiySorter());
    }

    /// <summary>
    /// Discovers serializers from an assembly.
    /// </summary>
    /// <param name="a">An Assembly.</param>
    protected void DiscoverFrom(Assembly a) {
      try {
        foreach (Type t in a.GetTypes()) {
          if (t.GetInterface(typeof(IPrimitiveSerializer).FullName) != null &&
              !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null && !t.ContainsGenericParameters) {
            IPrimitiveSerializer primitiveSerializer =
              (IPrimitiveSerializer)Activator.CreateInstance(t, true);
            if (!PrimitiveSerializers.ContainsKey(primitiveSerializer.SerialDataType))
              PrimitiveSerializers.Add(primitiveSerializer.SerialDataType, new List<IPrimitiveSerializer>());
            PrimitiveSerializers[primitiveSerializer.SerialDataType].Add(primitiveSerializer);
          }
          if (t.GetInterface(typeof(ICompositeSerializer).FullName) != null &&
              !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null && !t.ContainsGenericParameters) {
            CompositeSerializers.Add((ICompositeSerializer)Activator.CreateInstance(t, true));
          }
          if (t.GetInterface(typeof(IFormat).FullName) != null &&
             !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null && !t.ContainsGenericParameters) {
            IFormat format = (IFormat)Activator.CreateInstance(t, true);
            Formats.Add(format);
          }
        }
      } catch (ReflectionTypeLoadException e) {
        Logger.Warn("could not analyse assembly: " + a.FullName, e);
      }
    }

    /// <summary>
    /// Get the default (automatically discovered) configuration for a certain format.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>The default (auto discovered) configuration.</returns>
    public Configuration GetDefaultConfig(IFormat format) {
      Dictionary<Type, IPrimitiveSerializer> primitiveConfig = new Dictionary<Type, IPrimitiveSerializer>();
      if (PrimitiveSerializers.ContainsKey(format.SerialDataType)) {
        foreach (IPrimitiveSerializer f in PrimitiveSerializers[format.SerialDataType]) {
          if (!primitiveConfig.ContainsKey(f.SourceType))
            primitiveConfig.Add(f.SourceType, (IPrimitiveSerializer)Activator.CreateInstance(f.GetType()));
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
        CompositeSerializers.Where((d) => d.Priority > 0).Select(d => (ICompositeSerializer)Activator.CreateInstance(d.GetType())));
    }


    /// <summary>
    /// Get a configuration for a certain format. This returns a fresh copy of a custom configuration,
    /// if defined, otherwise returns the default (automatically discovered) configuration.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>A Configuration</returns>
    public Configuration GetConfiguration(IFormat format) {
      if (customConfigurations.ContainsKey(format))
        return customConfigurations[format].Copy();
      return GetDefaultConfig(format);
    }

    /// <summary>
    /// Define a new custom configuration for a ceratin format.
    /// </summary>
    /// <param name="configuration">The new configuration.</param>
    public void DefineConfiguration(Configuration configuration) {
      customConfigurations[configuration.Format] = configuration.Copy();
      SaveSettings();
    }

  }

}