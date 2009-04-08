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

namespace HeuristicLab.Persistence.GUI {

  public partial class PersistenceConfigurationForm : Form {

    private readonly Dictionary<string, IFormatter> formatterTable;
    private readonly Dictionary<IFormatter, string> reverseFormatterTable;    
    private readonly Dictionary<string, Type> typeNameTable;
    private readonly Dictionary<Type, string> reverseTypeNameTable;

    public PersistenceConfigurationForm() {      
      InitializeComponent();
      formatterTable = new Dictionary<string, IFormatter>();
      reverseFormatterTable = new Dictionary<IFormatter, string>();
      typeNameTable = new Dictionary<string, Type>();
      reverseTypeNameTable = new Dictionary<Type, string>();      
      initializeConfigPages();      
      UpdateFromConfigurationService();
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
    }    

    private void UpdateFromConfigurationService() {
      foreach (IFormat format in ConfigurationService.Instance.Formatters.Keys) {
        Configuration config = ConfigurationService.Instance.GetConfiguration(format);        
        UpdateFormatterGrid(
          (DataGridView)GetControlsOnPage(format.Name, "GridView"),
          config);        
        UpdateDecomposerList(
          (ListView)GetControlsOnPage(format.Name, "DecomposerList"),
          config);
      }
    }    

    private void initializeConfigPages() {
      configurationTabs.TabPages.Clear();
      foreach ( var formats in ConfigurationService.Instance.Formatters ) {        
        TabPage page = new TabPage(formats.Key.Name) {
          Name = formats.Key.Name,
          Tag = formats.Key,
        };
        configurationTabs.TabPages.Add(page);
        SplitContainer verticalSplit = new SplitContainer {
          Dock = DockStyle.Fill,
          Orientation = Orientation.Vertical,
          BorderStyle = BorderStyle.Fixed3D,
        };
        page.Controls.Add(verticalSplit);
        SplitContainer horizontalSplit = new SplitContainer {
          Dock = DockStyle.Fill,
          Orientation = Orientation.Horizontal,
          BorderStyle = BorderStyle.Fixed3D,
        };
        verticalSplit.Panel1.Controls.Add(horizontalSplit);
        ListView decomposerList = createDecomposerList();
        horizontalSplit.Panel1.Controls.Add(decomposerList);
        DataGridView gridView = createGridView();
        verticalSplit.Panel2.Controls.Add(gridView);        
        fillDataGrid(gridView, formats.Value);
        ListBox checkBox = new ListBox {
          Name = "CheckBox",
          Dock = DockStyle.Fill,
          Enabled = false,
        };
        horizontalSplit.Panel2.Controls.Add(checkBox);
      }
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
      return decomposerList;
    }

    private void fillDataGrid(DataGridView gridView, IEnumerable<IFormatter> formatters) {      
      updateNameTables(formatters);
      Dictionary<string, List<string>> formatterMap = createFormatterMap(formatters);
      foreach ( var formatterMapping in formatterMap ) {
        var row = gridView.Rows[gridView.Rows.Add()];
        row.Cells["Type"].Value = formatterMapping.Key;
        row.Cells["Type"].ToolTipText = formatterMapping.Key;
        row.Cells["Active"].Value = true;        
        var comboBoxCell = (DataGridViewComboBoxCell) row.Cells["Formatter"];          
        foreach ( var formatter in formatterMapping.Value ) {
          comboBoxCell.Items.Add(formatter);
        }          
        comboBoxCell.Value = comboBoxCell.Items[0];          
        comboBoxCell.ToolTipText = comboBoxCell.Items[0].ToString();
        if (comboBoxCell.Items.Count == 1) {
          comboBoxCell.ReadOnly = true;
          comboBoxCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        }     
      }
    }

    private Dictionary<string, List<string>> createFormatterMap(IEnumerable<IFormatter> formatters) {
      var formatterMap = new Dictionary<string, List<string>>();      
      foreach (var formatter in formatters) {
        string formatterName = reverseFormatterTable[formatter];
        string typeName = reverseTypeNameTable[formatter.Type];          
        if (!formatterMap.ContainsKey(typeName))
          formatterMap.Add(typeName, new List<string>());
        formatterMap[typeName].Add(formatterName);
      }
      return formatterMap;
    }

    private void updateNameTables(IEnumerable<IFormatter> formatters) {
      foreach (var formatter in formatters) {
        string formatterName = formatter.GetType().Name;
        if (formatterTable.ContainsKey(formatterName)) {
          IFormatter otherFormatter = formatterTable[formatterName];
          formatterTable.Remove(formatterName);
          reverseFormatterTable.Remove(otherFormatter);
          formatterTable.Add(otherFormatter.GetType().VersionInvariantName(), otherFormatter);
          reverseFormatterTable.Add(otherFormatter, otherFormatter.GetType().VersionInvariantName());
          formatterName = formatter.GetType().VersionInvariantName();
        }
        formatterTable.Add(formatterName, formatter);
        reverseFormatterTable.Add(formatter, formatterName);

        string typeName = formatter.Type.IsGenericType ?
          formatter.Type.SimpleFullName() : 
          formatter.Type.Name;
        if (typeNameTable.ContainsKey(typeName)) {
          Type otherType = typeNameTable[typeName];
          if (otherType != formatter.Type) {
            typeNameTable.Remove(typeName);
            reverseTypeNameTable.Remove(otherType);
            typeNameTable.Add(otherType.VersionInvariantName(), otherType);
            reverseTypeNameTable.Add(otherType, otherType.VersionInvariantName());
            typeName = formatter.Type.VersionInvariantName();
            typeNameTable.Add(typeName, formatter.Type);
            reverseTypeNameTable.Add(formatter.Type, typeName);
          }
        } else {
          typeNameTable.Add(typeName, formatter.Type);
          reverseTypeNameTable.Add(formatter.Type, typeName);
        }          
      }
    }    

    void gridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
      UpdatePreview();
    }    

    private void decomposerList_ItemDrag(object sender, ItemDragEventArgs e) {
      ListView decomposerList = (ListView) sender;
      decomposerList.DoDragDrop(decomposerList.SelectedItems, DragDropEffects.Move);
    }

    private void decomposerList_DragEnter(object sender, DragEventArgs e) {      
      if ( e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection).FullName)) {
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
      foreach ( ListViewItem dragItem in selectedItems ) {                
        if (targetIndex == dragItem.Index)
          return;
        if (dragItem.Index < targetIndex) {
          decomposerList.Items.Insert(targetIndex + 1, (ListViewItem) dragItem.Clone());
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
    
    private void UpdatePreview() {
      ListBox checkBox = (ListBox)GetActiveControl("CheckBox");      
      IFormat activeFormat = (IFormat)configurationTabs.SelectedTab.Tag;
      if (activeFormat != null && checkBox != null ) {
        checkBox.Items.Clear();
        Configuration activeConfig = GetActiveConfiguration();
        foreach (var formatter in activeConfig.Formatters) {
          checkBox.Items.Add(formatter.GetType().Name + " (F)");
        }
        foreach (var decomposer in activeConfig.Decomposers)
          checkBox.Items.Add(decomposer.GetType().Name + " (D)");      
      }      
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

    private Configuration GenerateConfiguration(DataGridView formatterGrid, ListView decomposerList) {
      if (formatterGrid == null || decomposerList == null)
        return null;
      var formatters = new Dictionary<Type, IFormatter>();
      foreach (DataGridViewRow row in formatterGrid.Rows) {
        if (row.Cells["Type"].Value != null &&
             row.Cells["Active"].Value != null &&
             row.Cells["Formatter"].Value != null &&
             (bool)row.Cells["Active"].Value == true) {
          formatters.Add(
            typeNameTable[(string)row.Cells["Type"].Value],
            formatterTable[(string)row.Cells["Formatter"].Value]);
        }
      }
      var decomposers = new List<IDecomposer>();
      foreach (ListViewItem item in decomposerList.Items) {
        if (item != null && item.Checked)
          decomposers.Add((IDecomposer)item.Tag);
      }
      return new Configuration(formatters, decomposers);
    }

    private Configuration GetActiveConfiguration() {      
      return GenerateConfiguration(
        (DataGridView)GetActiveControl("GridView"),
        (ListView)GetActiveControl("DecomposerList"));
    }

    private Configuration GetConfiguration(IFormat format) {
       return GenerateConfiguration(
        (DataGridView)GetControlsOnPage(format.Name, "GridView"),
        (ListView)GetControlsOnPage(format.Name, "DecomposerList"));
    }
    
    private void updateButton_Click(object sender, EventArgs e) {
      IFormat format = (IFormat)configurationTabs.SelectedTab.Tag;
      if (format != null)
        ConfigurationService.Instance.DefineConfiguration(
          format,
          GetActiveConfiguration());
    }    
    
  }

  [EmptyStorableClass]
  public class EmptyFormat : FormatBase {
    public override string Name { get { return "Empty"; } }
    public static EmptyFormat Instance = new EmptyFormat();
  }

  [EmptyStorableClass]
  public class EmptyFormatter : IFormatter {

    public Type Type { get { return typeof(Type); } }
    public IFormat  Format { get { return EmptyFormat.Instance; } }

    public object DoFormat(object o) {
 	   return null;
    }

    public object Parse(object o) {
      return null;
    }
  }

  [EmptyStorableClass]
  public class FakeBoolean2XmlFormatter : IFormatter {    

    public Type Type { get { return typeof (Boolean); } }

    public IFormat Format { get { return XmlFormat.Instance; } }

    public object DoFormat(object o) {
      return null;
    }

    public object Parse(object o) {
      return null;
    }    
  }  

  [EmptyStorableClass]
  public class Int2XmlFormatter: IFormatter {
    public Type Type { get { return typeof (int);  } }
    public IFormat Format { get { return XmlFormat.Instance; } }
    public object DoFormat(object o) {
      return null;
    }
    public object Parse(object o) {
      return null;
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
