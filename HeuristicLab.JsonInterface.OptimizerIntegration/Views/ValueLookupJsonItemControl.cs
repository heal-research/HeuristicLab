using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public partial class ValueLookupJsonItemControl : UserControl {

    private static IDictionary<Type, Type> ji2vm = null;
    private static IDictionary<Type, Type> JI2VM { 
      get {
        if (ji2vm == null) {
          ji2vm = new Dictionary<Type, Type>();
          foreach (var vmType in ApplicationManager.Manager.GetTypes(typeof(IJsonItemVM))) {
            IJsonItemVM vm = (IJsonItemVM)Activator.CreateInstance(vmType);
            ji2vm.Add(vm.TargetedJsonItemType, vmType);
          }
        }
        return ji2vm;
      } 
    }

    private ValueLookupJsonItemControl() {
      InitializeComponent();
    }

    protected ValueLookupJsonItemControl(IValueLookupJsonItemVM vm) {
      InitializeComponent();
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

    public static ValueLookupJsonItemControl Create(IValueLookupJsonItemVM vm) => new ValueLookupJsonItemControl(vm);
  }
}
