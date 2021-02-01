using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ValueLookupJsonItemControl : UserControl {
    private static IDictionary<Type, Type> JI2VM { get; set; }

    public ValueLookupJsonItemControl(IValueLookupJsonItemVM vm) {
      InitializeComponent();
      InitCache();
      if (vm.JsonItemReference != null && JI2VM.TryGetValue(vm.JsonItemReference.GetType(), out Type vmType)) {
        IJsonItemVM tmp = (IJsonItemVM)Activator.CreateInstance(vmType);
        tmp.Item = vm.JsonItemReference;
        content.Controls.Clear();
        UserControl control = tmp.Control;
        if(control != null) {
          content.Controls.Add(control);
          control.Dock = DockStyle.Fill;
        }
      }
    }

    private void InitCache() {
      if(JI2VM == null) {
        JI2VM = new Dictionary<Type, Type>();
        foreach (var vmType in ApplicationManager.Manager.GetTypes(typeof(IJsonItemVM))) {
          IJsonItemVM vm = (IJsonItemVM)Activator.CreateInstance(vmType);
          JI2VM.Add(vm.TargetedJsonItemType, vmType);
        }
      }
    }
  }
}
