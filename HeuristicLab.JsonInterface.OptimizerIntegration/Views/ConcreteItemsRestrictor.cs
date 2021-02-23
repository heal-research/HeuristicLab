using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ConcreteItemsRestrictor : UserControl {

    public event Action<object> OnChecked;
    public event Action<object> OnUnchecked;

    protected ConcreteItemsRestrictor() {
      InitializeComponent();
    }

    public static ConcreteItemsRestrictor Create<T>(IEnumerable<T> objs) {
      ConcreteItemsRestrictor ctrl = new ConcreteItemsRestrictor();
      ctrl.Init<T>(objs);
      return ctrl;
    }

    protected void Init<T>(IEnumerable<T> objs) {
      if(objs != null)
        foreach(var obj in objs)
          SetupOption(obj);
    }

    private void SetupOption(object opt) {
      AddComboOption(opt);
      TextBox tb = new TextBox();
      tb.Text = opt.ToString();
      tb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      tb.ReadOnly = true;

      CheckBox checkBox = new CheckBox();
      checkBox.Checked = true;

      checkBox.CheckStateChanged += (o, args) => {
        if (checkBox.Checked)
          AddComboOption(opt);
        else
          RemoveComboOption(opt);
      };
      tableOptions.Controls.Add(checkBox);
      tableOptions.Controls.Add(tb);
    }

    private void AddComboOption(object obj) {
      OnChecked?.Invoke(obj);
      tableOptions.Refresh();
    }

    private void RemoveComboOption(object obj) {
      OnUnchecked?.Invoke(obj);
      tableOptions.Refresh();
    }
  }
}
