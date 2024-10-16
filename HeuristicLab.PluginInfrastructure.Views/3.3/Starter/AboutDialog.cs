#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Advanced;

namespace HeuristicLab.PluginInfrastructure.Starter {
  /// <summary>
  /// Shows product, version and copyright information for HeuristicLab and all plugins.
  /// </summary>
  public partial class AboutDialog : Form {
    /// <summary>
    /// Creates a new about dialog with all plugins loaded in the current application.
    /// </summary>
    public AboutDialog() {
      InitializeComponent();
      var curAssembly = this.GetType().Assembly;
      productTextBox.Text = GetProduct(curAssembly);
      versionTextBox.Text = GetVersion();
      copyrightTextBox.Text = GetCopyright(curAssembly);
      imageList.Images.Add(HeuristicLab.PluginInfrastructure.Resources.Plugin);
      pictureBox.Image = HeuristicLab.PluginInfrastructure.Resources.HeuristicLabLogo;
      licenseTextBox.Text = HeuristicLab.PluginInfrastructure.Resources.LicenseText;
      UpdatePluginList(ApplicationManager.Manager.Plugins);
      ActiveControl = okButton;
    }

    /// <summary>
    /// Creates a new about dialog listing all plugins in the <paramref name="plugins"/> enumerable.
    /// </summary>
    /// <param name="plugins">Enumerable of plugins that should be listed.</param>
    public AboutDialog(IEnumerable<IPluginDescription> plugins)
      : this() {
      UpdatePluginList(plugins);
    }

    private void UpdatePluginList(IEnumerable<IPluginDescription> plugins) {
      pluginListView.Items.Clear();
      foreach (var plugin in plugins) {
        ListViewItem pluginItem = CreateListViewItem(plugin);
        pluginListView.Items.Add(pluginItem);
      }
      Util.ResizeColumns(pluginListView.Columns.OfType<ColumnHeader>());
    }

    private ListViewItem CreateListViewItem(IPluginDescription plugin) {
      ListViewItem item = new ListViewItem(new string[] { plugin.Name, plugin.Version.ToString(), plugin.Description });
      item.Tag = plugin;
      item.ImageIndex = 0;
      return item;
    }

    private string GetCopyright(Assembly asm) {
      AssemblyCopyrightAttribute attribute = GetAttribute<AssemblyCopyrightAttribute>(asm);
      return attribute.Copyright;
    }

    private string GetVersion() {
      return AssemblyHelpers.GetFileVersion(GetType().Assembly);
    }

    private string GetProduct(Assembly asm) {
      AssemblyProductAttribute attribute = GetAttribute<AssemblyProductAttribute>(asm);
      return attribute.Product;
    }

    private T GetAttribute<T>(Assembly asm) {
      return (T)asm.GetCustomAttributes(typeof(T), false).Single();
    }

    private void okButton_Click(object sender, EventArgs e) {
      Close();
    }

    private void pluginListView_ItemActivate(object sender, EventArgs e) {
      if (pluginListView.SelectedItems.Count > 0) {
        PluginView view = new PluginView((IPluginDescription)pluginListView.SelectedItems[0].Tag);
        view.Show(this);
      }
    }

    private void licenseTextBox_LinkClicked(object sender, LinkClickedEventArgs e) {
      System.Diagnostics.Process.Start(e.LinkText);
    }

    private void webLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      System.Diagnostics.Process.Start(webLinkLabel.Text);
    }

    private void mailLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      System.Diagnostics.Process.Start("mailto:" + mailLinkLabel.Text);
    }
  }
}
