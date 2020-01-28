using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public class JsonItemIntValueControl : JsonItemValueControl {

    public JsonItemIntValueControl(SingleValueVM<int> vm) : base(vm) { }

  }

  public class JsonItemDoubleValueControl : JsonItemValueControl {
    private SingleValueVM<double> VM { get; set; }

    public JsonItemDoubleValueControl(SingleValueVM<double> vm) : base(vm) {
      VM = vm;
    }

  }

  public abstract partial class JsonItemValueControl : JsonItemBaseControl {

    public JsonItemValueControl(JsonItemVMBase vm) : base(vm) {
      InitializeComponent();
    }

  }
}
