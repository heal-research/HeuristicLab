using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace HeuristicLab.Hive.Client.Console {
  public partial class EventLogEntryForm : Form {
    public EventLogEntryForm(HiveEventEntry curEventEntry) {
      InitializeComponent();
      lbDate.Text = curEventEntry.EventDate;
      lbId.Text = curEventEntry.ID;
      txtMessage.Text = curEventEntry.Message;
    }
  }
}
