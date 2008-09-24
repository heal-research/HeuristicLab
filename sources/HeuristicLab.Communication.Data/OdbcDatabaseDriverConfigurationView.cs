using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public partial class OdbcDatabaseDriverConfigurationView : ViewBase {
    public OdbcDatabaseDriverConfiguration OdbcDatabaseDriverConfiguration {
      get { return (OdbcDatabaseDriverConfiguration)base.Item; }
      set { base.Item = value; }
    }

    public OdbcDatabaseDriverConfigurationView() {
      InitializeComponent();
      BuildOdbcDriverComboBox();
    }

    public OdbcDatabaseDriverConfigurationView(OdbcDatabaseDriverConfiguration odbcDatabaseDriverConfiguration)
      : this() {
      OdbcDatabaseDriverConfiguration = odbcDatabaseDriverConfiguration;
    }

    private void BuildOdbcDriverComboBox() {
      RegistryKey lm = null;
      try {
        lm = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("ODBC").OpenSubKey("ODBCINST.INI").OpenSubKey("ODBC Drivers");
      } catch (Exception e) {
        MessageBox.Show("Unable to open registry:\n" + e.Message + "\n\nPlease provide the correct driver name for the ODBC connection driver.", "Registry error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      if (lm != null) {
        string[] odbcDrivers = lm.GetValueNames();
        odbcDriverComboBox.Items.AddRange(odbcDrivers);
        odbcDriverComboBox.SelectedIndex = 0;
      }
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (OdbcDatabaseDriverConfiguration == null) {
        odbcDriverComboBox.Enabled = false;
        ipAddressStringDataView.Enabled = false;
        ipAddressStringDataView.StringData = null;
        portIntDataView.Enabled = false;
        portIntDataView.IntData = null;
        dbNameStringDataView.Enabled = false;
        dbNameStringDataView.StringData = null;
        dbUserStringDataView.Enabled = false;
        dbUserStringDataView.StringData = null;
        dbPasswordStringDataView.Enabled = false;
        dbPasswordStringDataView.StringData = null;
        dbTableStringDataView.Enabled = false;
        dbTableStringDataView.StringData = null;
        dbIDColumnNameStringDataView.Enabled = false;
        dbIDColumnNameStringDataView.StringData = null;
        dbCommunicationColumnNameStringDataView.Enabled = false;
        dbCommunicationColumnNameStringDataView.StringData = null;
        dbSynchronizationColumnNameStringDataView.Enabled = false;
        dbSynchronizationColumnNameStringDataView.StringData = null;
      } else {
        odbcDriverComboBox.Enabled = true;
        ipAddressStringDataView.StringData = OdbcDatabaseDriverConfiguration.IPAddress;
        ipAddressStringDataView.Enabled = true;
        portIntDataView.IntData = OdbcDatabaseDriverConfiguration.Port;
        portIntDataView.Enabled = true;
        dbNameStringDataView.StringData = OdbcDatabaseDriverConfiguration.DBName;
        dbNameStringDataView.Enabled = true;
        dbUserStringDataView.StringData = OdbcDatabaseDriverConfiguration.DBUser;
        dbUserStringDataView.Enabled = true;
        dbPasswordStringDataView.StringData = OdbcDatabaseDriverConfiguration.DBPassword;
        dbPasswordStringDataView.Enabled = true;
        dbTableStringDataView.StringData = OdbcDatabaseDriverConfiguration.DBTable;
        dbTableStringDataView.Enabled = true;
        dbIDColumnNameStringDataView.StringData = OdbcDatabaseDriverConfiguration.DBIDColumnName;
        dbIDColumnNameStringDataView.Enabled = true;
        dbCommunicationColumnNameStringDataView.StringData = OdbcDatabaseDriverConfiguration.DBCommunciationColumnName;
        dbCommunicationColumnNameStringDataView.Enabled = true;
        dbSynchronizationColumnNameStringDataView.StringData = OdbcDatabaseDriverConfiguration.DBSynchronizationColumnName;
        dbSynchronizationColumnNameStringDataView.Enabled = true;
      }
    }

    private void odbcDriverComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (odbcDriverComboBox.SelectedIndex >= 0 && OdbcDatabaseDriverConfiguration != null) {
        OdbcDatabaseDriverConfiguration.OdbcDriver = new StringData((string)odbcDriverComboBox.SelectedItem);
      }
    }
  }
}
