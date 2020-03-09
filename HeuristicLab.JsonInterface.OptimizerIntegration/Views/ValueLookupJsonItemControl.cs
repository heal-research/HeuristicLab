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

namespace HeuristicLab.JsonInterface.OptimizerIntegration.Views {
  public partial class ValueLookupJsonItemControl : LookupJsonItemControl {
    private IDictionary<Type, Type> JI2VM { get; set; }

    public ValueLookupJsonItemControl() {
      InitializeComponent();
    }
    public ValueLookupJsonItemControl(IValueLookupJsonItemVM vm) : base(vm) {
      InitializeComponent();
      InitCache();
      if (JI2VM.TryGetValue(vm.JsonItemReference.GetType(), out Type vmType)) {
        IJsonItemVM tmp = (IJsonItemVM)Activator.CreateInstance(vmType);
        content.Controls.Add(tmp.Control);
      } else {
        //node.
      }
    }

    private void InitCache() {
      JI2VM = new Dictionary<Type, Type>();
      foreach (var vmType in ApplicationManager.Manager.GetTypes(typeof(IJsonItemVM))) {
        IJsonItemVM vm = (IJsonItemVM)Activator.CreateInstance(vmType);
        JI2VM.Add(vm.JsonItemType, vmType);
      }
    }
  }
}
