using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class JsonItemValidValuesControl : UserControl {

    StringValueVM VM { get; set; }

    public JsonItemValidValuesControl(StringValueVM vm) {
      InitializeComponent();
      VM = vm;
      if (VM.Item.ConcreteRestrictedItems != null) {
        concreteItemsRestrictor.OnChecked += AddComboOption;
        concreteItemsRestrictor.OnUnchecked += RemoveComboOption;
        concreteItemsRestrictor.Init(VM.Item.ConcreteRestrictedItems);
        comboBoxValues.DataBindings.Add("SelectedItem", VM, nameof(StringValueVM.Value));
      } else {
        groupBoxRange.Hide();
        TextBox tb = new TextBox();
        tableLayoutPanel2.Controls.Remove(comboBoxValues);
        tableLayoutPanel2.Controls.Add(tb, 1, 0);

        tb.Location = comboBoxValues.Location;
        tb.Margin = new Padding(0);
        tb.Size = new Size(comboBoxValues.Size.Width, 20);
        tb.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        tb.Dock = DockStyle.Fill;

        tb.DataBindings.Add("Text", VM, nameof(StringValueVM.Value));
        tb.Show();
      }
    }

    private void AddComboOption(object opt) {
      comboBoxValues.Items.Add(opt);
      IList<string> items = new List<string>();
      foreach (var i in comboBoxValues.Items) {
        items.Add((string)i);
      }
      VM.Range = items;
      comboBoxValues.Enabled = true;
    }

    private void RemoveComboOption(object opt) {
      comboBoxValues.Items.Remove(opt);
      IList<string> items = new List<string>();
      foreach (var i in comboBoxValues.Items) {
        items.Add((string)i);
      }
      VM.Range = items;
      if (VM.Range.Count() <= 0) {
        comboBoxValues.Enabled = false;
        comboBoxValues.SelectedIndex = -1;
      }
    }
  }
}
