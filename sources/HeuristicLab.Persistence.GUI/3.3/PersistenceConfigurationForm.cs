using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Interfaces;
using System.Text;
using HeuristicLab.Persistence.Default.Decomposers;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.GUI {

  public partial class PersistenceConfigurationForm : Form {

    private readonly Dictionary<string, IFormatter> formatterTable;
    private readonly Dictionary<string, bool> simpleFormatterTable;
    private readonly Dictionary<IFormatter, string> reverseFormatterTable;
    private readonly Dictionary<string, Type> typeNameTable;
    private readonly Dictionary<Type, string> reverseTypeNameTable;
    private bool underConstruction;

    public PersistenceConfigurationForm() {
      InitializeComponent();
      formatterTable = new Dictionary<string, IFormatter>();
      simpleFormatterTable = new Dictionary<string, bool>();
      reverseFormatterTable = new Dictionary<IFormatter, string>();
      typeNameTable = new Dictionary<string, Type>();
      reverseTypeNameTable = new Dictionary<Type, string>();
      underConstruction = true;
      InitializeTooltips();
      InitializeNameTables();
      initializeConfigPages();
      UpdateFromConfigurationService();
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

    private void UpdateFormatterGrid(DataGridView formatterGrid, Configuration config) {
      foreach (DataGridViewRow row in formatterGrid.Rows) {
        if (row.Cells["Type"] != null) {
          IFormatter formatter = config.GetFormatter(typeNameTable[(string)row.Cells["Type"].Value]);
          if (formatter == null) {
            row.Cells["Active"].Value = false;
          } else {
            foreach (var pair in formatterTable) {
              if (pair.Value.GetType().VersionInvariantName() == formatter.GetType().VersionInvariantName()) {
                row.Cells["Formatter"].Value = pair.Key;
                row.Cells["Active"].Value = true;
                break;
              }
            }
          }
        }
      }
    }

    private void UpdateDecomposerList(ListView decomposerList, Configuration config) {
      decomposerList.SuspendLayout();
      decomposerList.Items.Clear();
      var availableDecomposers = new Dictionary<string, IDecomposer>();
      foreach (IDecomposer d in ConfigurationService.Instance.Decomposers) {
        availableDecomposers.Add(d.GetType().VersionInvariantName(), d);
      }
      foreach (IDecomposer decomposer in config.Decomposers) {
        var item = decomposerList.Items.Add(decomposer.GetType().Name);
        item.Checked = true;
        item.Tag = decomposer;
        availableDecomposers.Remove(decomposer.GetType().VersionInvariantName());
      }
      foreach (KeyValuePair<string, IDecomposer> pair in availableDecomposers) {
        var item = decomposerList.Items.Add(pair.Value.GetType().Name);
        item.Checked = false;
        item.Tag = pair.Value;
      }
      decomposerList.ResumeLayout();
    }

    private void UpdateFromConfigurationService() {
      configurationTabs.SuspendLayout();
      foreach (IFormat format in ConfigurationService.Instance.Formats) {
        Configuration config = ConfigurationService.Instance.GetConfiguration(format);
        UpdateFormatterGrid(
          (DataGridView)GetControlsOnPage(format.Name, "GridView"),
          config);
        UpdateDecomposerList(
          (ListView)GetControlsOnPage(format.Name, "DecomposerList"),
          config);
      }
      configurationTabs.ResumeLayout();
    }

    private void initializeConfigPages() {
      configurationTabs.SuspendLayout();
      configurationTabs.TabPages.Clear();
      foreach (IFormat format in ConfigurationService.Instance.Formats) {
        List<IFormatter> formatters = ConfigurationService.Instance.Formatters[format.SerialDataType];
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
        ListView decomposerList = createDecomposerList();
        horizontalSplit.Panel1.Controls.Add(decomposerList);
        DataGridView gridView = createGridView();
        verticalSplit.Panel2.Controls.Add(gridView);
        fillDataGrid(gridView, formatters);
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
        Name = "Formatter",
        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
      });
      gridView.ResumeLayout();
      return gridView;
    }

    private ListView createDecomposerList() {
      ListView decomposerList = new ListView {
        Activation = ItemActivation.OneClick,
        AllowDrop = true,
        CheckBoxes = true,
        Dock = DockStyle.Fill,
        FullRowSelect = true,
        GridLines = true,
        HeaderStyle = ColumnHeaderStyle.Nonclickable,
        Name = "DecomposerList",
        ShowGroups = false,
        View = View.Details
      };
      decomposerList.SuspendLayout();
      decomposerList.Resize += decomposerList_Resize;
      decomposerList.ItemChecked += decomposerList_ItemChecked;
      decomposerList.DragDrop += decomposerList_DragDrop;
      decomposerList.DragEnter += decomposerList_DragEnter;
      decomposerList.ItemDrag += decomposerList_ItemDrag;
      decomposerList.Columns.Add(
        new ColumnHeader {
          Name = "DecomposerColumn", Text = "Decomposers",
        });
      foreach (IDecomposer decomposer in ConfigurationService.Instance.Decomposers) {
        var item = decomposerList.Items.Add(decomposer.GetType().Name);
        item.Checked = true;
        item.Tag = decomposer;
      }
      decomposerList.ResumeLayout();
      return decomposerList;
    }

    private void fillDataGrid(DataGridView gridView, IEnumerable<IFormatter> formatters) {
      gridView.SuspendLayout();
      Dictionary<string, List<string>> formatterMap = createFormatterMap(formatters);
      foreach (var formatterMapping in formatterMap) {
        var row = gridView.Rows[gridView.Rows.Add()];
        row.Cells["Type"].Value = formatterMapping.Key;
        row.Cells["Type"].ToolTipText = formatterMapping.Key;
        row.Cells["Active"].Value = true;
        var comboBoxCell = (DataGridViewComboBoxCell)row.Cells["Formatter"];
        foreach (var formatter in formatterMapping.Value) {
          comboBoxCell.Items.Add(formatter);
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

    private Dictionary<string, List<string>> createFormatterMap(IEnumerable<IFormatter> formatters) {
      var formatterMap = new Dictionary<string, List<string>>();
      foreach (var formatter in formatters) {
        string formatterName = reverseFormatterTable[formatter];
        string typeName = reverseTypeNameTable[formatter.SourceType];
        if (!formatterMap.ContainsKey(typeName))
          formatterMap.Add(typeName, new List<string>());
        formatterMap[typeName].Add(formatterName);
      }
      return formatterMap;
    }

    private void InitializeNameTables() {
      foreach (var serialDataType in ConfigurationService.Instance.Formatters.Keys) {
        foreach (var formatter in ConfigurationService.Instance.Formatters[serialDataType]) {
          string formatterName = formatter.GetType().Name;
          if (simpleFormatterTable.ContainsKey(formatterName)) {
            IFormatter otherFormatter = formatterTable[formatterName];
            formatterTable.Remove(formatterName);
            reverseFormatterTable.Remove(otherFormatter);
            formatterTable.Add(otherFormatter.GetType().VersionInvariantName(), otherFormatter);
            reverseFormatterTable.Add(otherFormatter, otherFormatter.GetType().VersionInvariantName());
            formatterName = formatter.GetType().VersionInvariantName();
          }
          simpleFormatterTable[formatter.GetType().Name] = true;
          formatterTable.Add(formatterName, formatter);
          reverseFormatterTable.Add(formatter, formatterName);

          string typeName = formatter.SourceType.IsGenericType ?
            formatter.SourceType.SimpleFullName() :
            formatter.SourceType.Name;
          if (typeNameTable.ContainsKey(typeName)) {
            Type otherType = typeNameTable[typeName];
            if (otherType != formatter.SourceType) {
              typeNameTable.Remove(typeName);
              reverseTypeNameTable.Remove(otherType);
              typeNameTable.Add(otherType.VersionInvariantName(), otherType);
              reverseTypeNameTable.Add(otherType, otherType.VersionInvariantName());
              typeName = formatter.SourceType.VersionInvariantName();
              typeNameTable.Add(typeName, formatter.SourceType);
              reverseTypeNameTable.Add(formatter.SourceType, typeName);
            }
          } else {
            typeNameTable.Add(typeName, formatter.SourceType);
            reverseTypeNameTable.Add(formatter.SourceType, typeName);
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
        foreach (var formatter in activeConfig.Formatters) {
          checkBox.Items.Add(formatter.GetType().Name + " (F)");
        }
        foreach (var decomposer in activeConfig.Decomposers)
          checkBox.Items.Add(decomposer.GetType().Name + " (D)");
      }
      checkBox.ResumeLayout();
    }


    void gridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
      UpdatePreview();
    }

    private void decomposerList_ItemDrag(object sender, ItemDragEventArgs e) {
      ListView decomposerList = (ListView)sender;
      decomposerList.DoDragDrop(decomposerList.SelectedItems, DragDropEffects.Move);
    }

    private void decomposerList_DragEnter(object sender, DragEventArgs e) {
      if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection).FullName)) {
        e.Effect = DragDropEffects.Move;
      }
    }

    private void decomposerList_DragDrop(object sender, DragEventArgs e) {
      ListView decomposerList = (ListView)sender;
      if (decomposerList.SelectedItems.Count == 0) {
        return;
      }
      Point cp = decomposerList.PointToClient(new Point(e.X, e.Y));
      ListViewItem targetItem = decomposerList.GetItemAt(cp.X, cp.Y);
      if (targetItem == null)
        return;
      int targetIndex = targetItem.Index;
      var selectedItems = new List<ListViewItem>(decomposerList.SelectedItems.Cast<ListViewItem>());
      int i = 0;
      foreach (ListViewItem dragItem in selectedItems) {
        if (targetIndex == dragItem.Index)
          return;
        if (dragItem.Index < targetIndex) {
          decomposerList.Items.Insert(targetIndex + 1, (ListViewItem)dragItem.Clone());
        } else {
          decomposerList.Items.Insert(targetIndex + i, (ListViewItem)dragItem.Clone());
        }
        decomposerList.Items.Remove(dragItem);
        i++;
      }
      UpdatePreview();
    }

    private void decomposerList_Resize(object sender, EventArgs e) {
      ListView decomposerList = (ListView)sender;
      decomposerList.Columns["DecomposerColumn"].Width = decomposerList.Width - 4;
    }


    private void decomposerList_ItemChecked(object sender, ItemCheckedEventArgs e) {
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

    private Configuration GenerateConfiguration(IFormat format, DataGridView formatterGrid, ListView decomposerList) {
      if (formatterGrid == null || decomposerList == null)
        return null;
      var formatters = new List<IFormatter>();
      foreach (DataGridViewRow row in formatterGrid.Rows) {
        if (row.Cells["Type"].Value != null &&
             row.Cells["Active"].Value != null &&
             row.Cells["Formatter"].Value != null &&
             (bool)row.Cells["Active"].Value == true) {
          formatters.Add(formatterTable[(string)row.Cells["Formatter"].Value]);
        }
      }
      var decomposers = new List<IDecomposer>();
      foreach (ListViewItem item in decomposerList.Items) {
        if (item != null && item.Checked)
          decomposers.Add((IDecomposer)item.Tag);
      }
      return new Configuration(format, formatters, decomposers);
    }

    private Configuration GetActiveConfiguration() {
      IFormat format = (IFormat)configurationTabs.SelectedTab.Tag;
      return GenerateConfiguration(format,
        (DataGridView)GetActiveControl("GridView"),
        (ListView)GetActiveControl("DecomposerList"));
    }

    private Configuration GetConfiguration(IFormat format) {
      return GenerateConfiguration(format,
       (DataGridView)GetControlsOnPage(format.Name, "GridView"),
       (ListView)GetControlsOnPage(format.Name, "DecomposerList"));
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

  public class Empty : ISerialData { }

  [EmptyStorableClass]
  public class EmptyFormat : FormatBase<Empty> {
    public override string Name { get { return "Empty"; } }
  }

  [EmptyStorableClass]
  public class EmptyFormatter : FormatterBase<Type, Empty> {

    public override Empty Format(Type o) {
      return null;
    }

    public override Type Parse(Empty o) {
      return null;
    }
  }

  [EmptyStorableClass]
  public class FakeBoolean2XmlFormatter : FormatterBase<bool, Empty> {

    public override Empty Format(bool o) {
      return null;
    }

    public override bool Parse(Empty o) {
      return false;
    }
  }

  [EmptyStorableClass]
  public class Int2XmlFormatter : FormatterBase<int, Empty> {

    public override Empty Format(int o) {
      return null;
    }

    public override int Parse(Empty o) {
      return 0;
    }

  }

  public static class TypeFormatter {

    public static string SimpleFullName(this Type type) {
      StringBuilder sb = new StringBuilder();
      SimpleFullName(type, sb);
      return sb.ToString();
    }

    private static void SimpleFullName(Type type, StringBuilder sb) {
      if (type.IsGenericType) {
        sb.Append(type.Name, 0, type.Name.LastIndexOf('`'));
        sb.Append("<");
        foreach (Type t in type.GetGenericArguments()) {
          SimpleFullName(t, sb);
          sb.Append(", ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(">");
      } else {
        sb.Append(type.Name);
      }
    }

  }

}
