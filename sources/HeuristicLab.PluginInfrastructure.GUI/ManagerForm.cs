#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace HeuristicLab.PluginInfrastructure.GUI {
  public partial class ManagerForm : Form {
    private List<PluginTag> allTags = new List<PluginTag>();
    private Dictionary<PluginTag, PluginAction> actions = new Dictionary<PluginTag, PluginAction>();
    private List<PluginDescription> allAvailablePlugins = new List<PluginDescription>();
    private string pluginDir = Application.StartupPath + "/" + HeuristicLab.PluginInfrastructure.Properties.Settings.Default.PluginDir;
    private string cacheDir = Application.StartupPath + "/" + HeuristicLab.PluginInfrastructure.GUI.Properties.Settings.Default.CacheDir;
    private string backupDir = Application.StartupPath + "/" + HeuristicLab.PluginInfrastructure.GUI.Properties.Settings.Default.BackupDir;
    private string tempDir = Application.StartupPath + "/" + HeuristicLab.PluginInfrastructure.GUI.Properties.Settings.Default.TempDir;
    private const string AVAILABLE_PLUGINS = "Available plugins";
    private const string DISABLED_PLUGINS = "Disabled plugins";
    private const string INSTALLED_PLUGINS = "Installed plugins";

    public ManagerForm() {
      InitializeComponent();
      InitializePlugins();
    }

    private void InitializePlugins() {
      listView.View = View.Details;
      listView.FullRowSelect = true;
      listView.Items.Clear();
      allTags.Clear();
      actions.Clear();

      publishButton.Enabled = false;
      publishMenuItem.Enabled = false;
      installButton.Enabled = false;
      installButton.Checked = false;
      installMenuItem.Enabled = false;
      installMenuItem.Checked = false;
      deleteButton.Enabled = false;
      deleteButton.Checked = false;
      deleteMenuItem.Enabled = false;
      deleteMenuItem.Checked = false;

      foreach(PluginInfo pluginInfo in PluginManager.Manager.ActivePlugins) {
        // create a new PluginAction tag for the plugin
        PluginTag tag = new PluginTag(allTags, pluginInfo, PluginState.Installed);
        allTags.Add(tag);
        // add to "installed plugins" item
        ListViewItem installedPluginsItem = new ListViewItem(pluginInfo.Name);
        installedPluginsItem.Tag = tag;
        installedPluginsItem.ImageIndex = 0;
        installedPluginsItem.Group = listView.Groups[INSTALLED_PLUGINS];
        installedPluginsItem.SubItems.Add(pluginInfo.Version.ToString());
        listView.Items.Add(installedPluginsItem);
      }
      foreach(PluginInfo pluginInfo in PluginManager.Manager.DisabledPlugins) {
        PluginTag tag = new PluginTag(allTags, pluginInfo, PluginState.Disabled);
        allTags.Add(tag);
        ListViewItem disabledPluginsItem = new ListViewItem(pluginInfo.Name);
        disabledPluginsItem.Tag = tag;
        disabledPluginsItem.ImageIndex = 0;
        disabledPluginsItem.Group = listView.Groups[DISABLED_PLUGINS];
        disabledPluginsItem.SubItems.Add(pluginInfo.Version.ToString());
        listView.Items.Add(disabledPluginsItem);
      }

      allAvailablePlugins = FilterMostRecentPluginVersions(allAvailablePlugins);
      // find all plugins that are installed for which a new version is available
      List<PluginDescription> upgrades = FindUpgrades(allTags, allAvailablePlugins);
      // find all available plugins that are not installed and new (= new name not new version) since the last update
      List<PluginDescription> newPlugins = FindNewPlugins(allTags, allAvailablePlugins);
      // find all plugins that are available (not installed) for which a new version has been released since the last update
      List<PluginDescription> overridingPlugins = FindOverridingPlugins(allTags, allAvailablePlugins);
      newPlugins.ForEach(delegate(PluginDescription plugin) {
        PluginTag tag = new PluginTag(allTags, plugin, PluginState.Available);
        allTags.Add(tag);
        ListViewItem availableItem = new ListViewItem(plugin.Name);
        availableItem.Tag = tag;
        availableItem.ImageIndex = 0;
        availableItem.Group = listView.Groups[AVAILABLE_PLUGINS];
        availableItem.SubItems.Add(plugin.Version.ToString());
        listView.Items.Add(availableItem);
      });
      upgrades.ForEach(delegate(PluginDescription upgrade) {
        // find the installed plugins that have the same name
        List<PluginTag> oldPlugins = allTags.FindAll(delegate(PluginTag tag) {
          return tag.PluginName == upgrade.Name;
        });
        PluginTag oldPlugin = oldPlugins[0];
        // store the upgrade in the old plugin
        oldPlugin.UpgradePluginDescription = upgrade;
        UpdateTreeNodes(oldPlugins);
      });
      overridingPlugins.ForEach(delegate(PluginDescription overridingPlugin) {
        List<PluginTag> currentPlugins = allTags.FindAll(delegate(PluginTag tag) {
          return tag.PluginName == overridingPlugin.Name;
        });
        PluginTag currentPlugin = currentPlugins[0];
        // replace the plugin description of the available plugin to point to the overriding plugin
        currentPlugin.PluginDescription = overridingPlugin;
      });
      toolStripStatusLabel.Text = "Installed: " + listView.Groups[INSTALLED_PLUGINS].Items.Count +
        " Updates: " + upgrades.Count + " Available: " + listView.Groups[AVAILABLE_PLUGINS].Items.Count;
      RebuildActionHulls();

      listView.Sort();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
      AboutBox box = new AboutBox();
      box.ShowDialog();
    }

    private void publishButton_Click(object sender, EventArgs args) {
      if(listView.SelectedItems.Count == 0) return;
      List<PluginInfo> plugins = new List<PluginInfo>();
      foreach(ListViewItem item in listView.SelectedItems) {
        plugins.Add(((PluginTag)item.Tag).Plugin);
      }
      PublishPlugin(plugins);
    }

    private void PublishPlugin(List<PluginInfo> plugins) {
      StringBuilder xmlEntries = new StringBuilder();
      try {
        infoTextBox.Text = "Publishing plugin:\n";
        foreach(PluginInfo plugin in plugins) {
          string packageFileName = plugin.Name + "-" + plugin.Version + ".zip";
          ZipFile zipFile = ZipFile.Create(packageFileName);
          zipFile.NameTransform = new PluginNameTransform();
          zipFile.BeginUpdate();

          infoTextBox.Text+="Creating " + packageFileName + "...\n";
          foreach(string filename in plugin.Files) {
            infoTextBox.Text += "Adding " + filename + "\n";
            zipFile.Add(filename);
          }

          zipFile.CommitUpdate();
          zipFile.Close();
          FileInfo fileInfo = new FileInfo(packageFileName);
          infoTextBox.Text += "Created " + packageFileName + " (" + fileInfo.Length + " bytes)\n";
          xmlEntries.Append("  <Plugin Name=\"").Append(plugin.Name).Append("\" Version=\"").Append(plugin.Version)
            .Append("\" Build=\"").Append(plugin.BuildDate.ToUniversalTime().ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat))
            .Append("\">").AppendLine();
          foreach(PluginInfo dependency in plugin.Dependencies) {
            xmlEntries.Append("    <Dependency Name=\"").Append(dependency.Name).Append("\" />").AppendLine();
          }
          xmlEntries.Append("  </Plugin>").AppendLine();
        }

        infoTextBox.Text += "Upload the zip files to your plugin source and add the following to" +
" the file plugins.xml residing in the base directory of your plugin source.\n\n";
        infoTextBox.Text += xmlEntries.ToString();
      } catch(Exception exception) {
        infoTextBox.Text += "\nThere was an error!\n" + exception;
      }
    }

    private void updateButton_Click(object sender, EventArgs e) {
      // connect to all plugin sources and get a list of available plugins
      // log progress in the infoPane
      BackgroundWorker worker = new BackgroundWorker();
      worker.WorkerSupportsCancellation = true;
      worker.WorkerReportsProgress = true;

      DownloaderDialog dialog = new DownloaderDialog();
      dialog.OnCancel += delegate() {
        worker.CancelAsync();
      };

      worker.ProgressChanged += delegate(object progressChangedSender, ProgressChangedEventArgs args) {
        dialog.SetProgress(args.ProgressPercentage);
      };

      worker.DoWork += delegate(object doWorkSender, DoWorkEventArgs args) {
        allAvailablePlugins.Clear();
        dialog.SetDownloadDescription("Updating available plugins...");
        int i = 0;
        int n = HeuristicLab.PluginInfrastructure.GUI.Properties.Settings.Default.PluginSources.Count;
        foreach(string pluginSourceUrl in HeuristicLab.PluginInfrastructure.GUI.Properties.Settings.Default.PluginSources) {
          if(!worker.CancellationPending) {
            dialog.SetDownloadDescription("Connecting to " + pluginSourceUrl + "...");
            PluginSource source = PluginSource.TryCreate(pluginSourceUrl);
            if(source == null) {
              Invoke((MethodInvoker)delegate() {
                infoTextBox.Text += "Error! Couldn't access the plugin source " + pluginSourceUrl + ".\n";
              });
            } else {
              dialog.SetDownloadDescription("Getting list of available plugins from " + pluginSourceUrl + "...");
              List<PluginDescription> availablePlugins = source.AvailablePlugins();
              allAvailablePlugins.AddRange(availablePlugins);
              Invoke((MethodInvoker)delegate() {
                infoTextBox.Text += pluginSourceUrl + " " + availablePlugins.Count + " plugins available.\n";
              });

              worker.ReportProgress((int)((++i / (double)n) * 100.0));
            }
          }
        }
        if(worker.CancellationPending) {
          args.Cancel = true;
        } else {
          args.Cancel = false;
          args.Result = allAvailablePlugins;
        }
      };

      worker.RunWorkerCompleted += delegate(object runWorkerCompletedSender, RunWorkerCompletedEventArgs args) {
        if(!args.Cancelled && args.Error == null) {
          InitializePlugins();
        }
        dialog.Close();
      };

      dialog.Show();
      worker.RunWorkerAsync();
    }


    private List<PluginDescription> FilterMostRecentPluginVersions(List<PluginDescription> list) {
      List<PluginDescription> newList = new List<PluginDescription>();

      list.ForEach(delegate(PluginDescription tag) {
        // find all entries with the same plugin name
        List<PluginDescription> samePlugins = list.FindAll(delegate(PluginDescription otherTag) {
          return tag.Name == otherTag.Name;
        });

        // keep only the most recent one
        PluginDescription mostRecentVersion = samePlugins[0];
        if(samePlugins.Count > 1) {
          samePlugins.ForEach(delegate(PluginDescription tag2) {
            if(tag2.Version > mostRecentVersion.Version || 
              (tag2.Version == mostRecentVersion.Version && tag2.BuildDate>mostRecentVersion.BuildDate)) {
              mostRecentVersion = tag2;
            }
          });
        }
        if(!newList.Contains(mostRecentVersion)) {
          newList.Add(mostRecentVersion);
        }

      });

      return newList;
    }

    private List<PluginDescription> FindNewPlugins(List<PluginTag> allTags, List<PluginDescription> allAvailablePlugins) {
      List<PluginDescription> newPlugins = new List<PluginDescription>();
      // for each of the available plugins check if there is an installed plugin that has the same name
      // only if there is no installed plugin with the same name then it is a new plugin
      // NOTE: make sure to keep only the most recent entry of a plugin in the allAvailablePlugins list
      allAvailablePlugins.ForEach(delegate(PluginDescription availablePlugin) {
        if(!allTags.Exists(delegate(PluginTag tag2) {
          return availablePlugin.Name == tag2.PluginName;
        })) {
          newPlugins.Add(availablePlugin);
        }
      });

      return newPlugins;
    }

    private List<PluginDescription> FindUpgrades(List<PluginTag> allTags, List<PluginDescription> allAvailablePlugins) {
      List<PluginDescription> upgrades = new List<PluginDescription>();
      // for each of the available plugins check if there is an installed plugin that has the same name
      // only if there is an installed plugin with the same name and the available plugin has a more recent version it is an upgrade
      // NOTE: make sure to keep only the most recent entry of a plugin in the allAvailablePlugins list
      allAvailablePlugins.ForEach(delegate(PluginDescription availablePlugin) {
        List<PluginTag> oldPlugins = allTags.FindAll(delegate(PluginTag currentPlugin) {
          return currentPlugin.PluginName == availablePlugin.Name &&
            (currentPlugin.State & (PluginState.Installed | PluginState.Disabled)) != 0;
        });

        if(oldPlugins.Count == 1) {
          if(oldPlugins[0].PluginVersion < availablePlugin.Version ||
            (oldPlugins[0].PluginVersion == availablePlugin.Version && oldPlugins[0].PluginBuildDate<availablePlugin.BuildDate)) {
            upgrades.Add(availablePlugin);
          }
        }
      });

      return upgrades;
    }

    private List<PluginDescription> FindOverridingPlugins(List<PluginTag> allTags, List<PluginDescription> allAvailablePlugins) {
      List<PluginDescription> overrides = new List<PluginDescription>();
      allAvailablePlugins.ForEach(delegate(PluginDescription availablePlugin) {
        List<PluginTag> currentPlugins = allTags.FindAll(delegate(PluginTag currentPlugin) {
          return currentPlugin.PluginName == availablePlugin.Name && currentPlugin.State == PluginState.Available;
        });

        if(currentPlugins.Count == 1 && (currentPlugins[0].PluginVersion < availablePlugin.Version ||
          (currentPlugins[0].PluginVersion==availablePlugin.Version && currentPlugins[0].PluginBuildDate<availablePlugin.BuildDate))) {
          overrides.Add(availablePlugin);
        }
      });

      return overrides;
    }

    private void RebuildActionHulls() {
      Dictionary<PluginTag, PluginAction> oldActions = new Dictionary<PluginTag, PluginAction>(actions);
      actions.Clear();

      foreach(PluginAction action in oldActions.Values) {
        if(action.Action == ManagerAction.Install) {
          MarkInstall(action.Plugin);
        } else if(action.Action == ManagerAction.Remove) {
          MarkRemove(action.Plugin);
        } else
          throw new InvalidOperationException();
      }

      // update the GUI to represent new state of the selected plugin
      if(listView.SelectedItems.Count>0 && listView.SelectedItems[0].Tag is PluginTag) {
        UpdateActionButtons((PluginTag)listView.SelectedItems[0].Tag);
        DisplayPluginInfo(((PluginTag)listView.SelectedItems[0].Tag).GetPluginDetails());
      }
    }

    private void MarkInstall(PluginTag actionTag) {
      if(!CheckInstallConflicts(actionTag)) {
        CreateNewInstallAction(actionTag);
      } else {
        HandleInstallConflict(actionTag);
      }
    }

    private void UnmarkInstall(PluginTag actionTag) {
      if(!CheckNoActionConflict(actionTag)) {
        List<PluginAction> rootActions = GetActionsInvolving(actionTag);
        PluginAction rootAction = rootActions[0];
        actions.Remove(rootAction.Plugin);
        UpdateTreeNodes(rootAction.Hull);
      } else {
        HandleNoActionConflict(actionTag);
      }
    }

    private void HandleNoActionConflict(PluginTag actionTag) {
      List<PluginAction> conflictingActions = GetOverlappingActions(actionTag, ManagerAction.Any);
      PluginAction theAction = GetSmallestActionInvolving(actionTag, conflictingActions);
      conflictingActions.Remove(theAction);
      string action = theAction.GetInverseActionString();
      DialogResult userResult = ShowGenericConflictDialog(action, theAction, conflictingActions);
      if(userResult == DialogResult.OK) {
        conflictingActions.ForEach(delegate(PluginAction conflictingAction) {
          actions.Remove(conflictingAction.Plugin);
          UpdateTreeNodes(conflictingAction.Hull);
        });

        PluginAction rootAction = GetSmallestActionInvolving(actionTag, GetOverlappingActions(actionTag, ManagerAction.Any));
        actions.Remove(rootAction.Plugin);
        UpdateTreeNodes(rootAction.Hull);
      }
    }

    private bool CheckNoActionConflict(PluginTag actionTag) {
      return GetOverlappingActions(actionTag, ManagerAction.Any).Count > 1;
    }

    private void MarkRemove(PluginTag actionTag) {
      if(!CheckRemoveConflicts(actionTag)) {
        CreateNewRemoveAction(actionTag);
      } else {
        HandleRemoveConflict(actionTag);
      }
    }

    private void UnmarkRemove(PluginTag pluginTag) {
      if(!CheckNoActionConflict(pluginTag)) {
        List<PluginAction> rootActions = GetActionsInvolving(pluginTag);
        // if there is no conflict then there can only be one action connected to the pluginTag
        PluginAction rootAction = rootActions[0];
        // kill the root action
        actions.Remove(rootAction.Plugin);
        UpdateTreeNodes(rootAction.Hull);
      } else {
        HandleNoActionConflict(pluginTag);
      }
    }

    private void CreateNewRemoveAction(PluginTag actionTag) {
      CreateNewAction(actionTag, ManagerAction.Remove, actionTag.GetDependentTags());
    }

    private void CreateNewInstallAction(PluginTag actionTag) {
      CreateNewAction(actionTag, ManagerAction.Install, actionTag.GetDependencyTags());
    }

    private void CreateNewAction(PluginTag actionTag, ManagerAction action, List<PluginTag> hull) {
      PluginAction newAction = new PluginAction();
      newAction.Action = action;
      newAction.Plugin = actionTag;
      newAction.Hull = hull;
      newAction.Hull.Add(actionTag);
      actions[actionTag] = newAction;
      UpdateTreeNodes(newAction.Hull);
    }

    private bool CheckRemoveConflicts(PluginTag actionTag) {
      return GetOverlappingActions(actionTag, ManagerAction.Install).Count > 0;
    }

    private void HandleRemoveConflict(PluginTag actionTag) {
      List<PluginAction> conflictingActions = GetOverlappingActions(actionTag, ManagerAction.Install);

      // create a planned action to display in the conflict message box
      PluginAction plannedRemoveAction = new PluginAction();
      plannedRemoveAction.Action = ManagerAction.Remove;
      plannedRemoveAction.Plugin = actionTag;

      DialogResult userResult = ShowGenericConflictDialog("Remove", plannedRemoveAction, conflictingActions);
      if(userResult == DialogResult.OK) {
        // if user says ok then remove the old actions and create a new remove action
        conflictingActions.ForEach(delegate(PluginAction conflictingAction) {
          actions.Remove(conflictingAction.Plugin);
          UpdateTreeNodes(conflictingAction.Hull);
        });

        CreateNewRemoveAction(actionTag);
      }
    }

    private bool CheckInstallConflicts(PluginTag actionTag) {
      return GetOverlappingActions(actionTag, ManagerAction.Remove).Count > 0;
    }

    private void HandleInstallConflict(PluginTag actionTag) {
      List<PluginAction> conflictingActions = GetOverlappingActions(actionTag, ManagerAction.Remove);

      // create a planned action to display in the conflict message box
      PluginAction plannedInstallAction = new PluginAction();
      plannedInstallAction.Action = ManagerAction.Install;
      plannedInstallAction.Plugin = actionTag;

      DialogResult userResult = ShowGenericConflictDialog("Install", plannedInstallAction, conflictingActions);

      if(userResult == DialogResult.OK) {
        conflictingActions.ForEach(delegate(PluginAction conflictingAction) {
          actions.Remove(conflictingAction.Plugin);
          UpdateTreeNodes(conflictingAction.Hull);
        });

        CreateNewInstallAction(actionTag);
      }
    }

    private DialogResult ShowGenericConflictDialog(string action, PluginAction theAction, List<PluginAction> conflictingActions) {
      string message = "The actions:\n\n";
      conflictingActions.ForEach(delegate(PluginAction conflictingAction) {
        message += " - " + conflictingAction.Action + " " + conflictingAction.Plugin.PluginName + "\n";
      });
      message += "\nconflict with the action: " + action + " " + theAction.Plugin.PluginName + ".\n\n";
      message += "\nSolution:\n";
      conflictingActions.ForEach(delegate(PluginAction conflictingAction) {
        message += " - " + conflictingAction.GetInverseActionString() + " " + conflictingAction.Plugin.PluginName + "\n";
      });
      message += " - " + action + " " + theAction.Plugin.PluginName + "\n\n";
      message += "Do you want to continue?";

      return MessageBox.Show(message, "Resolve conflicting actions", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

    }

    private List<PluginAction> GetActionsInvolving(PluginTag actionTag) {
      List<PluginAction> filteredActions = new List<PluginAction>();
      foreach(PluginAction action in actions.Values) {
        if(action.Hull.Contains(actionTag)) {
          filteredActions.Add(action);
        }
      }
      return filteredActions;
    }

    private PluginAction GetSmallestActionInvolving(PluginTag actionTag, List<PluginAction> actions) {
      PluginAction smallestAction;
      // if there is an action defined for which actionTag is the root then us it as root tag
      // otherwise use the root of the action that contains pluginTag in its hull and has the smallest hull of all actions
      smallestAction = actions.Find(delegate(PluginAction action) {
        return action.Plugin == actionTag;
      });
      // if nothing is found then get the action with the smallest hull
      if(smallestAction == null) {
        // find the action with the smallest hull
        smallestAction = actions[0];
        actions.ForEach(delegate(PluginAction action) {
          if(action.Hull.Count < smallestAction.Hull.Count)
            smallestAction = action;
        });
      }

      return smallestAction;
    }

    /// <summary>
    /// Similar to GetRootActions but filter only specific ManagerActions 
    /// </summary>
    /// <param name="pluginTag"></param>
    /// <param name="overlapAction"></param>
    /// <returns></returns>
    private List<PluginAction> GetOverlappingActions(PluginTag actionTag, ManagerAction overlapAction) {
      List<PluginAction> overlappingActions = new List<PluginAction>();
      foreach(PluginAction action in actions.Values) {
        if(((action.Action & overlapAction) > 0) &&
          action.Hull.Contains(actionTag)) {
          overlappingActions.Add(action);
        }
      }

      return overlappingActions;
    }

    private void UpdateTreeNodes(List<PluginTag> hull) {
      hull.ForEach(delegate(PluginTag tag) {
        FindPluginItems(tag).ForEach(delegate(ListViewItem item) {
          ManagerAction action = GetAction(tag);
          if(action != ManagerAction.None) {
            item.Text = tag.PluginName + " - Action: " + action;
            if(action == ManagerAction.Remove) {
              item.ImageIndex = 3;
            } else if(action == ManagerAction.Install) {
              item.ImageIndex = 2;
            }
          } else if(tag.State == PluginState.Upgradeable) {
            item.Text = tag.PluginName + " - Upgrade: " + tag.PluginVersion + " -> " + tag.UpgradePluginDescription.Version;
            item.ImageIndex = 2;
          } else {
            item.Text = tag.PluginName;
            item.ImageIndex = 0;
          }
        });
      });
    }

    private void UpdateActionButtons(PluginTag tag) {
      publishButton.Enabled = (tag.State & PluginState.Installed) == PluginState.Installed;
      installButton.Enabled = (tag.State & PluginState.Available) == PluginState.Available;
      deleteButton.Enabled = (tag.State & (PluginState.Installed | PluginState.Upgradeable | PluginState.Disabled)) != 0;

      installButton.Checked = GetAction(tag) == ManagerAction.Install;
      deleteButton.Checked = GetAction(tag) == ManagerAction.Remove;

      publishMenuItem.Enabled = publishButton.Enabled;
      installMenuItem.Enabled = installButton.Enabled;
      deleteMenuItem.Enabled = deleteButton.Enabled;
      installMenuItem.Checked = installButton.Checked;
      deleteMenuItem.Checked = deleteButton.Checked;
    }

    private ManagerAction GetAction(PluginTag tag) {
      ManagerAction plannedAction = ManagerAction.None;
      if(actions.ContainsKey(tag)) {
        plannedAction = actions[tag].Action;
      } else {
        foreach(PluginAction action in actions.Values) {
          if(action.Hull.Contains(tag)) {
            plannedAction = action.Action;
          }
        }
      }

      if(plannedAction == ManagerAction.Install
        && (tag.State == PluginState.Installed || tag.State == PluginState.Upgradeable)) {
        return ManagerAction.None;
      } else if(plannedAction == ManagerAction.Remove
        && tag.State == PluginState.Available) {
        return ManagerAction.None;
      } else {
        return plannedAction;
      }
    }

    private List<ListViewItem> FindPluginItems(PluginTag pluginTag) {
      List<ListViewItem> items = new List<ListViewItem>();
      foreach(ListViewGroup group in listView.Groups) {
        foreach(ListViewItem item in group.Items) {
          if(pluginTag.Equals(item.Tag)) {
            items.Add(item);
          }
        }
      }
      return items;
    }
    private void DisplayPluginInfo(string pluginInformation) {
      infoTextBox.Text = pluginInformation;
    }
    private void upgradeButton_Click(object sender, EventArgs args) {
      try {
        ClearTemporaryFiles();
        if(!DownloadFiles()) {
          return;
        }
        OnDeletePlugins();
        OnPreUpgradePlugins();
        PluginManager.Manager.UnloadAllPlugins();
        BackupOldFiles();
        DeleteOldFiles();
        InstallNewFiles();
        PluginManager.Manager.LoadAllPlugins();
        InitializePlugins();
        OnPostUpgradePlugins();
        OnInstallPlugins();
        ClearTemporaryFiles();
      } catch(Exception e) {
        ShowErrorDialog(e + "");
      }
    }
    private void OnDeletePlugins() {
      allTags.ForEach(delegate(PluginTag tag) {
        if(tag.State == PluginState.Installed) {
          List<PluginAction> actions = GetActionsInvolving(tag);
          if(actions.Count > 0 && actions[0].Action == ManagerAction.Remove) {
            PluginManager.Manager.OnDelete(tag.Plugin);
          }
        }
      });
    }
    private void OnInstallPlugins() {
      allTags.ForEach(delegate(PluginTag tag) {
        if(tag.State == PluginState.Available) {
          List<PluginAction> actions = GetActionsInvolving(tag);
          if(actions.Count > 0 && actions[0].Action == ManagerAction.Install) {
            PluginManager.Manager.OnInstall(tag.Plugin);
          }
        }
      });
    }
    private List<string> upgradedPlugins = new List<string>();
    private void OnPreUpgradePlugins() {
      upgradedPlugins.Clear();
      allTags.ForEach(delegate(PluginTag tag) {
        if(tag.State == PluginState.Upgradeable) {
          PluginManager.Manager.OnPreUpdate(tag.Plugin);

          // save the name of the plugin in  a list that is used later to call OnPostUpdate for all plugins
          upgradedPlugins.Add(tag.PluginName);
        }
      });
    }
    private void OnPostUpgradePlugins() {
      allTags.ForEach(delegate(PluginTag tag) {
        if(upgradedPlugins.Contains(tag.PluginName)) {
          PluginManager.Manager.OnPostUpdate(tag.Plugin);
        }
      });
      upgradedPlugins.Clear();
    }
    /// <summary>
    /// Deletes all files in the directories cacheDir, backupDir, tempDir
    /// </summary>
    private void ClearTemporaryFiles() {
      // can't really handle exceptions here -> let higher layer handle them
      string[] filenames = Directory.GetFiles(cacheDir, "*", SearchOption.AllDirectories);
      foreach(string filename in filenames) {
        File.Delete(filename);
      }
      filenames = Directory.GetFiles(backupDir, "*", SearchOption.AllDirectories);
      foreach(string filename in filenames) {
        File.Delete(filename);
      }
      filenames = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);
      foreach(string filename in filenames) {
        File.Delete(filename);
      }
    }

    /// <summary>
    /// Extracts zip packages in cacheDir to tempDir and then copies the files from tempDir to pluginDir
    /// When there is a problem on extraction or later when copying the files to the pluginDir the method 
    /// delets all files that have already been copied from tempDir to pluginDir (the filename exists in both
    /// locations) and then copies all files in the backup directory back to the plugin directory.
    /// </summary>
    /// <returns></returns>
    private void InstallNewFiles() {
      try {
        // extract all packages
        string[] packages = Directory.GetFiles(cacheDir, "*", SearchOption.AllDirectories);
        FastZip fastZip = new FastZip();
        foreach(string package in packages) {
          fastZip.ExtractZip(package, tempDir, String.Empty);
        }

        // copy extracted files to plugin dir
        string[] extractedFiles = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);
        foreach(string extractedFile in extractedFiles) {
          File.Copy(extractedFile, pluginDir + extractedFile.Remove(0, tempDir.Length));
        }
      } catch(Exception e) {
        infoTextBox.Text = e + "";
        // remove already copied files
        string[] extractedFiles = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);
        foreach(string extractedFile in extractedFiles) {
          string filename = pluginDir + extractedFile.Remove(0, tempDir.Length);
          if(File.Exists(filename)) {
            File.Delete(filename);
          }
        }

        // restore files from backup
        string[] backupFiles = Directory.GetFiles(backupDir, "*", SearchOption.AllDirectories);
        foreach(string backupFile in backupFiles) {
          File.Copy(backupFile, pluginDir + backupFile.Remove(0, backupDir.Length));
        }

        throw e;
      }
    }

    /// <summary>
    /// Deletes all files of plugins that have been marked as 'Remove' or 'Upgrade'.
    /// If there is a problem when removing the files then all files that have been removed earlier
    /// (the filename existis in backupDir but not in pluginDir) are copied from the backupDir to the pluginDir
    /// </summary>
    /// <returns></returns>
    private void DeleteOldFiles() {
      try {
        allTags.ForEach(delegate(PluginTag tag) {
          List<PluginAction> involvingActions = GetActionsInvolving(tag);

          if(tag.State == PluginState.Upgradeable || (involvingActions.Count > 0 && involvingActions[0].Action == ManagerAction.Remove)) {
            tag.Plugin.Files.ForEach(delegate(string filename) {
              File.Delete(filename);
            });
          }
        });
      } catch(Exception e) {
        infoTextBox.Text = e + "";
        // restore deleted files from backup
        string[] backupFiles = Directory.GetFiles(backupDir, "*", SearchOption.AllDirectories);
        foreach(string backupFile in backupFiles) {
          string oldFileName = pluginDir + backupFile.Remove(0, backupDir.Length);
          if(!File.Exists(oldFileName)) {
            File.Move(backupFile, oldFileName);
          }
        }
        throw e;
      }
    }

    /// <summary>
    /// Copies all files of plugins that are marked with 'Remove' or 'Upgrade' to the backup directory.
    /// When there is a problem all files in the backup directory are deleted.
    /// </summary>
    /// <returns></returns>
    private void BackupOldFiles() {
      try {
        allTags.ForEach(delegate(PluginTag tag) {
          List<PluginAction> actionsInvolving = GetActionsInvolving(tag);

          if(tag.State == PluginState.Upgradeable || (actionsInvolving.Count > 0 && actionsInvolving[0].Action == ManagerAction.Remove)) {
            tag.Plugin.Files.ForEach(delegate(string filename) {
              File.Copy(filename, backupDir + filename.Remove(0, pluginDir.Length));
            });
          }
        });
      } catch(Exception e) {
        infoTextBox.Text = e + "";
        // delete all files in the backup directory
        string[] copiedFiles = Directory.GetFiles(backupDir, "*", SearchOption.AllDirectories);
        string filesString = "";
        foreach(string fs in copiedFiles) {
          filesString += fs + "\n";
        }
        foreach(string copiedFile in copiedFiles) {
          File.Delete(copiedFile);
        }
        throw e;
      }
    }

    private bool DownloadFiles() {
      DownloaderDialog dialog = new DownloaderDialog();
      IEnumerator<PluginTag> pluginEnumerator = allTags.GetEnumerator();
      BackgroundWorker worker = new BackgroundWorker();
      worker.WorkerReportsProgress = true;
      worker.WorkerSupportsCancellation = true;

      worker.DoWork += delegate(object sender, DoWorkEventArgs args) {
        // count number of plugins to download
        int numberOfPlugins = 0;
        allTags.ForEach(delegate(PluginTag current) {
          if(current.UpgradeAvailable()) {
            numberOfPlugins++;
          } else {
            List<PluginAction> actionsInvolving = GetActionsInvolving(current);

            if(actionsInvolving.Count > 0 && actionsInvolving[0].Action == ManagerAction.Install && current.State == PluginState.Available) {
              numberOfPlugins++;
            }
          }
        });

        // download
        int downloaded = 0;
        Invoke((MethodInvoker)delegate() {
          infoTextBox.Text = "Downloading " + numberOfPlugins + " plugins.\n";
        });

        allTags.ForEach(delegate(PluginTag current) {
          if(worker.CancellationPending) {
            args.Cancel = true;
            return;
          }
          List<PluginAction> actionsInvolving = GetActionsInvolving(current);
          if(current.UpgradeAvailable() ||
            (actionsInvolving.Count > 0 && actionsInvolving[0].Action == ManagerAction.Install && current.State == PluginState.Available)) {
            dialog.SetDownloadDescription("Downloading " + current.PluginName + " ...");
            Invoke((MethodInvoker)delegate() { infoTextBox.Text += "Downloading " + current.PluginName + " ..."; });

            long nBytes = 0;
            if(current.UpgradeAvailable()) {
              nBytes = current.UpgradePluginDescription.Source.DownloadPlugin(current.UpgradePluginDescription);
            } else {
              nBytes = current.PluginDescription.Source.DownloadPlugin(current.PluginDescription);
            }

            worker.ReportProgress(downloaded / numberOfPlugins);
            Invoke((MethodInvoker)delegate() { infoTextBox.Text += " " + nBytes + " bytes.\n"; });
          }
        });
      };

      worker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs args) {
        dialog.SetProgress(args.ProgressPercentage);
      };

      worker.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs args) {
        if(args.Cancelled) {
          infoTextBox.Text += "Cancelled downloading plugins";
          dialog.DialogResult = DialogResult.Cancel;
        } else if(args.Error != null) {
          infoTextBox.Text += "Error while downloading plugins:\n" + args.Error;
          dialog.DialogResult = DialogResult.Cancel;
        }
        dialog.DialogResult = DialogResult.OK;
        dialog.Close();
      };


      dialog.Shown += delegate(object sender, EventArgs args) {
        worker.RunWorkerAsync();
      };

      dialog.OnCancel += delegate() {
        worker.CancelAsync();
        dialog.SetDownloadDescription("Cancelling download...");
        infoTextBox.Text += "Cancelled!\n";
      };

      if(dialog.ShowDialog() == DialogResult.Cancel) {
        return false;
      } else {
        return true;
      }
    }

    private void removeButton_Clicked(object sender, EventArgs e) {
      // get the tag of the selected listViewItem
      PluginTag actionTag = (PluginTag)listView.SelectedItems[0].Tag;
      List<PluginAction> rootActions = GetActionsInvolving(actionTag);
      if(rootActions.Count > 0) {
        UnmarkRemove(actionTag);
      } else {
        MarkRemove(actionTag);
      }

      // reflect the change of plugin actions in the install/remove buttons
      UpdateActionButtons(actionTag);
      // update the plugin detail information of the selected plugin
      DisplayPluginInfo(actionTag.GetPluginDetails());
    }

    private void installButton_Clicked(object sender, EventArgs e) {
      // get the tag of the selected listViewItem
      PluginTag actionTag = (PluginTag)listView.SelectedItems[0].Tag;
      List<PluginAction> rootActions = GetActionsInvolving(actionTag);

      if(rootActions.Count > 0) {
        UnmarkInstall(actionTag);
      } else {
        MarkInstall(actionTag);
      }

      // reflect the change of plugin actions in the install/remove buttons
      UpdateActionButtons(actionTag);
      // update the plugin detail information of the selected plugin
      DisplayPluginInfo(actionTag.GetPluginDetails());
    }

    private void ShowErrorDialog(string message) {
      ErrorDialog dialog = new ErrorDialog(message, "Exception");
      dialog.ShowDialog();
    }

    private void managePluginSourcesToolStripMenuItem_Click(object sender, EventArgs e) {
      PluginSourceEditor sourceEditor = new PluginSourceEditor();
      sourceEditor.ShowDialog();
    }

    private void refreshPluginListToolStripMenuItem_Click(object sender, EventArgs e) {
      PluginManager.Manager.UnloadAllPlugins();
      PluginManager.Manager.LoadAllPlugins();

      InitializePlugins();
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
      Close();
    }

    private void installPluginFromFileToolStripMenuItem_Click(object sender, EventArgs args) {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Multiselect = false;
      DialogResult result = dialog.ShowDialog();
      if(result == DialogResult.OK) {
        string packageName = dialog.FileName;
        try {
          ClearTemporaryFiles();

          FastZip fastZip = new FastZip();
          fastZip.ExtractZip(packageName, cacheDir, String.Empty);

          // check if none of the files exist
          foreach(string filename in Directory.GetFiles(cacheDir)) {
            if(File.Exists(pluginDir + filename.Remove(0, cacheDir.Length))) {
              ShowErrorDialog("Sorry can't install the plugin " + packageName + "\nThe file: " + filename.Remove(0, cacheDir.Length) + " already exist in " + pluginDir + "\nIt seems the plugin is already installed.");
              ClearTemporaryFiles();
              return;
            }
          }

          PluginManager.Manager.UnloadAllPlugins();
          // move the files
          foreach(string filename in Directory.GetFiles(cacheDir)) {
            File.Move(filename, pluginDir + filename.Remove(0, cacheDir.Length));
          }
          PluginManager.Manager.LoadAllPlugins();
          InitializePlugins();

        } catch(Exception e) {
          ShowErrorDialog(e + "");
        } finally {
          ClearTemporaryFiles();
        }
      }
    }

    private void pluginTreeView_KeyDown(object sender, KeyEventArgs e) {
      if(e.KeyData == Keys.I && installButton.Enabled) {
        e.Handled = true;
        e.SuppressKeyPress = true;
        installButton_Clicked(sender, e);
      } else if((e.KeyData == Keys.D || e.KeyData == Keys.Delete) && deleteButton.Enabled) {
        e.Handled = true;
        e.SuppressKeyPress = true;
        removeButton_Clicked(sender, e);
      }
    }

    private void listView_MouseDown(object sender, MouseEventArgs e) {
      // dumb solution to automatically select the node on right clicks which opens the context menu because 
      // per default the treeview doesn't select nodes on right click
      if(e.Button == MouseButtons.Right) {
        listView.GetItemAt(e.X, e.Y).Selected = true;
      }
    }

    private void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
      if(e.Item.Tag != null && e.IsSelected) {
        UpdateActionButtons((PluginTag)e.Item.Tag);
        // display the plugin details in the lower pane
        DisplayPluginInfo(((PluginTag)e.Item.Tag).GetPluginDetails());
      } else {
        // when an item was 'unselected' or was selected but doesn't represent a plugin then install and remove are not possible
        publishButton.Enabled = false;
        installButton.Enabled = false;
        deleteButton.Enabled = false;
      }
    }
  }
}
