#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Persistence.Auxiliary;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.GUI {

  public partial class PersistenceConfigurationForm : Form {

    private readonly Dictionary<string, IPrimitiveSerializer> primitiveSerializersTable;
    private readonly Dictionary<string, bool> simplePrimitiveSerializersTable;
    private readonly Dictionary<IPrimitiveSerializer, string> reversePrimitiveSerializersTable;
    private readonly Dictionary<string, Type> typeNameTable;
    private readonly Dictionary<Type, string> reverseTypeNameTable;
    private bool underConstruction;

    public PersistenceConfigurationForm() {
      InitializeComponent();
      primitiveSerializersTable = new Dictionary<string, IPrimitiveSerializer>();
      simplePrimitiveSerializersTable = new Dictionary<string, bool>();
      reversePrimitiveSerializersTable = new Dictionary<IPrimitiveSerializer, string>();
      typeNameTable = new Dictionary<string, Type>();
      reverseTypeNameTable = new Dictionary<Type, string>();
      underConstruction = true;
      InitializeTooltips();
      InitializeNameTables();
      initializeConfigPages();
      try {
        ConfigurationService.Instance.LoadSettings(true);
        UpdateFromConfigurationService();
      }
      catch (PersistenceException) {
        MessageBox.Show(
          "Persistence settings could not be loaded.\r\n" +
          "Default configurations will be used instead.",
          "Loading Settings Failed",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
      }
      underConstruction = false;
      UpdatePreview();
    }

    private void InitializeTooltips() {
      ToolTip tooltip = new ToolTip() {
        AutoPopDelay = 5000,
        InitialDelay = 1000,
        ReshowDelay = 500,
        ShowAlways = true
      };
      tooltip.SetToolTip(resetButton,
        "Clear all custom configurations from memory.\r\n" +
        "The saved configuration will still be used next\r\n" +
        "time if you don't save (define) this change.");
      tooltip.SetToolTip(updateButton,
        "Define configuration for currently active format\r\n" +
        "and save to disk.");
    }

    private void UpdatePrimitiveSerializersGrid(DataGridView primitiveSerializersGrid, Configuration config) {
      foreach (DataGridViewRow row in primitiveSerializersGrid.Rows) {
        if (row.Cells["Type"] != null) {
          IPrimitiveSerializer primitiveSerializer = config.GetPrimitiveSerializer(typeNameTable[(string)row.Cells["Type"].Value]);
          if (primitiveSerializer == null) {
            row.Cells["Active"].Value = false;
          } else {
            foreach (var pair in primitiveSerializersTable) {
              if (pair.Value.GetType().VersionInvariantName() == primitiveSerializer.GetType().VersionInvariantName()) {
                row.Cells["Primitive Serializer"].Value = pair.Key;
                row.Cells["Active"].Value = true;
                break;
              }
            }
          }
        }
      }
    }

    private void UpdateCompositeSerializersList(ListView compositeSerializersList, Configuration config) {
      compositeSerializersList.SuspendLayout();
      compositeSerializersList.Items.Clear();
      var availableCompositeSerializers = new Dictionary<string, ICompositeSerializer>();
      foreach (ICompositeSerializer d in ConfigurationService.Instance.CompositeSerializers) {
        availableCompositeSerializers.Add(d.GetType().VersionInvariantName(), d);
      }
      foreach (ICompositeSerializer compositeSerializer in config.CompositeSerializers) {
        var item = compositeSerializersList.Items.Add(compositeSerializer.GetType().Name);
        item.Checked = true;
        item.Tag = compositeSerializer;
        availableCompositeSerializers.Remove(compositeSerializer.GetType().VersionInvariantName());
      }
      foreach (KeyValuePair<string, ICompositeSerializer> pair in availableCompositeSerializers) {
        var item = compositeSerializersList.Items.Add(pair.Value.GetType().Name);
        item.Checked = false;
        item.Tag = pair.Value;
      }
      compositeSerializersList.ResumeLayout();
    }

    private void UpdateFromConfigurationService() {
      configurationTabs.SuspendLayout();
      foreach (IFormat format in ConfigurationService.Instance.Formats) {
        Configuration config = ConfigurationService.Instance.GetConfiguration(format);
        UpdatePrimitiveSerializersGrid(
          (DataGridView)GetControlsOnPage(format.Name, "GridView"),
          config);
        UpdateCompositeSerializersList(
          (ListView)GetControlsOnPage(format.Name, "CompositeSerializersList"),
          config);
      }
      configurationTabs.ResumeLayout();
    }

    private void initializeConfigPages() {
      configurationTabs.SuspendLayout();
      configurationTabs.TabPages.Clear();
      foreach (IFormat format in ConfigurationService.Instance.Formats) {
        List<IPrimitiveSerializer> primitiveSerializers = ConfigurationService.Instance.PrimitiveSerializers[format.SerialDataType];
        TabPage page = new TabPage(format.Name) {
          Name = format.Name,
          Tag = format,
        };
        page.SuspendLayout();
        configurationTabs.TabPages.Add(page);
        SplitContainer verticalSplit = new SplitContainer {
          Dock = DockStyle.Fill,
          Orientation = Orientation.Vertical,
          BorderStyle = BorderStyle.Fixed3D,
        };
        verticalSplit.SuspendLayout();
        page.Controls.Add(verticalSplit);
        SplitContainer horizontalSplit = new SplitContainer {
          Dock = DockStyle.Fill,
          Orientation = Orientation.Horizontal,
          BorderStyle = BorderStyle.Fixed3D,
        };
        horizontalSplit.SuspendLayout();
        verticalSplit.Panel1.Controls.Add(horizontalSplit);
        ListView compositeSerializersList = createCompsiteSerializersList();
        horizontalSplit.Panel1.Controls.Add(compositeSerializersList);
        DataGridView gridView = createGridView();
        verticalSplit.Panel2.Controls.Add(gridView);
        fillDataGrid(gridView, primitiveSerializers);
        ListBox checkBox = new ListBox {
          Name = "CheckBox",
          Dock = DockStyle.Fill,
        };
        horizontalSplit.Panel2.Controls.Add(checkBox);
        horizontalSplit.ResumeLayout();
        verticalSplit.ResumeLayout();
        page.ResumeLayout();
      }
      configurationTabs.ResumeLayout();
    }

    private DataGridView createGridView() {
      DataGridView gridView = new DataGridView {
        Name = "GridView",
        Dock = DockStyle.Fill,
        RowHeadersVisible = false,
        MultiSelect = false,
        EditMode = DataGridViewEditMode.EditOnEnter,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        AllowUserToOrderColumns = true,
      };
      gridView.SuspendLayout();
      gridView.CellValueChanged += gridView_CellValueChanged;
      gridView.Columns.Add(new DataGridViewTextBoxColumn {
        Name = "Type", ReadOnly = true,
        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
      });
      gridView.Columns.Add(new DataGridViewCheckBoxColumn {
        Name = "Active",
        AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
      });
      gridView.Columns.Add(new DataGridViewComboBoxColumn {
        Name = "Primitive Serializer",
        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
      });
      gridView.ResumeLayout();
      return gridView;
    }

    private ListView createCompsiteSerializersList() {
      ListView compositeSerializersList = new ListView {
        Activation = ItemActivation.OneClick,
        AllowDrop = true,
        CheckBoxes = true,
        Dock = DockStyle.Fill,
        FullRowSelect = true,
        GridLines = true,
        HeaderStyle = ColumnHeaderStyle.Nonclickable,
        Name = "CompositeSerializersList",
        ShowGroups = false,
        View = View.Details
      };
      compositeSerializersList.SuspendLayout();
      compositeSerializersList.Resize += compositeSerializersList_Resize;
      compositeSerializersList.ItemChecked += compositeSerializersList_ItemChecked;
      compositeSerializersList.DragDrop += compositeSerializersList_DragDrop;
      compositeSerializersList.DragEnter += compositeSerializersList_DragEnter;
      compositeSerializersList.ItemDrag += compositeSerializersList_ItemDrag;
      compositeSerializersList.Columns.Add(
        new ColumnHeader {
          Name = "CompositeSerializersColumn", Text = "Composite Serializer",
        });
      foreach (ICompositeSerializer compositeSerializer in ConfigurationService.Instance.CompositeSerializers) {
        var item = compositeSerializersList.Items.Add(compositeSerializer.GetType().Name);
        item.Checked = true;
        item.Tag = compositeSerializer;
      }
      compositeSerializersList.ResumeLayout();
      return compositeSerializersList;
    }

    private void fillDataGrid(DataGridView gridView, IEnumerable<IPrimitiveSerializer> primitiveSerializers) {
      gridView.SuspendLayout();
      Dictionary<string, List<string>> primitiveSerializersMap = createPrimitiveSerializersMap(primitiveSerializers);
      foreach (var primitiveSerializersMapping in primitiveSerializersMap) {
        var row = gridView.Rows[gridView.Rows.Add()];
        row.Cells["Type"].Value = primitiveSerializersMapping.Key;
        row.Cells["Type"].ToolTipText = primitiveSerializersMapping.Key;
        row.Cells["Active"].Value = true;
        var comboBoxCell = (DataGridViewComboBoxCell)row.Cells["Primitive Serializer"];
        foreach (var primitiveSerializer in primitiveSerializersMapping.Value) {
          comboBoxCell.Items.Add(primitiveSerializer);
        }
        comboBoxCell.Value = comboBoxCell.Items[0];
        comboBoxCell.ToolTipText = comboBoxCell.Items[0].ToString();
        if (comboBoxCell.Items.Count == 1) {
          comboBoxCell.ReadOnly = true;
          comboBoxCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        }
      }
      gridView.ResumeLayout();
    }

    private Dictionary<string, List<string>> createPrimitiveSerializersMap(IEnumerable<IPrimitiveSerializer> primitiveSerializers) {
      var primitiveSerializersMap = new Dictionary<string, List<string>>();
      foreach (var primitiveSerializer in primitiveSerializers) {
        string primitiveSerializerName = reversePrimitiveSerializersTable[primitiveSerializer];
        string typeName = reverseTypeNameTable[primitiveSerializer.SourceType];
        if (!primitiveSerializersMap.ContainsKey(typeName))
          primitiveSerializersMap.Add(typeName, new List<string>());
        primitiveSerializersMap[typeName].Add(primitiveSerializerName);
      }
      return primitiveSerializersMap;
    }

    private void InitializeNameTables() {
      foreach (var serialDataType in ConfigurationService.Instance.PrimitiveSerializers.Keys) {
        foreach (var primtiveSerializer in ConfigurationService.Instance.PrimitiveSerializers[serialDataType]) {
          string primitiveSerializerName = primtiveSerializer.GetType().Name;
          if (simplePrimitiveSerializersTable.ContainsKey(primitiveSerializerName)) {
            IPrimitiveSerializer otherPrimitiveSerializer = primitiveSerializersTable[primitiveSerializerName];
            primitiveSerializersTable.Remove(primitiveSerializerName);
            reversePrimitiveSerializersTable.Remove(otherPrimitiveSerializer);
            primitiveSerializersTable.Add(otherPrimitiveSerializer.GetType().VersionInvariantName(), otherPrimitiveSerializer);
            reversePrimitiveSerializersTable.Add(otherPrimitiveSerializer, otherPrimitiveSerializer.GetType().VersionInvariantName());
            primitiveSerializerName = primtiveSerializer.GetType().VersionInvariantName();
          }
          simplePrimitiveSerializersTable[primtiveSerializer.GetType().Name] = true;
          primitiveSerializersTable.Add(primitiveSerializerName, primtiveSerializer);
          reversePrimitiveSerializersTable.Add(primtiveSerializer, primitiveSerializerName);

          string typeName = primtiveSerializer.SourceType.IsGenericType ?
            primtiveSerializer.SourceType.SimpleFullName() :
            primtiveSerializer.SourceType.Name;
          if (typeNameTable.ContainsKey(typeName)) {
            Type otherType = typeNameTable[typeName];
            if (otherType != primtiveSerializer.SourceType) {
              typeNameTable.Remove(typeName);
              reverseTypeNameTable.Remove(otherType);
              typeNameTable.Add(otherType.VersionInvariantName(), otherType);
              reverseTypeNameTable.Add(otherType, otherType.VersionInvariantName());
              typeName = primtiveSerializer.SourceType.VersionInvariantName();
              typeNameTable.Add(typeName, primtiveSerializer.SourceType);
              reverseTypeNameTable.Add(primtiveSerializer.SourceType, typeName);
            }
          } else {
            typeNameTable.Add(typeName, primtiveSerializer.SourceType);
            reverseTypeNameTable.Add(primtiveSerializer.SourceType, typeName);
          }
        }
      }
    }

    private void UpdatePreview() {
      if (underConstruction)
        return;
      ListBox checkBox = (ListBox)GetActiveControl("CheckBox");
      checkBox.SuspendLayout();
      IFormat activeFormat = (IFormat)configurationTabs.SelectedTab.Tag;
      if (activeFormat != null && checkBox != null) {
        checkBox.Items.Clear();
        Configuration activeConfig = GetActiveConfiguration();
        foreach (var primitveSerializer in activeConfig.PrimitiveSerializers) {
          checkBox.Items.Add(primitveSerializer.GetType().Name + " (F)");
        }
        foreach (var compositeSerializer in activeConfig.CompositeSerializers)
          checkBox.Items.Add(compositeSerializer.GetType().Name + " (D)");
      }
      checkBox.ResumeLayout();
    }


    void gridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
      UpdatePreview();
    }

    private void compositeSerializersList_ItemDrag(object sender, ItemDragEventArgs e) {
      ListView compositeSerializersList = (ListView)sender;
      compositeSerializersList.DoDragDrop(compositeSerializersList.SelectedItems, DragDropEffects.Move);
    }

    private void compositeSerializersList_DragEnter(object sender, DragEventArgs e) {
      if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection).FullName)) {
        e.Effect = DragDropEffects.Move;
      }
    }

    private void compositeSerializersList_DragDrop(object sender, DragEventArgs e) {
      ListView compositeSerializersList = (ListView)sender;
      if (compositeSerializersList.SelectedItems.Count == 0) {
        return;
      }
      Point cp = compositeSerializersList.PointToClient(new Point(e.X, e.Y));
      ListViewItem targetItem = compositeSerializersList.GetItemAt(cp.X, cp.Y);
      if (targetItem == null)
        return;
      int targetIndex = targetItem.Index;
      var selectedItems = new List<ListViewItem>(compositeSerializersList.SelectedItems.Cast<ListViewItem>());
      int i = 0;
      foreach (ListViewItem dragItem in selectedItems) {
        if (targetIndex == dragItem.Index)
          return;
        if (dragItem.Index < targetIndex) {
          compositeSerializersList.Items.Insert(targetIndex + 1, (ListViewItem)dragItem.Clone());
        } else {
          compositeSerializersList.Items.Insert(targetIndex + i, (ListViewItem)dragItem.Clone());
        }
        compositeSerializersList.Items.Remove(dragItem);
        i++;
      }
      UpdatePreview();
    }

    private void compositeSerializersList_Resize(object sender, EventArgs e) {
      ListView compositeSerializersList = (ListView)sender;
      compositeSerializersList.Columns["CompositeSerializersColumn"].Width = compositeSerializersList.Width - 4;
    }


    private void compositeSerializersList_ItemChecked(object sender, ItemCheckedEventArgs e) {
      UpdatePreview();
    }

    private Control GetActiveControl(string name) {
      Control[] controls = configurationTabs.SelectedTab.Controls.Find(name, true);
      if (controls.Length == 1) {
        return controls[0];
      } else {
        return null;
      }
    }

    private Control GetControlsOnPage(string pageName, string name) {
      Control[] controls = configurationTabs.TabPages[pageName].Controls.Find(name, true);
      if (controls.Length == 1) {
        return controls[0];
      } else {
        return null;
      }
    }

    private Configuration GenerateConfiguration(IFormat format, DataGridView primitiveSerializersGrid, ListView compositeSerializersList) {
      if (primitiveSerializersGrid == null || compositeSerializersList == null)
        return null;
      var primitiveSerializers = new List<IPrimitiveSerializer>();
      foreach (DataGridViewRow row in primitiveSerializersGrid.Rows) {
        if (row.Cells["Type"].Value != null &&
             row.Cells["Active"].Value != null &&
             row.Cells["Primitive Serializer"].Value != null &&
             (bool)row.Cells["Active"].Value == true) {
          primitiveSerializers.Add(primitiveSerializersTable[(string)row.Cells["Primitive Serializer"].Value]);
        }
      }
      var compositeSerializers = new List<ICompositeSerializer>();
      foreach (ListViewItem item in compositeSerializersList.Items) {
        if (item != null && item.Checked)
          compositeSerializers.Add((ICompositeSerializer)item.Tag);
      }
      return new Configuration(format, primitiveSerializers, compositeSerializers);
    }

    private Configuration GetActiveConfiguration() {
      IFormat format = (IFormat)configurationTabs.SelectedTab.Tag;
      return GenerateConfiguration(format,
        (DataGridView)GetActiveControl("GridView"),
        (ListView)GetActiveControl("CompositeSerializersList"));
    }

    private Configuration GetConfiguration(IFormat format) {
      return GenerateConfiguration(format,
       (DataGridView)GetControlsOnPage(format.Name, "GridView"),
       (ListView)GetControlsOnPage(format.Name, "CompositeSerializersList"));
    }

    private void updateButton_Click(object sender, EventArgs e) {
      IFormat format = (IFormat)configurationTabs.SelectedTab.Tag;
      if (format != null)
        ConfigurationService.Instance.DefineConfiguration(
          GetActiveConfiguration());
    }

    private void resetButton_Click(object sender, EventArgs e) {
      ConfigurationService.Instance.Reset();
      underConstruction = true;
      UpdateFromConfigurationService();
      underConstruction = false;
      UpdatePreview();
    }

  }



}
