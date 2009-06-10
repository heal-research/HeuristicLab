using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.Options {
  public partial class AddLineDialog : Form {
    private readonly IChartDataRowsModel model;

    public AddLineDialog(IChartDataRowsModel model) {
      InitializeComponent();
      this.model = model;
    }

    private void AddLineDialog_Load(object sender, EventArgs e) {
      cbAddLineDialogGeneralType.SelectedIndex = 0;
      clbAddLineDialogAggregatorLines.Enabled = false;
      clbAddLineDialogAggregatorLines.DataSource = model.Rows;
      clbAddLineDialogAggregatorLines.DisplayMember = "Label";
      cbAddLineDialogYAxis.DataSource = model.YAxes;
      cbAddLineDialogYAxis.DisplayMember = "Label";
    }

    private void cbAddLineDialogGeneralType_SelectedIndexChanged(object sender,
                                                                 EventArgs e) {
      if (cbAddLineDialogGeneralType.SelectedItem != null) {
        if (cbAddLineDialogGeneralType.SelectedIndex == 1) {
//Aggregator selected
          clbAddLineDialogAggregatorLines.Enabled = true;
          cbAddLineDialogYAxis.Enabled = false;
          Type[] types = Assembly.GetExecutingAssembly().GetTypes();
          List<Type> aggregatorTypes = new List<Type>();
          foreach (Type type in types) {
            if (type.GetInterface(typeof (IAggregator).FullName) != null) {
              aggregatorTypes.Add(type);
            }
          }
          cbAddLineDialogSubtype.DataSource = aggregatorTypes;
          cbAddLineDialogSubtype.DisplayMember = "Name";
        } else {
          cbAddLineDialogYAxis.Enabled = true;
          clbAddLineDialogAggregatorLines.Enabled = false;
          cbAddLineDialogSubtype.DataSource = null;
          cbAddLineDialogSubtype.Items.Clear();
          cbAddLineDialogSubtype.Items.Add(DataRowType.Normal);
          cbAddLineDialogSubtype.Items.Add(DataRowType.Points);
          cbAddLineDialogSubtype.Items.Add(DataRowType.SingleValue);
          cbAddLineDialogSubtype.SelectedIndex = 0;
        }
      }
    }

    private void btnAddLineDialogCancel_Click(object sender, EventArgs e) {
      Close();
    }

    private void btnAddLineDialogOk_Click(object sender, EventArgs e) {
      //TODO: Generate the new DataRow and add it to the Model

      string label = txtAddLineDialogLabel.Text;
      IDataRow newDataRow;
      if (cbAddLineDialogGeneralType.SelectedIndex == 0) {
        //Default init in Constructor is 0!
        newDataRow = new DataRow();
        switch (cbAddLineDialogSubtype.SelectedIndex) {
          case 0:
            newDataRow.RowSettings.LineType = DataRowType.Normal;
            break;
          case 1:
            newDataRow.RowSettings.LineType = DataRowType.Points;
            break;
          case 2:
            newDataRow.RowSettings.LineType = DataRowType.SingleValue;
            break;
        }

        newDataRow.YAxis = (YAxisDescriptor) cbAddLineDialogYAxis.SelectedItem;
      } else {
        if (cbAddLineDialogSubtype.SelectedValue == null)
          return;
        newDataRow =
          (IDataRow)
          Activator.CreateInstance((Type) cbAddLineDialogSubtype.SelectedValue);
        YAxisDescriptor yAxis = null;
        foreach (var item in clbAddLineDialogAggregatorLines.CheckedItems) {
          if (yAxis == null)
            yAxis = ((IDataRow) item).YAxis;
          //else if (((DataRow) item).YAxis != yAxis)
          //  throw new ArgumentException("Watches for lines on different YAxis not supported");
          ((IAggregator) newDataRow).AddWatch((IDataRow) item);
        }
        newDataRow.YAxis = yAxis;
      }
      newDataRow.RowSettings.Label = label;
      newDataRow.RowSettings.Color = Color.DarkRed;
      newDataRow.RowSettings.Thickness = 3;
      newDataRow.RowSettings.Style = DrawingStyle.Solid;
      newDataRow.RowSettings.ShowMarkers = true;
      model.AddDataRow(newDataRow);
      Close();
    }
  }
}