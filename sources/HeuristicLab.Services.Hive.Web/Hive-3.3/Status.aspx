<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Status.aspx.cs" Inherits="Status" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
  Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>HeuristicLab Hive Status Monitor</title> 
  <link rel="icon" type="image/ico" href="HeuristicLab.ico" /> 
    <style type="text/css">
        .auto-style1 {
            color: #F6921B;
        }
        .auto-style2 {
            color: #000000;
        }
    </style>
</head>
<body>
  <center>
    <h1 class="auto-style1">HeuristicLab <span class="auto-style2">Hive Status Monitor</span></h1>
  </center>
  <form id="form1" runat="server">
  <div>
      <strong>Overall Available Cores:</strong>
    <asp:Label ID="overallAvailableCoresLabel" runat="server" />
      <br />
      <strong>Available Cores (real):</strong>
    <asp:Label ID="availableCoresLabel" runat="server" />
      <br />
      <strong>Used Cores / Calculating Jobs:</strong>
    <asp:Label ID="usedCoresLabel" runat="server" />
    <br />
      <strong>Waiting Jobs:</strong>
    <asp:Label ID="waitingJobsLabel" runat="server" />
    <br />
      <strong>Overall
    Avg. CPU Utilization:</strong>
    <asp:Label ID="overallCpuUtilizationLabel" runat="server" />
      <br />
      <strong>Real Avg. CPU Utilization:</strong>
    <asp:Label ID="cpuUtilizationLabel" runat="server" />
      &nbsp;<br />
      <strong>Slaves (CPU Utilization):</strong>
    <asp:Label ID="slavesLabel" runat="server" />
      <br />
      <strong>Groups:</strong>
    <asp:Label ID="groupsLabel" runat="server" />
      <br />
      <br />
      <strong>Number of Calculating Tasks by User:</strong><asp:Table ID="calculatingTasksByUserTable" runat="server" GridLines="Both">
          <asp:TableRow runat="server" BackColor="#CCCCCC">
              <asp:TableCell runat="server" Font-Bold="False">User</asp:TableCell>
              <asp:TableCell runat="server" Font-Bold="False">Nr. of Tasks</asp:TableCell>
          </asp:TableRow>
      </asp:Table>
      <br />
      <strong>Number of Waiting Tasks by User:</strong><asp:Table ID="waitingTasksByUserTable" runat="server" GridLines="Both">
          <asp:TableRow runat="server" BackColor="#CCCCCC">
              <asp:TableCell runat="server" Font-Bold="False">User</asp:TableCell>
              <asp:TableCell runat="server" Font-Bold="False">Nr. of Tasks</asp:TableCell>
          </asp:TableRow>
      </asp:Table>
      <br />
    <br />
      <strong>Days:</strong>
    <asp:DropDownList ID="daysDropDownList" runat="server" AutoPostBack="True">
      <asp:ListItem Value="1"></asp:ListItem>
      <asp:ListItem Value="2"></asp:ListItem>
      <asp:ListItem Value="3"></asp:ListItem>
      <asp:ListItem Value="4"></asp:ListItem>
      <asp:ListItem Value="5"></asp:ListItem>
      <asp:ListItem Value="6"></asp:ListItem>
      <asp:ListItem Value="7"></asp:ListItem>
      <asp:ListItem Value="8"></asp:ListItem>
      <asp:ListItem Value="9"></asp:ListItem>
      <asp:ListItem Value="10"></asp:ListItem>
      <asp:ListItem Value="11"></asp:ListItem>
      <asp:ListItem Value="12"></asp:ListItem>
      <asp:ListItem Value="13"></asp:ListItem>
      <asp:ListItem Value="14"></asp:ListItem>
      <asp:ListItem Value="All"></asp:ListItem>
    </asp:DropDownList>
    <br />
    <br />
      <strong>Avg. CPU Utilization History of all Slaves</strong><br />
    <asp:Chart ID="cpuUtilizationChart" runat="server" Height="270px" Width="1280px">
      <Series>
        <asp:Series BorderWidth="2" ChartType="Line" Color="0, 176, 80" Name="Series1" XValueType="DateTime"
          YValueType="Double">
        </asp:Series>
      </Series>
      <ChartAreas>
        <asp:ChartArea BackColor="Black" BackHatchStyle="DottedGrid" BackSecondaryColor="0, 96, 43"
          BorderColor="DarkGreen" BorderDashStyle="Dot" Name="ChartArea1">
          <AxisY>
            <MajorGrid Enabled="False" />
          </AxisY>
          <AxisX IntervalAutoMode="VariableCount" IntervalOffset="1" IntervalOffsetType="Hours"
            IntervalType="Hours" IsLabelAutoFit="False" >
            <MajorGrid Enabled="False" />
            <LabelStyle Format="d/M/yyyy HH:mm" IsStaggered="True" />
          </AxisX>
        </asp:ChartArea>
      </ChartAreas>
    </asp:Chart>
    <br />
      <strong>Cores/Used Cores History</strong><br />
    <asp:Chart ID="coresChart" runat="server" Palette="None" Width="1280px" PaletteCustomColors="137, 165, 78; 185, 205, 150">
      <Series>
        <asp:Series ChartType="Area" Name="Cores" XValueType="DateTime" YValueType="Double">
        </asp:Series>
        <asp:Series ChartArea="ChartArea1" ChartType="Area" Name="FreeCores" XValueType="DateTime"
          YValueType="Double">
        </asp:Series>
      </Series>
      <ChartAreas>
        <asp:ChartArea BackColor="Black" BackHatchStyle="DottedGrid" BackSecondaryColor="0, 96, 43"
          BorderColor="DarkGreen" BorderDashStyle="Dot" Name="ChartArea1">
          <AxisY>
            <MajorGrid Enabled="False" />
          </AxisY>
          <AxisX IntervalAutoMode="VariableCount" IntervalOffset="1" IntervalOffsetType="Hours"
            IntervalType="Hours" IsLabelAutoFit="False" >
            <MajorGrid Enabled="False" />
            <LabelStyle Format="d/M/yyyy HH:mm" IsStaggered="True" />
          </AxisX>
        </asp:ChartArea>
      </ChartAreas>
    </asp:Chart>
    <br />
      <strong>Memory/Used Memory History (GB)</strong><br />
    <asp:Chart ID="memoryChart" runat="server" Palette="None" PaletteCustomColors="170, 70, 67; 209, 147, 146"
      Width="1280px">
      <Series>
        <asp:Series ChartType="Area" Name="Cores" XValueType="DateTime" YValueType="Double">
        </asp:Series>
        <asp:Series ChartArea="ChartArea1" ChartType="Area" Name="FreeCores" XValueType="DateTime"
          YValueType="Double">
        </asp:Series>
      </Series>
      <ChartAreas>
        <asp:ChartArea BackColor="Black" BackHatchStyle="DottedGrid" BackSecondaryColor="0, 96, 43"
          BorderColor="DarkGreen" BorderDashStyle="Dot" Name="ChartArea1">
          <AxisY>
            <MajorGrid Enabled="False" />
          </AxisY>
          <AxisX IntervalAutoMode="VariableCount" IntervalOffset="1" IntervalOffsetType="Hours"
            IntervalType="Hours" IsLabelAutoFit="False" >
            <MajorGrid Enabled="False" />
            <LabelStyle Format="d/M/yyyy HH:mm" IsStaggered="True" />
          </AxisX>
        </asp:ChartArea>
      </ChartAreas>
    </asp:Chart>
      <br />
  </div>
  </form>
</body>
</html>
