#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace HeuristicLab.Hive.Client.Console {
  partial class HiveClientConsole {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      Calendar.DrawTool drawTool3 = new Calendar.DrawTool();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HiveClientConsole));
      this.dvOnline = new Calendar.DayView();
      this.tcClientConsole = new System.Windows.Forms.TabControl();
      this.tpConnection = new System.Windows.Forms.TabPage();
      this.gbServerConnection = new System.Windows.Forms.GroupBox();
      this.btn_clientShutdown = new System.Windows.Forms.Button();
      this.lbStatus = new System.Windows.Forms.Label();
      this.lbStatusCaption = new System.Windows.Forms.Label();
      this.btnDisconnect = new System.Windows.Forms.Button();
      this.btConnect = new System.Windows.Forms.Button();
      this.tbPort = new System.Windows.Forms.TextBox();
      this.tbIPAdress = new System.Windows.Forms.TextBox();
      this.lbServerIPCaption = new System.Windows.Forms.Label();
      this.lbPortCaption = new System.Windows.Forms.Label();
      this.gbJobCommon = new System.Windows.Forms.GroupBox();
      this.lvJobDetail = new System.Windows.Forms.ListView();
      this.chJobId = new System.Windows.Forms.ColumnHeader();
      this.chSince = new System.Windows.Forms.ColumnHeader();
      this.chProgress = new System.Windows.Forms.ColumnHeader();
      this.gbCommon = new System.Windows.Forms.GroupBox();
      this.pbGraph = new System.Windows.Forms.PictureBox();
      this.lbJobsAborted = new System.Windows.Forms.Label();
      this.lbJobdone = new System.Windows.Forms.Label();
      this.lbJobsFetched = new System.Windows.Forms.Label();
      this.lbGuid = new System.Windows.Forms.Label();
      this.lbGuidCaption = new System.Windows.Forms.Label();
      this.lbCs = new System.Windows.Forms.Label();
      this.lbConnectionStatus = new System.Windows.Forms.Label();
      this.lbConnectionStatusCaption = new System.Windows.Forms.Label();
      this.lbJobsAbortedCaption = new System.Windows.Forms.Label();
      this.lbJobdoneCaption = new System.Windows.Forms.Label();
      this.lbJobsFetchedCaption = new System.Windows.Forms.Label();
      this.lbCsCaption = new System.Windows.Forms.Label();
      this.gbEventLog = new System.Windows.Forms.GroupBox();
      this.lvLog = new System.Windows.Forms.ListView();
      this.chType = new System.Windows.Forms.ColumnHeader();
      this.chId = new System.Windows.Forms.ColumnHeader();
      this.chMessage = new System.Windows.Forms.ColumnHeader();
      this.chDate = new System.Windows.Forms.ColumnHeader();
      this.ilEventLog = new System.Windows.Forms.ImageList(this.components);
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.txttimeFrom = new System.Windows.Forms.TextBox();
      this.lblTimeFrame = new System.Windows.Forms.Label();
      this.btbDelete = new System.Windows.Forms.Button();
      this.txtTo = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.txtFrom = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btCreate = new System.Windows.Forms.Button();
      this.mcOnline = new System.Windows.Forms.MonthCalendar();
      this.txtTimeTo = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.tcClientConsole.SuspendLayout();
      this.tpConnection.SuspendLayout();
      this.gbServerConnection.SuspendLayout();
      this.gbJobCommon.SuspendLayout();
      this.gbCommon.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbGraph)).BeginInit();
      this.gbEventLog.SuspendLayout();
      this.tabPage2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // dvOnline
      // 
      drawTool3.DayView = this.dvOnline;
      this.dvOnline.ActiveTool = drawTool3;
      this.dvOnline.AmPmDisplay = false;
      this.dvOnline.AppointmentHeightMode = Calendar.AppHeightDrawMode.TrueHeightAll;
      this.dvOnline.DayHeadersHeight = 15;
      this.dvOnline.DaysToShow = 7;
      this.dvOnline.DrawAllAppBorder = false;
      this.dvOnline.EnableDurationDisplay = false;
      this.dvOnline.EnableRoundedCorners = false;
      this.dvOnline.EnableShadows = false;
      this.dvOnline.EnableTimeIndicator = false;
      this.dvOnline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
      this.dvOnline.Location = new System.Drawing.Point(9, 179);
      this.dvOnline.MinHalfHourApp = false;
      this.dvOnline.Name = "dvOnline";
      this.dvOnline.SelectionEnd = new System.DateTime(((long)(0)));
      this.dvOnline.SelectionStart = new System.DateTime(((long)(0)));
      this.dvOnline.Size = new System.Drawing.Size(823, 225);
      this.dvOnline.StartDate = new System.DateTime(((long)(0)));
      this.dvOnline.TabIndex = 18;
      // 
      // tcClientConsole
      // 
      this.tcClientConsole.Controls.Add(this.tpConnection);
      this.tcClientConsole.Controls.Add(this.tabPage2);
      this.tcClientConsole.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tcClientConsole.Location = new System.Drawing.Point(0, 0);
      this.tcClientConsole.Name = "tcClientConsole";
      this.tcClientConsole.SelectedIndex = 0;
      this.tcClientConsole.Size = new System.Drawing.Size(853, 438);
      this.tcClientConsole.TabIndex = 1;
      // 
      // tpConnection
      // 
      this.tpConnection.Controls.Add(this.gbServerConnection);
      this.tpConnection.Controls.Add(this.gbJobCommon);
      this.tpConnection.Controls.Add(this.gbCommon);
      this.tpConnection.Controls.Add(this.gbEventLog);
      this.tpConnection.Location = new System.Drawing.Point(4, 22);
      this.tpConnection.Name = "tpConnection";
      this.tpConnection.Padding = new System.Windows.Forms.Padding(3);
      this.tpConnection.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.tpConnection.Size = new System.Drawing.Size(845, 412);
      this.tpConnection.TabIndex = 0;
      this.tpConnection.Text = "Status";
      this.tpConnection.UseVisualStyleBackColor = true;
      // 
      // gbServerConnection
      // 
      this.gbServerConnection.Controls.Add(this.btn_clientShutdown);
      this.gbServerConnection.Controls.Add(this.lbStatus);
      this.gbServerConnection.Controls.Add(this.lbStatusCaption);
      this.gbServerConnection.Controls.Add(this.btnDisconnect);
      this.gbServerConnection.Controls.Add(this.btConnect);
      this.gbServerConnection.Controls.Add(this.tbPort);
      this.gbServerConnection.Controls.Add(this.tbIPAdress);
      this.gbServerConnection.Controls.Add(this.lbServerIPCaption);
      this.gbServerConnection.Controls.Add(this.lbPortCaption);
      this.gbServerConnection.Location = new System.Drawing.Point(424, 6);
      this.gbServerConnection.Name = "gbServerConnection";
      this.gbServerConnection.Size = new System.Drawing.Size(414, 133);
      this.gbServerConnection.TabIndex = 15;
      this.gbServerConnection.TabStop = false;
      this.gbServerConnection.Text = "Server connection";
      // 
      // btn_clientShutdown
      // 
      this.btn_clientShutdown.Location = new System.Drawing.Point(257, 80);
      this.btn_clientShutdown.Name = "btn_clientShutdown";
      this.btn_clientShutdown.Size = new System.Drawing.Size(147, 23);
      this.btn_clientShutdown.TabIndex = 8;
      this.btn_clientShutdown.Text = "Shutdown Client";
      this.btn_clientShutdown.UseVisualStyleBackColor = true;
      this.btn_clientShutdown.Click += new System.EventHandler(this.btn_clientShutdown_Click);
      // 
      // lbStatus
      // 
      this.lbStatus.AutoSize = true;
      this.lbStatus.Location = new System.Drawing.Point(74, 80);
      this.lbStatus.Name = "lbStatus";
      this.lbStatus.Size = new System.Drawing.Size(50, 13);
      this.lbStatus.TabIndex = 7;
      this.lbStatus.Text = "loading...";
      // 
      // lbStatusCaption
      // 
      this.lbStatusCaption.AutoSize = true;
      this.lbStatusCaption.Location = new System.Drawing.Point(17, 80);
      this.lbStatusCaption.Name = "lbStatusCaption";
      this.lbStatusCaption.Size = new System.Drawing.Size(40, 13);
      this.lbStatusCaption.TabIndex = 6;
      this.lbStatusCaption.Text = "Status:";
      // 
      // btnDisconnect
      // 
      this.btnDisconnect.Location = new System.Drawing.Point(257, 51);
      this.btnDisconnect.Name = "btnDisconnect";
      this.btnDisconnect.Size = new System.Drawing.Size(147, 23);
      this.btnDisconnect.TabIndex = 5;
      this.btnDisconnect.Text = "Disconnect from server";
      this.btnDisconnect.UseVisualStyleBackColor = true;
      this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
      // 
      // btConnect
      // 
      this.btConnect.Location = new System.Drawing.Point(257, 23);
      this.btConnect.Name = "btConnect";
      this.btConnect.Size = new System.Drawing.Size(147, 23);
      this.btConnect.TabIndex = 4;
      this.btConnect.Text = "Connect to server";
      this.btConnect.UseVisualStyleBackColor = true;
      this.btConnect.Click += new System.EventHandler(this.btConnect_Click);
      // 
      // tbPort
      // 
      this.tbPort.Location = new System.Drawing.Point(77, 51);
      this.tbPort.Name = "tbPort";
      this.tbPort.Size = new System.Drawing.Size(143, 20);
      this.tbPort.TabIndex = 3;
      // 
      // tbIPAdress
      // 
      this.tbIPAdress.Location = new System.Drawing.Point(77, 25);
      this.tbIPAdress.Name = "tbIPAdress";
      this.tbIPAdress.Size = new System.Drawing.Size(143, 20);
      this.tbIPAdress.TabIndex = 2;
      // 
      // lbServerIPCaption
      // 
      this.lbServerIPCaption.AutoSize = true;
      this.lbServerIPCaption.Location = new System.Drawing.Point(17, 28);
      this.lbServerIPCaption.Name = "lbServerIPCaption";
      this.lbServerIPCaption.Size = new System.Drawing.Size(54, 13);
      this.lbServerIPCaption.TabIndex = 0;
      this.lbServerIPCaption.Text = "IP adress:";
      // 
      // lbPortCaption
      // 
      this.lbPortCaption.AutoSize = true;
      this.lbPortCaption.Location = new System.Drawing.Point(17, 54);
      this.lbPortCaption.Name = "lbPortCaption";
      this.lbPortCaption.Size = new System.Drawing.Size(29, 13);
      this.lbPortCaption.TabIndex = 1;
      this.lbPortCaption.Text = "Port:";
      // 
      // gbJobCommon
      // 
      this.gbJobCommon.Controls.Add(this.lvJobDetail);
      this.gbJobCommon.Location = new System.Drawing.Point(8, 152);
      this.gbJobCommon.Name = "gbJobCommon";
      this.gbJobCommon.Size = new System.Drawing.Size(412, 252);
      this.gbJobCommon.TabIndex = 13;
      this.gbJobCommon.TabStop = false;
      this.gbJobCommon.Text = "Jobs detail";
      // 
      // lvJobDetail
      // 
      this.lvJobDetail.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chJobId,
            this.chSince,
            this.chProgress});
      this.lvJobDetail.FullRowSelect = true;
      this.lvJobDetail.GridLines = true;
      this.lvJobDetail.Location = new System.Drawing.Point(6, 14);
      this.lvJobDetail.Name = "lvJobDetail";
      this.lvJobDetail.Size = new System.Drawing.Size(398, 232);
      this.lvJobDetail.TabIndex = 0;
      this.lvJobDetail.UseCompatibleStateImageBehavior = false;
      this.lvJobDetail.View = System.Windows.Forms.View.Details;
      // 
      // chJobId
      // 
      this.chJobId.Text = "ID";
      // 
      // chSince
      // 
      this.chSince.Text = "Since";
      this.chSince.Width = 120;
      // 
      // chProgress
      // 
      this.chProgress.Text = "Progress";
      this.chProgress.Width = 200;
      // 
      // gbCommon
      // 
      this.gbCommon.Controls.Add(this.pbGraph);
      this.gbCommon.Controls.Add(this.lbJobsAborted);
      this.gbCommon.Controls.Add(this.lbJobdone);
      this.gbCommon.Controls.Add(this.lbJobsFetched);
      this.gbCommon.Controls.Add(this.lbGuid);
      this.gbCommon.Controls.Add(this.lbGuidCaption);
      this.gbCommon.Controls.Add(this.lbCs);
      this.gbCommon.Controls.Add(this.lbConnectionStatus);
      this.gbCommon.Controls.Add(this.lbConnectionStatusCaption);
      this.gbCommon.Controls.Add(this.lbJobsAbortedCaption);
      this.gbCommon.Controls.Add(this.lbJobdoneCaption);
      this.gbCommon.Controls.Add(this.lbJobsFetchedCaption);
      this.gbCommon.Controls.Add(this.lbCsCaption);
      this.gbCommon.Location = new System.Drawing.Point(8, 6);
      this.gbCommon.Name = "gbCommon";
      this.gbCommon.Size = new System.Drawing.Size(410, 133);
      this.gbCommon.TabIndex = 14;
      this.gbCommon.TabStop = false;
      this.gbCommon.Text = "Common";
      // 
      // pbGraph
      // 
      this.pbGraph.Location = new System.Drawing.Point(292, 19);
      this.pbGraph.Name = "pbGraph";
      this.pbGraph.Size = new System.Drawing.Size(112, 108);
      this.pbGraph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pbGraph.TabIndex = 21;
      this.pbGraph.TabStop = false;
      // 
      // lbJobsAborted
      // 
      this.lbJobsAborted.AutoSize = true;
      this.lbJobsAborted.Location = new System.Drawing.Point(195, 114);
      this.lbJobsAborted.Name = "lbJobsAborted";
      this.lbJobsAborted.Size = new System.Drawing.Size(50, 13);
      this.lbJobsAborted.TabIndex = 20;
      this.lbJobsAborted.Text = "loading...";
      // 
      // lbJobdone
      // 
      this.lbJobdone.AutoSize = true;
      this.lbJobdone.Location = new System.Drawing.Point(62, 114);
      this.lbJobdone.Name = "lbJobdone";
      this.lbJobdone.Size = new System.Drawing.Size(50, 13);
      this.lbJobdone.TabIndex = 19;
      this.lbJobdone.Text = "loading...";
      // 
      // lbJobsFetched
      // 
      this.lbJobsFetched.AutoSize = true;
      this.lbJobsFetched.Location = new System.Drawing.Point(74, 90);
      this.lbJobsFetched.Name = "lbJobsFetched";
      this.lbJobsFetched.Size = new System.Drawing.Size(50, 13);
      this.lbJobsFetched.TabIndex = 18;
      this.lbJobsFetched.Text = "loading...";
      // 
      // lbGuid
      // 
      this.lbGuid.AutoSize = true;
      this.lbGuid.Location = new System.Drawing.Point(41, 16);
      this.lbGuid.Name = "lbGuid";
      this.lbGuid.Size = new System.Drawing.Size(50, 13);
      this.lbGuid.TabIndex = 15;
      this.lbGuid.Text = "loading...";
      // 
      // lbGuidCaption
      // 
      this.lbGuidCaption.AutoSize = true;
      this.lbGuidCaption.Location = new System.Drawing.Point(9, 16);
      this.lbGuidCaption.Name = "lbGuidCaption";
      this.lbGuidCaption.Size = new System.Drawing.Size(37, 13);
      this.lbGuidCaption.TabIndex = 8;
      this.lbGuidCaption.Text = "GUID:";
      // 
      // lbCs
      // 
      this.lbCs.AutoSize = true;
      this.lbCs.Location = new System.Drawing.Point(94, 40);
      this.lbCs.Name = "lbCs";
      this.lbCs.Size = new System.Drawing.Size(50, 13);
      this.lbCs.TabIndex = 17;
      this.lbCs.Text = "loading...";
      // 
      // lbConnectionStatus
      // 
      this.lbConnectionStatus.AutoSize = true;
      this.lbConnectionStatus.Location = new System.Drawing.Point(97, 66);
      this.lbConnectionStatus.Name = "lbConnectionStatus";
      this.lbConnectionStatus.Size = new System.Drawing.Size(50, 13);
      this.lbConnectionStatus.TabIndex = 16;
      this.lbConnectionStatus.Text = "loading...";
      // 
      // lbConnectionStatusCaption
      // 
      this.lbConnectionStatusCaption.AutoSize = true;
      this.lbConnectionStatusCaption.Location = new System.Drawing.Point(9, 66);
      this.lbConnectionStatusCaption.Name = "lbConnectionStatusCaption";
      this.lbConnectionStatusCaption.Size = new System.Drawing.Size(93, 13);
      this.lbConnectionStatusCaption.TabIndex = 13;
      this.lbConnectionStatusCaption.Text = "Connected status:";
      // 
      // lbJobsAbortedCaption
      // 
      this.lbJobsAbortedCaption.AutoSize = true;
      this.lbJobsAbortedCaption.Location = new System.Drawing.Point(129, 114);
      this.lbJobsAbortedCaption.Name = "lbJobsAbortedCaption";
      this.lbJobsAbortedCaption.Size = new System.Drawing.Size(71, 13);
      this.lbJobsAbortedCaption.TabIndex = 11;
      this.lbJobsAbortedCaption.Text = "Jobs aborted:";
      // 
      // lbJobdoneCaption
      // 
      this.lbJobdoneCaption.AutoSize = true;
      this.lbJobdoneCaption.Location = new System.Drawing.Point(9, 114);
      this.lbJobdoneCaption.Name = "lbJobdoneCaption";
      this.lbJobdoneCaption.Size = new System.Drawing.Size(59, 13);
      this.lbJobdoneCaption.TabIndex = 10;
      this.lbJobdoneCaption.Text = "Jobs done:";
      // 
      // lbJobsFetchedCaption
      // 
      this.lbJobsFetchedCaption.AutoSize = true;
      this.lbJobsFetchedCaption.Location = new System.Drawing.Point(9, 90);
      this.lbJobsFetchedCaption.Name = "lbJobsFetchedCaption";
      this.lbJobsFetchedCaption.Size = new System.Drawing.Size(71, 13);
      this.lbJobsFetchedCaption.TabIndex = 12;
      this.lbJobsFetchedCaption.Text = "Jobs fetched:";
      // 
      // lbCsCaption
      // 
      this.lbCsCaption.AutoSize = true;
      this.lbCsCaption.Location = new System.Drawing.Point(9, 40);
      this.lbCsCaption.Name = "lbCsCaption";
      this.lbCsCaption.Size = new System.Drawing.Size(90, 13);
      this.lbCsCaption.TabIndex = 9;
      this.lbCsCaption.Text = "Connected since:";
      // 
      // gbEventLog
      // 
      this.gbEventLog.Controls.Add(this.lvLog);
      this.gbEventLog.Location = new System.Drawing.Point(426, 152);
      this.gbEventLog.Name = "gbEventLog";
      this.gbEventLog.Size = new System.Drawing.Size(412, 252);
      this.gbEventLog.TabIndex = 7;
      this.gbEventLog.TabStop = false;
      this.gbEventLog.Text = "Hive Client Log";
      // 
      // lvLog
      // 
      this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chType,
            this.chId,
            this.chMessage,
            this.chDate});
      this.lvLog.FullRowSelect = true;
      this.lvLog.GridLines = true;
      this.lvLog.LargeImageList = this.ilEventLog;
      this.lvLog.Location = new System.Drawing.Point(8, 14);
      this.lvLog.MultiSelect = false;
      this.lvLog.Name = "lvLog";
      this.lvLog.Size = new System.Drawing.Size(398, 232);
      this.lvLog.SmallImageList = this.ilEventLog;
      this.lvLog.TabIndex = 6;
      this.lvLog.UseCompatibleStateImageBehavior = false;
      this.lvLog.View = System.Windows.Forms.View.Details;
      this.lvLog.DoubleClick += new System.EventHandler(this.lvLog_DoubleClick);
      this.lvLog.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvLog_ColumnClick);
      // 
      // chType
      // 
      this.chType.Text = "Type";
      this.chType.Width = 42;
      // 
      // chId
      // 
      this.chId.Text = "ID";
      this.chId.Width = 50;
      // 
      // chMessage
      // 
      this.chMessage.Text = "Message";
      this.chMessage.Width = 163;
      // 
      // chDate
      // 
      this.chDate.Text = "Date";
      this.chDate.Width = 125;
      // 
      // ilEventLog
      // 
      this.ilEventLog.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilEventLog.ImageStream")));
      this.ilEventLog.TransparentColor = System.Drawing.Color.Transparent;
      this.ilEventLog.Images.SetKeyName(0, "Info.png");
      this.ilEventLog.Images.SetKeyName(1, "Error.png");
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.pictureBox1);
      this.tabPage2.Controls.Add(this.groupBox1);
      this.tabPage2.Controls.Add(this.mcOnline);
      this.tabPage2.Controls.Add(this.dvOnline);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(845, 412);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Schedule";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::HeuristicLab.Hive.Client.Console.Properties.Resources.schedule;
      this.pictureBox1.Location = new System.Drawing.Point(586, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(246, 155);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 22;
      this.pictureBox1.TabStop = false;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.txtTimeTo);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.txttimeFrom);
      this.groupBox1.Controls.Add(this.lblTimeFrame);
      this.groupBox1.Controls.Add(this.btbDelete);
      this.groupBox1.Controls.Add(this.txtTo);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.txtFrom);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.btCreate);
      this.groupBox1.Location = new System.Drawing.Point(354, 7);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(215, 161);
      this.groupBox1.TabIndex = 21;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Create";
      // 
      // txttimeFrom
      // 
      this.txttimeFrom.Location = new System.Drawing.Point(90, 89);
      this.txttimeFrom.Name = "txttimeFrom";
      this.txttimeFrom.Size = new System.Drawing.Size(31, 20);
      this.txttimeFrom.TabIndex = 27;
      // 
      // lblTimeFrame
      // 
      this.lblTimeFrame.AutoSize = true;
      this.lblTimeFrame.Location = new System.Drawing.Point(51, 92);
      this.lblTimeFrame.Name = "lblTimeFrame";
      this.lblTimeFrame.Size = new System.Drawing.Size(33, 13);
      this.lblTimeFrame.TabIndex = 26;
      this.lblTimeFrame.Text = "From:";
      // 
      // btbDelete
      // 
      this.btbDelete.Location = new System.Drawing.Point(132, 129);
      this.btbDelete.Name = "btbDelete";
      this.btbDelete.Size = new System.Drawing.Size(65, 26);
      this.btbDelete.TabIndex = 25;
      this.btbDelete.Text = "Delete";
      this.btbDelete.UseVisualStyleBackColor = true;
      this.btbDelete.Click += new System.EventHandler(this.btbDelete_Click);
      // 
      // txtTo
      // 
      this.txtTo.Location = new System.Drawing.Point(54, 54);
      this.txtTo.Name = "txtTo";
      this.txtTo.Size = new System.Drawing.Size(143, 20);
      this.txtTo.TabIndex = 24;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(15, 57);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(23, 13);
      this.label2.TabIndex = 23;
      this.label2.Text = "To:";
      // 
      // txtFrom
      // 
      this.txtFrom.Location = new System.Drawing.Point(54, 23);
      this.txtFrom.Name = "txtFrom";
      this.txtFrom.Size = new System.Drawing.Size(143, 20);
      this.txtFrom.TabIndex = 22;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(15, 26);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(33, 13);
      this.label1.TabIndex = 21;
      this.label1.Text = "From:";
      // 
      // btCreate
      // 
      this.btCreate.Location = new System.Drawing.Point(54, 129);
      this.btCreate.Name = "btCreate";
      this.btCreate.Size = new System.Drawing.Size(58, 26);
      this.btCreate.TabIndex = 20;
      this.btCreate.Text = "Save";
      this.btCreate.UseVisualStyleBackColor = true;
      this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
      // 
      // mcOnline
      // 
      this.mcOnline.CalendarDimensions = new System.Drawing.Size(2, 1);
      this.mcOnline.Location = new System.Drawing.Point(9, 12);
      this.mcOnline.Name = "mcOnline";
      this.mcOnline.TabIndex = 19;
      this.mcOnline.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.mcOnline_DateChanged);
      // 
      // txtTimeTo
      // 
      this.txtTimeTo.Location = new System.Drawing.Point(166, 89);
      this.txtTimeTo.Name = "txtTimeTo";
      this.txtTimeTo.Size = new System.Drawing.Size(31, 20);
      this.txtTimeTo.TabIndex = 29;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(137, 92);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(23, 13);
      this.label3.TabIndex = 28;
      this.label3.Text = "To:";
      // 
      // HiveClientConsole
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.ClientSize = new System.Drawing.Size(853, 438);
      this.Controls.Add(this.tcClientConsole);
      this.Name = "HiveClientConsole";
      this.Text = "Client Console (loading...)";
      this.Load += new System.EventHandler(this.HiveClientConsole_Load);
      this.Resize += new System.EventHandler(this.HiveClientConsole_Resize);
      this.tcClientConsole.ResumeLayout(false);
      this.tpConnection.ResumeLayout(false);
      this.gbServerConnection.ResumeLayout(false);
      this.gbServerConnection.PerformLayout();
      this.gbJobCommon.ResumeLayout(false);
      this.gbCommon.ResumeLayout(false);
      this.gbCommon.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbGraph)).EndInit();
      this.gbEventLog.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tcClientConsole;
    private System.Windows.Forms.TabPage tpConnection;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.ListView lvLog;
    private System.Windows.Forms.ColumnHeader chType;
    private System.Windows.Forms.ColumnHeader chMessage;
    private System.Windows.Forms.ColumnHeader chDate;
    private System.Windows.Forms.Label lbJobdoneCaption;
    private System.Windows.Forms.Label lbCsCaption;
    private System.Windows.Forms.Label lbGuidCaption;
    private System.Windows.Forms.GroupBox gbEventLog;
    private System.Windows.Forms.Label lbJobsFetchedCaption;
    private System.Windows.Forms.Label lbJobsAbortedCaption;
    private System.Windows.Forms.GroupBox gbJobCommon;
    private System.Windows.Forms.GroupBox gbCommon;
    private System.Windows.Forms.ListView lvJobDetail;
    private System.Windows.Forms.ColumnHeader chJobId;
    private System.Windows.Forms.ColumnHeader chSince;
    private System.Windows.Forms.ColumnHeader chProgress;
    private System.Windows.Forms.Label lbConnectionStatusCaption;
    private System.Windows.Forms.ColumnHeader chId;
    private System.Windows.Forms.ImageList ilEventLog;
    private System.Windows.Forms.Label lbCs;
    private System.Windows.Forms.Label lbConnectionStatus;
    private System.Windows.Forms.Label lbGuid;
    private System.Windows.Forms.Label lbJobsFetched;
    private System.Windows.Forms.Label lbJobdone;
    private System.Windows.Forms.Label lbJobsAborted;
    private System.Windows.Forms.PictureBox pbGraph;
    private System.Windows.Forms.GroupBox gbServerConnection;
    private System.Windows.Forms.Button btn_clientShutdown;
    private System.Windows.Forms.Label lbStatus;
    private System.Windows.Forms.Label lbStatusCaption;
    private System.Windows.Forms.Button btnDisconnect;
    private System.Windows.Forms.Button btConnect;
    private System.Windows.Forms.TextBox tbPort;
    private System.Windows.Forms.TextBox tbIPAdress;
    private System.Windows.Forms.Label lbServerIPCaption;
    private System.Windows.Forms.Label lbPortCaption;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button btCreate;
    private System.Windows.Forms.MonthCalendar mcOnline;
    private Calendar.DayView dvOnline;
    private System.Windows.Forms.TextBox txtTo;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtFrom;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button btbDelete;
    private System.Windows.Forms.TextBox txttimeFrom;
    private System.Windows.Forms.Label lblTimeFrame;
    private System.Windows.Forms.TextBox txtTimeTo;
    private System.Windows.Forms.Label label3;
  }
}

