#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.PluginAdministrator {
  /// <summary>
  /// Wraps a ListView and adds functionality to automatically check and uncheck dependencies of plugins.
  /// </summary>
  internal partial class PluginListView : UserControl {
    public event ItemCheckedEventHandler ItemChecked;

    private IEnumerable<IPluginDescription> plugins;
    public IEnumerable<IPluginDescription> Plugins {
      get { return plugins; }
      set {
        plugins = value;
        checkedPlugins.Clear();
        UpdateControls();
      }
    }

    private List<IPluginDescription> checkedPlugins = new List<IPluginDescription>();
    public IEnumerable<IPluginDescription> CheckedPlugins {
      get {
        return checkedPlugins;
      }
    }

    public PluginListView() {
      InitializeComponent();
    }

    public void CheckPlugin(IPluginDescription plugin) {
      MarkPluginChecked(plugin);
      listView.CheckItems(new IPluginDescription[] { plugin }.Select(x => FindItemsForPlugin(x).Single()));
    }


    private void UpdateControls() {
      if (plugins != null) {
        listView.Items.Clear();
        listView.SuppressItemCheckedEvents = true;
        foreach (var plugin in plugins) {
          listView.Items.Add(CreateListViewItem(plugin));
        }
        listView.SuppressItemCheckedEvents = false;
      }
    }

    private ListViewItem CreateListViewItem(IPluginDescription plugin) {
      var item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString() });
      item.Checked = (from p in checkedPlugins where p.Name == plugin.Name where p.Version == plugin.Version select p).Any();
      item.Tag = plugin;
      return item;
    }

    private void listView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      List<IPluginDescription> modifiedPlugins = new List<IPluginDescription>();
      if (e.Item.Checked) {
        foreach (ListViewItem item in listView.SelectedItems) {
          var plugin = (IPluginDescription)item.Tag;
          // also check all dependencies
          MarkPluginChecked(plugin);
          modifiedPlugins.Add(plugin);
          foreach (var dep in GetAllDependencies(plugin)) {
            MarkPluginChecked(dep);
            modifiedPlugins.Add(dep);
          }
        }
        listView.CheckItems(modifiedPlugins.Select(x => FindItemsForPlugin(x).Single()));
        OnItemChecked(e);
      } else {
        foreach (ListViewItem item in listView.SelectedItems) {
          var plugin = (IPluginDescription)item.Tag;
          // also uncheck all dependent plugins
          MarkPluginUnchecked(plugin);
          modifiedPlugins.Add(plugin);
          foreach (var dep in GetAllDependents(plugin)) {
            MarkPluginUnchecked(dep);
            modifiedPlugins.Add(dep);
          }

        }
        listView.UncheckItems(modifiedPlugins.Select(x => FindItemsForPlugin(x).Single()));
        OnItemChecked(e);
      }
    }

    private void MarkPluginChecked(IPluginDescription plugin) {
      var matching = from p in checkedPlugins
                     where p.Name == plugin.Name
                     where p.Version == plugin.Version
                     select p;
      if (!matching.Any()) checkedPlugins.Add(plugin);
    }

    private void MarkPluginUnchecked(IPluginDescription plugin) {
      checkedPlugins = (from p in checkedPlugins
                        where p.Name != plugin.Name ||
                              p.Version != plugin.Version
                        select p).ToList();
    }

    private IEnumerable<ListViewItem> FindItemsForPlugin(IPluginDescription plugin) {
      return from item in listView.Items.OfType<ListViewItem>()
             let p = item.Tag as IPluginDescription
             where p.Name == plugin.Name
             where p.Version == plugin.Version
             select item;
    }

    private IEnumerable<IPluginDescription> GetAllDependents(IPluginDescription plugin) {
      return from p in plugins
             let matchingEntries = from dep in GetAllDependencies(p)
                                   where dep.Name == plugin.Name
                                   where dep.Version == plugin.Version
                                   select dep
             where matchingEntries.Any()
             select p;
    }

    private IEnumerable<IPluginDescription> GetAllDependencies(IPluginDescription plugin) {
      foreach (var dep in plugin.Dependencies) {
        foreach (var recDep in GetAllDependencies(dep)) {
          yield return recDep;
        }
        yield return dep;
      }
    }

    private void OnItemChecked(ItemCheckedEventArgs e) {
      if (ItemChecked != null) ItemChecked(this, e);
    }
  }
}
