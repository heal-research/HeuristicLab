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

// copied from http://www.codeproject.com/KB/edit/IPAddressTextBox.aspx

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace HeuristicLab.Hive.Client.Console {
  [
  DefaultProperty("Text"),
  DefaultEvent("TextChanged"),
  ]
  /// <summary>
  /// IPAddressTextBox
  /// Control to enter IP-Addresses manually
  /// Supports Binary and Decimal Notation
  /// Supports input of CIDR Notation (appending of Bitmask of Subnetmask devided by Slash)
  /// Support input of IP-Address in IPv6 format
  /// </summary>
  public class IPAddressTextBox : System.Windows.Forms.TextBox {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    private IPNotation ipNotation = IPNotation.IPv4Decimal;
    private IPNotation newIPNotation = IPNotation.IPv4Decimal;
    private bool bOverwrite = true;
    private bool bPreventLeave = true;
    private System.Windows.Forms.ErrorProvider error;
    private Regex regexValidNumbers = new Regex("[0-9]");
    private ArrayList arlDelimeter = new ArrayList(new char[] { '.' });


    public enum IPNotation {
      IPv4Decimal,			//192.168.000.001
      IPv4Binary,				//11000000.10101000.00000000.00000001
      IPv4DecimalCIDR,		//192.168.000.001/16
      IPv4BinaryCIDR,			//11000000.10101000.00000000.00000001/16
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public IPAddressTextBox()
      : base() {
      this.InitializeComponent();

      this.ResetText();
    }

    private void InitializeComponent() {
      this.error = new System.Windows.Forms.ErrorProvider();
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null)
          components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Properties
    #region Not allowed Properties from BaseClass
    /// <summary>
    /// Multiline is not allowed
    /// </summary>
    [
    Category("Behavior"),
    Description("Multiline is not allowed"),
    DefaultValue(false),
    Browsable(false)
    ]
    public override bool Multiline {
      get {
        return base.Multiline;
      }
      set {
        //base.Multiline = value;
        base.Multiline = false;
      }
    }
    [
    Category("Behavior"),
    Description("AllowDrop is not allowed"),
    DefaultValue(false),
    Browsable(false)
    ]
    public override bool AllowDrop {
      get {
        return base.AllowDrop;
      }
      set {
        //base.AllowDrop = value;
        base.AllowDrop = false;
      }
    }
    [
    Category("Behavior"),
    Description("AcceptsReturn is not allowed"),
    DefaultValue(false),
    Browsable(false)
    ]
    public new bool AcceptsReturn {
      get {
        return base.AcceptsReturn;
      }
      set {
        //base.AcceptsReturn = value;
        base.AcceptsReturn = false;
      }
    }
    [
    Category("Behavior"),
    Description("AcceptsTab is not allowed"),
    DefaultValue(false),
    Browsable(false)
    ]
    public new bool AcceptsTab {
      get {
        return base.AcceptsTab;
      }
      set {
        //base.AcceptTab = value;
        base.AcceptsTab = false;
      }
    }
    [
    Category("Behavior"),
    Description("CharacterCasing is not allowed"),
    DefaultValue(CharacterCasing.Normal),
    Browsable(false)
    ]
    public new CharacterCasing CharacterCasing {
      get {
        return base.CharacterCasing;
      }
      set {
        //base.CharacterCasing = value;
        base.CharacterCasing = CharacterCasing.Normal;
      }
    }
    [
    Category("Behavior"),
    Description("WordWrap is not allowed"),
    DefaultValue(true),
    Browsable(false)
    ]
    public new bool WordWrap {
      get {
        return base.WordWrap;
      }
      set {
        //base.WordWrap = value;
        base.WordWrap = true;
      }
    }

    /// <summary>
    /// Maxlength must not be changed by user
    /// </summary>
    [
    Category("Behavior"),
    Description("Specifies maximum length of a String. Change is not allowed"),
    DefaultValue(15),
    Browsable(false)
    ]
    public override int MaxLength {
      get {
        return base.MaxLength;
      }
      set {
        //base.MaxLength = value;
        base.MaxLength = this.Text.Length;
      }
    }

    #endregion // Not allowed Properties from BaseClass

    /// <summary>
    /// Specifies if IP-Address
    /// </summary>
    [
    Category("Appearance"),
    Description("Specifies the IP-Address"),
    DefaultValue("   .   .   .   ")
    ]
    public override string Text {
      get {
        return base.Text;
      }
      set {
        try {
          if (IPAddressTextBox.ValidateIP(value, this.newIPNotation, this.arlDelimeter))
            base.Text = IPAddressTextBox.MakeValidSpaces(value, this.newIPNotation, this.arlDelimeter);
        }
        catch {

        }
      }
    }

    /// <summary>
    /// Specifies if Numbers should be overwritten
    /// </summary>
    [
    Category("Behavior"),
    Description("Specifies if Numbers should be overwritten"),
    DefaultValue(true)
    ]
    public bool OverWriteMode {
      get {
        return this.bOverwrite;
      }
      set {
        if (value != this.bOverwrite) {
          this.bOverwrite = value;
          this.OnOverWriteChanged(value);
        }
      }
    }

    /// <summary>
    /// Prevents leaving of Control if there is an input-error
    /// </summary>
    [
    Category("Behavior"),
    Description("Prevents leaving of Control if there is an input-error"),
    DefaultValue(true)
    ]
    public bool PreventLeaveAtError {
      get {
        return this.bPreventLeave;
      }
      set {
        if (value != this.bPreventLeave) {
          this.bPreventLeave = value;
          this.OnPreventLeaveChanged(value);
        }
      }
    }

    /// <summary>
    /// Specifies if IP-Address Notation (IPv4, IPv6, Binary, Decimal, CIDR
    /// </summary>
    [
    Category("Appearance"),
    Description("Specifies if IP-Address Notation (IPv4, IPv6, Binary, Decimal, CIDR"),
    DefaultValue(IPNotation.IPv4Decimal)
    ]
    public IPNotation Notation {
      get {
        return this.ipNotation;
      }
      set {
        if (value != this.ipNotation) {
          try {
            this.newIPNotation = value;
            this.ChangeNotation(this.ipNotation, this.newIPNotation);
            this.ipNotation = this.newIPNotation;
            this.OnNotationChanged(this.newIPNotation);
          }
          catch (Exception LastError) {
            System.Diagnostics.Debug.WriteLine(LastError.Message);
            throw LastError;
          }
        }
      }
    }

    /// <summary>
    /// Specifies the Errorprovider that appears at invalid IPs
    /// </summary>
    [
    Category("Appearance"),
    Description("Specifies the Errorprovider that appears at invalid IPs"),
    DefaultValue(false)
    ]
    public ErrorProvider IPError {
      get {
        return this.error;
      }
    }


    #endregion //Properties

    #region Eventhandling

    /// <summary>
    /// Delegate for Notation-Events
    /// </summary>
    public delegate void NotationChangedEventHandler(IPNotation arg_newValue);
    /// <summary>
    /// Event called if AppearanceMode Notation is changed
    /// </summary>
    public event NotationChangedEventHandler NotationChanged;
    /// <summary>
    /// Delegate for Bool-Properties-Events
    /// </summary>
    public delegate void BoolPropertyChangedEventHandler(bool arg_bNewValue);
    /// <summary>
    /// Event called if BehaviorMode OverWriteMode is changed
    /// </summary>
    public event BoolPropertyChangedEventHandler OverWriteModeChanged;
    /// <summary>
    /// Event called if BehaviorMode PreventLeave is changed
    /// </summary>
    public event BoolPropertyChangedEventHandler PreventLeaveChanged;

    /// <summary>
    /// Occures when Appearance-Mode Notation was changed
    /// </summary>
    /// <param name="arg_Value">Value, Input IP-Address  notation</param>
    protected virtual void OnNotationChanged(IPNotation arg_Value) {
      if (this.NotationChanged != null)
        this.NotationChanged(arg_Value);
    }

    private void ChangeNotation(IPNotation arg_oldValue, IPNotation arg_newValue) {
      string sTo = "";
      ArrayList arlFrom = new ArrayList(this.Text.Replace(" ", "").Split((char[])this.arlDelimeter.ToArray(typeof(char))));

      switch (arg_newValue) {
        case IPNotation.IPv4Decimal:
          this.regexValidNumbers = new Regex("[0-9]");
          this.arlDelimeter = new ArrayList(new char[] { '.' });
          break;
        case IPNotation.IPv4DecimalCIDR:
          this.regexValidNumbers = new Regex("[0-9]");
          this.arlDelimeter = new ArrayList(new char[] { '.', '/' });
          break;
        case IPNotation.IPv4Binary:
          this.regexValidNumbers = new Regex("[01]");
          this.arlDelimeter = new ArrayList(new char[] { '.' });
          break;
        case IPNotation.IPv4BinaryCIDR:
          this.regexValidNumbers = new Regex("[01]");
          this.arlDelimeter = new ArrayList(new char[] { '.', '/' });
          break;
        default:
          break;
      }

      switch (arg_oldValue) {
        case IPNotation.IPv4Decimal:
          switch (arg_newValue) {
            case IPNotation.IPv4Decimal:
              break;
            case IPNotation.IPv4DecimalCIDR:
              for (int i = 0; i < arlFrom.Count; i++) {
                sTo += arlFrom[i].ToString() +
                  //Add Slash if its the last IPPart, els add a dot
                  (i == arlFrom.Count - 1 ? "/    " : ".");
              }
              break;
            case IPNotation.IPv4Binary:
              for (int i = 0; i < arlFrom.Count; i++) {
                //Convert Decimal to Binary
                sTo += this.Dec2Bin(arlFrom[i].ToString()) +
                  //Add Dot if its not the last IPPart, else add nothing
                  (i == arlFrom.Count - 1 ? "" : ".");
              }
              break;
            case IPNotation.IPv4BinaryCIDR:
              for (int i = 0; i < arlFrom.Count; i++) {
                //Convert Decimal to Binary
                sTo += this.Dec2Bin(arlFrom[i].ToString()) +
                  //Add Slash if its the last IPPart, else add a dot
                  (i == arlFrom.Count - 1 ? "/    " : ".");
              }
              break;
            default:
              break;
          }
          break;
        case IPNotation.IPv4DecimalCIDR:
          switch (arg_newValue) {
            case IPNotation.IPv4Decimal:
              //do not use the last Item, its the Subnetmask
              for (int i = 0; i < arlFrom.Count - 1; i++) {
                sTo += arlFrom[i].ToString() +
                  //Add Dot if its not the last IPPart, else add nothing
                  (i == arlFrom.Count - 2 ? "" : ".");
              }
              break;
            case IPNotation.IPv4DecimalCIDR:
              break;
            case IPNotation.IPv4Binary:
              //do not use the last Item, its the Subnetmask
              for (int i = 0; i < arlFrom.Count - 1; i++) {
                //Convert Decimal to Binary
                sTo += this.Dec2Bin(arlFrom[i].ToString()) +
                  //Add Dot if its not the last IPPart, else add nothing
                  (i == arlFrom.Count - 2 ? "" : ".");
              }
              break;
            case IPNotation.IPv4BinaryCIDR:
              //do not use the last Item, its the Subnetmask
              for (int i = 0; i < arlFrom.Count - 1; i++) {
                //Convert Decimal to Binary
                sTo += this.Dec2Bin(arlFrom[i].ToString()) +
                  //Add Dot if its not the last IPPart, else add nothing
                  (i == arlFrom.Count - 2 ? "" : ".");
              }
              //Add Subnetmask
              sTo += "/" + arlFrom[arlFrom.Count - 1];
              break;
            default:
              break;
          }
          break;
        case IPNotation.IPv4Binary:
          switch (arg_newValue) {
            case IPNotation.IPv4Decimal:
              for (int i = 0; i < arlFrom.Count; i++) {
                //Convert Binary to Decimal
                sTo += this.Bin2Dec(arlFrom[i].ToString()) +
                  //Add Dot if its not the last IPPart, else add nothing
                  (i == arlFrom.Count - 1 ? "" : ".");
              }
              break;
            case IPNotation.IPv4DecimalCIDR:
              for (int i = 0; i < arlFrom.Count; i++) {
                //Convert Binary to Decimal
                sTo += this.Bin2Dec(arlFrom[i].ToString()) +
                  //Add Slash if its the last IPPart, els add a dot
                  (i == arlFrom.Count - 1 ? "/    " : ".");
              }
              break;
            case IPNotation.IPv4Binary:
              break;
            case IPNotation.IPv4BinaryCIDR:
              for (int i = 0; i < arlFrom.Count; i++) {
                sTo += arlFrom[i].ToString() +
                  //Add Slash if its the last IPPart, else add a dot
                  (i == arlFrom.Count - 1 ? "/    " : ".");
              }
              break;
            default:
              break;
          }
          break;
        case IPNotation.IPv4BinaryCIDR:
          switch (arg_newValue) {
            case IPNotation.IPv4Decimal:
              //do not use the last Item, its the Subnetmask
              for (int i = 0; i < arlFrom.Count - 1; i++) {
                sTo += this.Bin2Dec(arlFrom[i].ToString()) +
                  //Add Dot if its not the last IPPart, else add nothing
                  (i == arlFrom.Count - 2 ? "" : ".");
              }
              break;
            case IPNotation.IPv4DecimalCIDR:
              //do not use the last Item, its the Subnetmask
              for (int i = 0; i < arlFrom.Count - 1; i++) {
                sTo += this.Bin2Dec(arlFrom[i].ToString()) +
                  //Add Dot if its not the last IPPart, else add nothing
                  (i == arlFrom.Count - 2 ? "" : ".");
              }
              //Add Subnetmask
              sTo += "/" + arlFrom[arlFrom.Count - 1];
              break;
            case IPNotation.IPv4Binary:
              //do not use the last Item, its the Subnetmask
              for (int i = 0; i < arlFrom.Count - 1; i++) {
                sTo += arlFrom[i].ToString() +
                  //Add Dot if its not the last IPPart, else add nothing
                  (i == arlFrom.Count - 2 ? "" : ".");
              }
              break;
            case IPNotation.IPv4BinaryCIDR:
              break;
            default:
              break;
          }
          break;

      }

      this.Text = sTo;
      this.MaxLength = this.TextLength;
    }

    /// <summary>
    /// Occures when Behaviour-Mode OverWriteMode was changed
    /// </summary>
    /// <param name="arg_bValue">Value, Overwrite Numbers in Editfield or not</param>
    protected virtual void OnOverWriteChanged(bool arg_bValue) {
      if (this.OverWriteModeChanged != null)
        this.OverWriteModeChanged(arg_bValue);
    }

    /// <summary>
    /// Occures when Behaviour-Mode PreventLeave was changed
    /// </summary>
    /// <param name="arg_bValue">Value, leave control if there is an error or not</param>
    protected virtual void OnPreventLeaveChanged(bool arg_bValue) {
      if (this.PreventLeaveChanged != null)
        this.PreventLeaveChanged(arg_bValue);
    }

    #endregion //events

    #region Event-Overrides

    /// <summary>
    /// Override standard KeyDownEventHandler
    /// Catches Inputs of "." and "/" to jump to next positions
    /// </summary>
    /// <param name="e">KeyEventArgument</param>
    protected override void OnKeyDown(KeyEventArgs e) {
      //Zeichen an die richtige stelle schreiben
      int iPos = this.SelectionStart;
      char[] cText = this.Text.ToCharArray();

      if (e.Modifiers == Keys.None) {
        if ((char.IsLetterOrDigit(Convert.ToChar(e.KeyValue)) || e.KeyCode == Keys.NumPad0)//Numpad0=96 --> `
          && iPos < this.TextLength) {
          if (this.arlDelimeter.Contains(cText[iPos]))
            iPos += 1;
          this.SelectionStart = iPos;
          if (this.OverWriteMode) {
            if (iPos < this.TextLength)
              this.SelectionLength = 1;
          } else {
            if (iPos < this.TextLength)
              if (cText[iPos] == ' ')
                this.SelectionLength = 1;
          }
        }
      }
      base.OnKeyDown(e);
    }

    /// <summary>
    /// Override standard KeyUpEventHandler
    /// Catches Inputs of "." and "/" to jump to next positions
    /// </summary>
    /// <param name="e">KeyEventArgument</param>
    protected override void OnKeyUp(KeyEventArgs e) {
      //Zeichen an die richtige stelle schreiben
      int iPos = this.SelectionStart;
      char[] cText = this.Text.ToCharArray();

      //Cursor hintern Punkt setzen
      if ((char.IsLetterOrDigit(Convert.ToChar(e.KeyValue)) || e.KeyCode == Keys.NumPad0)//Numpad0=96 --> `
        && iPos < this.TextLength) {
        if (this.arlDelimeter.Contains(cText[iPos]))
          iPos += 1;

        this.SelectionStart = iPos;
      }
      base.OnKeyUp(e);
    }


    /// <summary>
    /// Override standard KeyPressEventHandler
    /// Catches Inputs of "." and "/" to jump to next positions
    /// </summary>
    /// <param name="e">KeyPressEventArgument</param>
    protected override void OnKeyPress(KeyPressEventArgs e) {
      //valid input charachters
      if (char.IsControl(e.KeyChar) ||
        regexValidNumbers.IsMatch(e.KeyChar.ToString())) {
        e.Handled = false;
      } else {
        switch (e.KeyChar) {
          case '/':
            this.JumpToSlash();
            break;
          case '.':
          case ':':
            this.JumpToNextDot();
            break;
          default:
            break;
        }
        e.Handled = true;
      }
      base.OnKeyPress(e);
    }

    /// <summary>
    /// Override standard TextChangedEventHandler
    /// Looks if inserted IP-Address is valid
    /// </summary>
    /// <param name="e">EventArgument</param>
    protected override void OnTextChanged(EventArgs e) {
      base.OnTextChanged(e);
      if (this.Text.Length == 0)
        this.ResetText();

      try {
        if (!this.ValidateIP())
          this.error.SetError(this, "Invalid IP-address");
        else
          this.error.SetError(this, "");
      }
      catch (Exception LastError) {
        this.error.SetError(this, LastError.Message);
      }
    }

    /// <summary>
    /// Override standard ValidatingEventHandler
    /// Validates inserted IP-Address, and cancels Textbox if valid or PreventLeave=false
    /// </summary>
    /// <param name="e">CancelEventArgument</param>
    protected override void OnValidating(CancelEventArgs e) {
      //e.Cancel = true;//suppress cancel-signal = not validated
      e.Cancel = (!this.ValidateIP() && this.bPreventLeave);
      base.OnValidating(e);
    }

    #endregion	//Eventhandling

    #region Methods

    /// <summary>
    /// Override standard ResetText
    /// Fills Textbox with Dots and Slashes dependend on Properties
    /// </summary>
    public override void ResetText() {
      base.ResetText();
      switch (this.Notation) {
        case IPNotation.IPv4Decimal:
          this.Text = "   .   .   .   ";
          break;
        case IPNotation.IPv4DecimalCIDR:
          this.Text = "   .   .   .   /    ";
          break;
        case IPNotation.IPv4Binary:
          this.Text = "        .        .        .        ";
          break;
        case IPNotation.IPv4BinaryCIDR:
          this.Text = "        .        .        .        /    ";
          break;
        default:
          break;
      }

      this.MaxLength = this.TextLength;
    }


    /// <summary>
    /// Window-Message Constant
    /// </summary>
    protected const int WM_KEYDOWN = 0x0100;

    /// <summary>
    /// Override standard PreProcessMessge
    /// Catches Inputs of Backspaces and Deletes to remove IP-Digits at the right position
    /// </summary>
    /// <param name="msg">Process Message</param>
    public override bool PreProcessMessage(ref Message msg) {
      if (msg.Msg == WM_KEYDOWN) {
        Keys keyData = ((Keys)(int)msg.WParam) | ModifierKeys;
        Keys keyCode = ((Keys)(int)msg.WParam);

        int iPos = this.SelectionStart;
        char[] cText = this.Text.ToCharArray();
        switch (keyCode) {
          case Keys.Delete:
            if (iPos < this.TextLength) {
              while (cText[iPos] == '.' || cText[iPos] == ':' || cText[iPos] == '/') {
                if ((iPos += 1) >= cText.Length)
                  break;
              }
              if (iPos < this.TextLength) {
                base.Text = this.Text.Substring(0, iPos) + " " + this.Text.Substring(iPos + 1);
                this.SelectionStart = iPos + 1;
              } else
                this.SelectionStart = this.TextLength - 1;
            }
            return true;
          case Keys.Back:
            if (iPos > 0) {
              while (cText[iPos - 1] == '.' || cText[iPos - 1] == ':' || cText[iPos - 1] == '/') {
                if ((iPos -= 1) <= 0)
                  break;
              }
              if (iPos > 0) {
                base.Text = this.Text.Substring(0, iPos - 1) + " " + this.Text.Substring(iPos);
                this.SelectionStart = iPos - 1;
              } else
                this.SelectionStart = 0;
            }
            return true;
          default:
            break;
        }
      }
      return base.PreProcessMessage(ref msg);
    }

    /// <summary>
    /// Returns the formatted IP-Addresses without spaces and Zeroes
    /// </summary>
    /// <returns>IP-Address without spaces and Zeroes</returns>
    public string GetPureIPAddress() {
      string s = "";
      ArrayList arlIP = new ArrayList(this.Text.Replace(" ", "").Split((char[])this.arlDelimeter.ToArray(typeof(char))));
      for (int i = 0; i < arlIP.Count; i++) {
        while (arlIP[i].ToString().StartsWith("0"))
          arlIP[i] = arlIP[i].ToString().Substring(1);
      }
      s = IPAddressTextBox.MakeIP((string[])arlIP.ToArray(typeof(string)), this.ipNotation);
      return s;
    }

    #endregion //Methods

    #region Helperfunctions

    /// <summary>
    /// Sets Inputcursor to Subnet-Slash
    /// </summary>
    private void JumpToSlash() {
      int iSelStart = this.Text.LastIndexOf("/");
      if (iSelStart >= 0) {
        this.Select(iSelStart + 1, 0);
      }
    }

    /// <summary>
    /// Sets input cursour to next Dot
    /// </summary>
    private void JumpToNextDot() {
      int iSelStart = this.Text.IndexOf('.', this.SelectionStart);
      if (iSelStart >= 0) {
        this.Select(iSelStart + 1, 0);
      } else {
        iSelStart = this.Text.IndexOf(':', this.SelectionStart);
        if (iSelStart >= 0) {
          this.Select(iSelStart + 1, 0);
        }
      }
    }

    /// <summary>
    /// Converts Decimal IP-Part to Binary (default IPv6 = false)
    /// </summary>
    /// <param name="arg_sDec">Decimal IP-Part</param>
    /// <param name="arg_bIPv6">Binary for IPv6 (has 16 digits)</param>
    /// <returns>Binary IP-Part</returns>
    private string Dec2Bin(string arg_sDec) {
      return this.Dec2Bin(arg_sDec, false);
    }
    /// <summary>
    /// Converts Decimal IP-Part to Binary
    /// </summary>
    /// <param name="arg_sDec">Decimal IP-Part</param>
    /// <param name="arg_bIPv6">Binary for IPv6 (has 16 digits)</param>
    /// <returns>Binary IP-Part</returns>
    private string Dec2Bin(string arg_sDec, bool arg_bIPv6) {
      string sBin = (arg_bIPv6 ? "0000000000000000" : "00000000"), sSubnet = "";
      arg_sDec = arg_sDec.Trim();
      while (arg_sDec.Length < 3)
        arg_sDec = "0" + arg_sDec;
      if (arg_sDec.IndexOf("/") >= 0) {
        sSubnet = arg_sDec.Substring(arg_sDec.IndexOf("/"));
        arg_sDec = arg_sDec.Substring(0, arg_sDec.IndexOf("/"));
      }
      int iDec = Convert.ToInt32(arg_sDec, 10);
      sBin = Convert.ToString(iDec, 2);
      while (sBin.Length < (arg_bIPv6 ? 16 : 8))
        sBin = "0" + sBin;
      return sBin + sSubnet;
    }

    /// <summary>
    /// Converts Binary IP-Part to Decimal
    /// </summary>
    /// <param name="arg_sBin">Binary IP-Part</param>
    /// <returns>Decimal IP-Part</returns>
    private string Bin2Dec(string arg_sBin) {
      string sDec = "000", sSubnet = "";
      arg_sBin = arg_sBin.Trim();
      while (arg_sBin.Length < 8)
        arg_sBin = "0" + arg_sBin;
      if (arg_sBin.IndexOf("/") >= 0) {
        sSubnet = arg_sBin.Substring(arg_sBin.IndexOf("/"));
        arg_sBin = arg_sBin.Substring(0, arg_sBin.IndexOf("/"));
      }
      int iBin = Convert.ToInt32(arg_sBin, 2);
      if (iBin > 255)
        throw new Exception(string.Format("Can't convert Binary to Decimal IP-Address\nbin:{0} is greater than 255", iBin));
      sDec = Convert.ToString(iBin, 10);
      while (sDec.Length < 3)
        sDec = "0" + sDec;
      return sDec + sSubnet;
    }

    /// <summary>
    /// Converts Binary IP-Part to Hexadecimal
    /// </summary>
    /// <param name="arg_sBin">Binary IP-Part</param>
    /// <returns>Hexadecimal IP-Part</returns>
    private string Bin2Hex(string arg_sBin) {
      string sHex = "0000", sSubnet = "";
      arg_sBin = arg_sBin.Trim();
      while (arg_sBin.Length < 8)
        arg_sBin = "0" + arg_sBin;
      if (arg_sBin.IndexOf("/") >= 0) {
        sSubnet = arg_sBin.Substring(arg_sBin.IndexOf("/"));
        arg_sBin = arg_sBin.Substring(0, arg_sBin.IndexOf("/"));
      }
      int iBin = Convert.ToInt32(arg_sBin, 2);
      sHex = Convert.ToString(iBin, 16);
      while (sHex.Length < 4)
        sHex = "0" + sHex;
      return sHex + sSubnet;
    }

    /// <summary>
    /// Converts Hexadecimal IP-Part to Binary (default IPv6=true)
    /// </summary>
    /// <param name="arg_sHex">Hexadecimal IP-Part</param>
    /// <returns>Binary IP-Part</returns>
    private string Hex2Bin(string arg_sHex) {
      return this.Hex2Bin(arg_sHex, true);
    }
    /// <summary>
    /// Converts Hexadecimal IP-Part to Binary
    /// </summary>
    /// <param name="arg_sHex">Hexadecimal IP-Part</param>
    /// <param name="arg_bIPv6">Binary for IPv6 (16 digits)</param>
    /// <returns>Binary IP-Part</returns>
    private string Hex2Bin(string arg_sHex, bool arg_bIPv6) {
      string sBin = (arg_bIPv6 ? "0000000000000000" : "00000000"), sSubnet = "";
      arg_sHex = arg_sHex.Trim();
      while (arg_sHex.Length < 3)
        arg_sHex = "0" + arg_sHex;
      if (arg_sHex.IndexOf("/") >= 0) {
        sSubnet = arg_sHex.Substring(arg_sHex.IndexOf("/"));
        arg_sHex = arg_sHex.Substring(0, arg_sHex.IndexOf("/"));
      }
      int iHex = Convert.ToInt32(arg_sHex, 16);
      if (iHex > 255 && !arg_bIPv6)
        throw new Exception(string.Format("Can't convert Hexadecimal to Binary IP-Address\nhex:{0} is greater than 11111111", iHex));
      sBin = Convert.ToString(iHex, 2);
      while (sBin.Length < (arg_bIPv6 ? 16 : 8))
        sBin = "0" + sBin;
      return sBin + sSubnet;
    }

    /// <summary>
    /// Converts Decimal IP-Part to Hexadecimal
    /// </summary>
    /// <param name="arg_sDec">Decimal IP-Part</param>
    /// <returns>Hexadecimal IP-Part</returns>
    private string Dec2Hex(string arg_sDec) {
      string sHex = "0000", sSubnet = "";
      arg_sDec = arg_sDec.Trim();
      while (arg_sDec.Length < 8)
        arg_sDec = "0" + arg_sDec;
      if (arg_sDec.IndexOf("/") >= 0) {
        sSubnet = arg_sDec.Substring(arg_sDec.IndexOf("/"));
        arg_sDec = arg_sDec.Substring(0, arg_sDec.IndexOf("/"));
      }
      int iDec = Convert.ToInt32(arg_sDec, 10);
      sHex = Convert.ToString(iDec, 16);
      while (sHex.Length < 4)
        sHex = "0" + sHex;
      return sHex + sSubnet;
    }

    /// <summary>
    /// Converts Hexadecimal IP-Part to Decimal
    /// </summary>
    /// <param name="arg_sHex">Hexadecimal IP-Part</param>
    /// <returns>Decimal IP-Part</returns>
    private string Hex2Dec(string arg_sHex) {
      string sDec = "000", sSubnet = "";
      arg_sHex = arg_sHex.Trim();
      while (arg_sHex.Length < 8)
        arg_sHex = "0" + arg_sHex;
      if (arg_sHex.IndexOf("/") >= 0) {
        sSubnet = arg_sHex.Substring(arg_sHex.IndexOf("/"));
        arg_sHex = arg_sHex.Substring(0, arg_sHex.IndexOf("/"));
      }
      int iHex = Convert.ToInt32(arg_sHex, 16);
      if (iHex > 255)
        throw new Exception(string.Format("Can't convert Hexadecimal to Decimal IP-Address\nhex:{0} is greater than 255", iHex));
      sDec = Convert.ToString(iHex, 10);
      while (sDec.Length < 3)
        sDec = "0" + sDec;
      return sDec + sSubnet;
    }

    /// <summary>
    /// Checks if IP in Textfield is valid
    /// </summary>
    /// <returns>true/false valid/not</returns>
    private bool ValidateIP() {
      if (IPAddressTextBox.ValidateIP(this.Text, this.newIPNotation, this.arlDelimeter))
        return true;
      else
        //if Control is not visible or enabled, it doesn't matter if IP is valid
        return this.Enabled || this.Visible ? false : true;
    }

    /// <summary>
    /// Checks if the given String is an valid ip-address
    /// </summary>
    /// <param name="arg_sIP">IP-String</param>
    /// <param name="arg_ipNotation">IP-notation</param>
    /// <param name="arg_arlDelimeter">Delimeter to parse IPString</param>
    /// <returns>true/false validated/not</returns>
    protected static bool ValidateIP(string arg_sIP, IPNotation arg_ipNotation, ArrayList arg_arlDelimeter) {
      bool bValidated = false;
      ArrayList arlIP = new ArrayList(arg_sIP.Split((char[])arg_arlDelimeter.ToArray(typeof(char))));

      try {
        switch (arg_ipNotation) {
          case IPNotation.IPv4Decimal:
          case IPNotation.IPv4Binary:
            bValidated = arlIP.Count == 4;
            break;
          case IPNotation.IPv4DecimalCIDR:
          case IPNotation.IPv4BinaryCIDR:
            bValidated = arlIP.Count == 5;
            break;
          default:
            break;
        }
        if (!bValidated) {
          throw new Exception("IP-Address has wrong element count");
        }

        //don't check the 1st 2 elemnt if its IPv4 in IPv6-notation
        for (int i = (arg_ipNotation.ToString().IndexOf("IPv6IPv4") == 0 ? 2 : 0);
          //don't check the subnet element
          i < (arg_ipNotation.ToString().IndexOf("CIDR") > 0 ? arlIP.Count - 1 : arlIP.Count);
          i++) {
          string sIPPart = arlIP[i].ToString().Replace(" ", "");
          int iIPPart = 0;
          switch (arg_ipNotation) {
            case IPNotation.IPv4Decimal:
            case IPNotation.IPv4DecimalCIDR:
              while (sIPPart.Length < 3)
                sIPPart = "0" + sIPPart;
              iIPPart = Convert.ToInt32(sIPPart, 10);
              if (iIPPart < 256)
                bValidated = true;
              else
                bValidated = false;
              break;
            case IPNotation.IPv4Binary:
            case IPNotation.IPv4BinaryCIDR:
              while (sIPPart.Length < 8)
                sIPPart = "0" + sIPPart;
              iIPPart = Convert.ToInt32(sIPPart, 2);
              if (iIPPart < 256)
                bValidated = true;
              else
                bValidated = false;
              break;
            default:
              break;
          }
          if (!bValidated) {
            throw new Exception(string.Format("IP-Address element {0}({1}) has wrong format", i, sIPPart));
          }
        }
      }
      catch (Exception LastError) {
        System.Diagnostics.Debug.WriteLine(LastError.Message);
        bValidated = false;
        throw LastError;
      }
      return bValidated;
    }

    /// <summary>
    /// Adds Spaces to given IP-Address, so it fits in the textfield
    /// </summary>
    /// <param name="arg_sIP">IP-String</param>
    /// <param name="arg_ipNotation">IP-notation</param>
    /// <param name="arg_arlDelimeter">Delimeter to parse IPString</param>
    /// <returns>IP-Address with Spaces</returns>
    protected static string MakeValidSpaces(string arg_sIP, IPNotation arg_ipNotation, ArrayList arg_arlDelimeter) {
      ArrayList arlIP = new ArrayList(arg_sIP.Split((char[])arg_arlDelimeter.ToArray(typeof(char))));
      //don't check the 1st 2 elemnt if its IPv4 in IPv6-notation
      for (int i = (arg_ipNotation.ToString().IndexOf("IPv6IPv4") == 0 ? 2 : 0);
        //don't check the subnet element
        i < (arg_ipNotation.ToString().IndexOf("CIDR") > 0 ? arlIP.Count - 1 : arlIP.Count);
        i++) {
        switch (arg_ipNotation) {
          case IPNotation.IPv4Decimal:
          case IPNotation.IPv4DecimalCIDR:
            while (arlIP[i].ToString().Length < 3)
              arlIP[i] = arlIP[i].ToString() + " ";
            break;
          case IPNotation.IPv4Binary:
          case IPNotation.IPv4BinaryCIDR:
            while (arlIP[i].ToString().Length < 8)
              arlIP[i] = arlIP[i].ToString() + " ";
            break;
          default:
            break;
        }
      }

      return IPAddressTextBox.MakeIP((string[])arlIP.ToArray(typeof(string)), arg_ipNotation);
    }

    /// <summary>
    /// Adds Zeroes to given IP-Address, so it fits in the textfield
    /// </summary>
    /// <param name="arg_sIP">IP-String</param>
    /// <param name="arg_ipNotation">IP-notation</param>
    /// <param name="arg_arlDelimeter">Delimeter to parse IPString</param>
    /// <returns>IP-Address with Spaces</returns>
    protected static string MakeValidZeroes(string arg_sIP, IPNotation arg_ipNotation, ArrayList arg_arlDelimeter) {
      ArrayList arlIP = new ArrayList(arg_sIP.Split((char[])arg_arlDelimeter.ToArray(typeof(char))));
      //don't check the 1st 2 elemnt if its IPv4 in IPv6-notation
      for (int i = (arg_ipNotation.ToString().IndexOf("IPv6IPv4") == 0 ? 2 : 0);
        //don't check the subnet element
        i < (arg_ipNotation.ToString().IndexOf("CIDR") > 0 ? arlIP.Count - 1 : arlIP.Count);
        i++) {
        switch (arg_ipNotation) {
          case IPNotation.IPv4Decimal:
          case IPNotation.IPv4DecimalCIDR:
            while (arlIP[i].ToString().Length < 3)
              arlIP[i] = "0" + arlIP[i].ToString();
            break;
          case IPNotation.IPv4Binary:
          case IPNotation.IPv4BinaryCIDR:
            while (arlIP[i].ToString().Length < 8)
              arlIP[i] = "0" + arlIP[i].ToString();
            break;
          default:
            break;
        }
      }

      return IPAddressTextBox.MakeIP((string[])arlIP.ToArray(typeof(string)), arg_ipNotation);
    }

    /// <summary>
    /// Creates IP-Addresstring from given StrignArray and Notation
    /// </summary>
    /// <param name="arg_sIP">String-Array with elements for IP-Address</param>
    /// <param name="arg_ipNotation">Notation of IP-Address</param>
    /// <returns>IPAddress-String</returns>
    protected static string MakeIP(string[] arg_sIP, IPNotation arg_ipNotation) {
      string s = "";
      for (int i = 0; i < arg_sIP.Length; i++) {
        switch (arg_ipNotation) {
          case IPNotation.IPv4Decimal:
          case IPNotation.IPv4Binary:
            s += (arg_sIP[i].Length > 0 ? arg_sIP[i] : "0") + (i < (arg_sIP.Length - 1) ? "." : "");
            break;
          case IPNotation.IPv4DecimalCIDR:
          case IPNotation.IPv4BinaryCIDR:
            s += (arg_sIP[i].Length > 0 ? arg_sIP[i] : "0") + (i < (arg_sIP.Length - 2) ? "." : (i < arg_sIP.Length - 1) ? "/" : "");
            break;
          default:
            break;
        }
      }
      return s;
    }

    #endregion //Helperfunctions
  }
}
