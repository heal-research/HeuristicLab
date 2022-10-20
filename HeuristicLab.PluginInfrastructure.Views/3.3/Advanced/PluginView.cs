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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class PluginView : Form {
    private const string IMAGE_KEY_PLUGIN = "Plugin";
    private const string IMAGE_KEY_ASSEMBLY = "Assembly";
    private const string IMAGE_KEY_FILE = "File";
    private const string IMAGE_KEY_DOCUMENT = "Document";

    private IPluginDescription plugin;

    public PluginView()
      : base() {
      InitializeComponent();
      PopulateImageList();
    }

    public PluginView(IPluginDescription plugin)
      : base() {
      InitializeComponent();
      PopulateImageList();

      this.plugin = plugin;
      this.Text = "Plugin Details: " + plugin.ToString();
      UpdateControls();
    }

    private void PopulateImageList() {
      pluginsImageList.Images.Add(IMAGE_KEY_PLUGIN, HeuristicLab.PluginInfrastructure.Resources.Plugin);
      filesImageList.Images.Add(IMAGE_KEY_ASSEMBLY, HeuristicLab.PluginInfrastructure.Resources.Assembly);
      filesImageList.Images.Add(IMAGE_KEY_FILE, HeuristicLab.PluginInfrastructure.Resources.File);
      filesImageList.Images.Add(IMAGE_KEY_DOCUMENT, HeuristicLab.PluginInfrastructure.Resources.Document);
    }

    public void UpdateControls() {
      string appDir = Path.GetDirectoryName(Application.ExecutablePath);
      nameTextBox.Text = plugin.Name;
      versionTextBox.Text = plugin.Version.ToString();
      contactTextBox.Text = CombineStrings(plugin.ContactName, plugin.ContactEmail);
      toolTip.SetToolTip(contactTextBox, contactTextBox.Text);
      descriptionTextBox.Text = plugin.Description;
      toolTip.SetToolTip(descriptionTextBox, plugin.Description);
      var localPlugin = plugin as PluginDescription;
      if (localPlugin != null) {
        stateTextBox.Text = localPlugin.PluginState.ToString();
        if (!string.IsNullOrEmpty(localPlugin.LoadingErrorInformation))
          errorTextBox.Text = localPlugin.LoadingErrorInformation.Replace(Environment.NewLine, " ");
        toolTip.SetToolTip(stateTextBox, stateTextBox.Text + Environment.NewLine + errorTextBox.Text);
        toolTip.SetToolTip(errorTextBox, errorTextBox.Text);
      }
      foreach (PluginDescription dependency in plugin.Dependencies) {
        var depItem = new ListViewItem(new string[] { dependency.Name, dependency.Version.ToString(), dependency.Description });
        depItem.Tag = dependency;
        depItem.ImageKey = IMAGE_KEY_PLUGIN;
        dependenciesListView.Items.Add(depItem);
      }
      foreach (var file in plugin.Files) {
        string displayedFileName = file.Name.Replace(appDir, string.Empty);
        displayedFileName = displayedFileName.TrimStart(Path.DirectorySeparatorChar);
        var fileItem = new ListViewItem(new string[] { displayedFileName, file.Type.ToString() });
        if (file.Type == PluginFileType.Assembly) {
          fileItem.ImageKey = IMAGE_KEY_ASSEMBLY;
        } else if (file.Type == PluginFileType.License) {
          fileItem.ImageKey = IMAGE_KEY_DOCUMENT;
        } else fileItem.ImageKey = IMAGE_KEY_FILE;
        filesListView.Items.Add(fileItem);
      }
      Util.ResizeColumns(dependenciesListView.Columns.OfType<ColumnHeader>());
      Util.ResizeColumns(filesListView.Columns.OfType<ColumnHeader>());

      showLicenseButton.Enabled = !string.IsNullOrEmpty(plugin.LicenseText);
    }

    private string CombineStrings(string a, string b) {
      if (string.IsNullOrEmpty(a))
        // a is empty
        if (!string.IsNullOrEmpty(b)) return CombineStrings(b, string.Empty);
        else return string.Empty;
      // a is not empty
      else if (string.IsNullOrEmpty(b)) return a;
      // and b are not empty
      else return a + ", " + b;
    }

    private void dependenciesListView_ItemActivate(object sender, EventArgs e) {
      if (dependenciesListView.SelectedItems.Count > 0) {
        var dep = (PluginDescription)dependenciesListView.SelectedItems[0].Tag;
        PluginView view = new PluginView(dep);
        view.Show(this);
      }
    }

    private void showLicenseButton_Click(object sender, EventArgs e) {
      LicenseView view = new LicenseView(plugin);
      view.Show(this);
    }
  }
}
