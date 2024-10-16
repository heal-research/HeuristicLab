//DayView2 
//original code based on https://calendar.codeplex.com/
//
//modified by Simon Carter (s1cart3r@gmail.com)
//
//Redistribution and use in source and binary forms are permitted
//provided that the above copyright notice and this paragraph are
//duplicated in all such forms and that any documentation,
//advertising materials, and other materials related to such
//distribution and use acknowledge that the software was developed
//by techcoil.com and Simon Carter.  The name of the
//techcoil.com and Simon Carter may not be used to endorse or promote products derived
//from this software without specific prior written permission.
//THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR
//IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
//
//change history
// 09/06/2013 - initial release 
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Calendar
{
    [Description("DayView2 Calendar Control")]
    public class DayView : Control
    {
        #region Private Members

        private readonly TextBox editbox;
        private readonly VScrollBar scrollBarV;
        private readonly DrawTool drawTool;
        private readonly SelectionTool selectionTool;
        private int allDayEventsHeaderHeight = 20;
        
        private DateTime workStart;
        private DateTime workEnd;
        private AppHeightDrawMode appHeightMode = AppHeightDrawMode.TrueHeightAll;
        private bool showMinutes = true;
        private bool highlightCurrentTime = true;
        private AbstractRenderer renderer;
        private bool ampmdisplay = false;
        private SelectionType selection;
        private DateTime startDate;
        private Appointment _selectedAppointment = null;
        
        private DateTime selectionStart;
        private DateTime selectionEnd;
        private int selectedColumn;
        private int columnCount;

        private int startHour = 8;
        
        private bool minHalfHourApp = false;
        private bool drawAllAppBorder = false;
        private int halfHourHeight = 18;
        private bool _allowAppointmentResize = true;

        private bool selectedAppointmentIsNew;
        private bool allowScroll = true;
        private bool allowInplaceEditing = true;
        private bool allowNew = true;
        private bool _alwaysShowAppointmentText = true;

        private int slotsPerHour = 4;
        private ITool activeTool; 
        
        private int workingMinuteEnd = 30;
        private int workingHourStart = 8;
        private int workingMinuteStart = 30;
        private int workingHourEnd = 18;

        private ContextMenuStrip _ContextMenuHeader;
        private ContextMenuStrip _ContextMenuAllDay;
        private ContextMenuStrip _ContextMenuDiary;

        private int _scrollRows = 4; //scroll 1 hour by default
        private bool _rightMouseSelect = false;
        private DayViewType _dayViewType;

        #region Timer / Tooltip variables
        
        private Point _oldPoint;
        private bool _moveStart;
        private readonly Timer _timerTooltip;
        private readonly ToolTip _tooltip;

        #endregion Timer / Tooltip variables

        #endregion Private Members

        #region Constants

        private const int MAX_APPOINTMENTS_CONFLICTS = 10;

        private const int hourLabelWidth = 50;
        private const int hourLabelIndent = 2;
        private const int dayHeadersHeight = 22;
        private const int horizontalAppointmentHeight = 20;

        internal int appointmentGripWidth = 5;

        #endregion Constants

        #region Enums

        public enum AppHeightDrawMode
        {
            TrueHeightAll = 0, // all appointments have height proportional to true length
            FullHalfHourBlocksAll = 1, // all appts drawn in half hour blocks
            EndHalfHourBlocksAll = 2, // all appts boxes start at actual time, end in half hour blocks
            FullHalfHourBlocksShort = 3, // short (< 30 mins) appts drawn in half hour blocks
            EndHalfHourBlocksShort = 4, // short appts boxes start at actual time, end in half hour blocks
        }

        /// <summary>
        /// Specifies where the mouse is located
        /// </summary>
        public enum MouseLocation
        {
            Header,
            AllDay,
            Diary,
            Times
        }

        /// <summary>
        /// Type of View, Single View is 1 person multiple days, Team View is multiple people single day
        /// </summary>
        public enum DayViewType
        {
            SingleView = 0,
            TeamView = 1
        }

        #endregion Enums

        #region Constructors / Destructors

        public DayView()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);

            ///timer for tooltip
            _timerTooltip = new Timer();
            _timerTooltip.Interval = 2500;
            _timerTooltip.Enabled = false;
            _timerTooltip.Tick += new EventHandler(timerTooltip_Tick);

            //tooltip
            _tooltip = new ToolTip();

            scrollBarV = new VScrollBar();
            scrollBarV.SmallChange = halfHourHeight * _scrollRows;
            scrollBarV.LargeChange = halfHourHeight * _scrollRows;
            scrollBarV.Dock = DockStyle.Right;
            scrollBarV.Visible = allowScroll;
            scrollBarV.Scroll += new ScrollEventHandler(scrollbar_Scroll);
            Controls.Add(scrollBarV);

            AdjustScrollbar();
            scrollBarV.Value = startHour * slotsPerHour * halfHourHeight;

            editbox = new TextBox();
            editbox.Multiline = true;
            editbox.Visible = false;
            editbox.ScrollBars = ScrollBars.Vertical;
            editbox.BorderStyle = BorderStyle.None;
            editbox.KeyUp += new KeyEventHandler(editbox_KeyUp);
            editbox.Margin = Padding.Empty;

            Controls.Add(editbox);

            drawTool = new DrawTool();
            drawTool.DayView = this;

            selectionTool = new SelectionTool();
            selectionTool.DayView = this;
            selectionTool.Complete += new EventHandler(selectionTool_Complete);

            activeTool = drawTool;

            UpdateWorkingHours();

            Renderer = new Office12Renderer(this);
        }

        #endregion Constructors / Destructors

        #region Properties

        #region Design Properties

        [Description("Determines the type of view single (1 person multiple days) team (multiple people single day)")]
        [Category("Design")]
        [DefaultValue(DayViewType.SingleView)]
        public DayViewType ViewType
        {
            get { return _dayViewType; }
            set
            {
                if (_dayViewType != value)
                {
                    _dayViewType = value;
                }
            }
        }

        [DefaultValue(18)]
        [Category("Design")]
        [Description("Determines the half hour height")]
        public int HalfHourHeight
        {
            get
            {
                return halfHourHeight;
            }
            set
            {
                halfHourHeight = value;
                OnHalfHourHeightChanged();
            }
        }

        #endregion Design Properties

        #region Appearance Properties

        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Highlights the current time in the hour label")]
        public bool HighlightCurrentTime
        {
            get
            {
                return highlightCurrentTime;
            }

            set
            {
                highlightCurrentTime = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Determines if the minimum height for an appointment is 1/2 an hour")]
        public bool MinHalfHourApp
        {
            get
            {
                return minHalfHourApp;
            }
            set
            {
                minHalfHourApp = value;
                Invalidate();
            }
        }

        [DefaultValue(AppHeightDrawMode.TrueHeightAll)]
        [Category("Appearance")]
        [Description("Determines how the appointment is drawn.")]
        public AppHeightDrawMode AppHeightMode
        {
            get { return appHeightMode; }
            set { appHeightMode = value; }
        }

        [DefaultValue(4)]
        [Category("Appearance")]
        [Description("Determines the number of slots to show for each hour")]
        public int SlotsPerHour
        {
            get
            {
                return slotsPerHour;
            }
            set
            {
                if (value > 60)
                {
                    value = 60;
                }
                else if (value < 1)
                {
                    value = 1;
                }
                slotsPerHour = value;

                if (value > ScrollRows)
                    ScrollRows = value;

                Invalidate();
            }
        }

        [DefaultValue(18)]
        [Category("Appearance")]
        [Description("Determines wether the minutes are shown by the hour")]
        public bool ShowMinutes
        {
            get
            {
                return showMinutes;
            }

            set
            {
                showMinutes = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Determines wether AM/PM or 24 hour clock is shown")]
        public bool AmPmDisplay
        {
            get
            {
                return ampmdisplay;
            }
            set
            {
                ampmdisplay = value;
                Invalidate();
            }
        }

        [DefaultValue(8)]
        [Category("Appearance")]
        [Description("Determines what hour is at the top when loaded")]
        public int StartHour
        {
            get
            {
                return startHour;
            }
            set
            {
                startHour = value;
                OnStartHourChanged();
            }
        }

        #endregion Appearance Properties

        #region Runtime Only Properties

        [Browsable(false)]
        [Category("Runtime")]
        [Description("Retrieves the selection type")]
        public SelectionType Selection
        {
            get
            {
                return selection;
            }
        }

        [Description("Determines which column is selected during runtime")]
        [Category("Runtime")]
        [Browsable(false)]
        [DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int SelectedColumn
        {
            get
            {
                return selectedColumn;
            }

            set
            {
                selectedColumn = value;
            }
        }

        [Browsable(false)]
        [Category("Runtime")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AbstractRenderer Renderer
        {
            get
            {
                return renderer;
            }
            set
            {
                renderer = value;
                OnRendererChanged();
            }
        }

        [Browsable(false)]
        [Category("Runtime")]
        [Description("Sets/Gets the selected appointment during runtime.")]
        public Appointment SelectedAppointment
        {
            get { return _selectedAppointment; }

            set
            {
                //is the selected appointment being nulled?
                if ((value == null && _selectedAppointment != null) || (value != _selectedAppointment) && _selectedAppointment != null)
                    RaiseAppointmentDeSelected(_selectedAppointment);

                if (value != _selectedAppointment)
                    RaiseAppointmentSelected(value);

                _selectedAppointment = value;
            }
        }

        [Browsable(false)]
        [Category("Runtime")]
        [Description("Sets/Gets the selected start date/time during runtime.")]
        public DateTime SelectionStart
        {
            get { return selectionStart; }

            set 
            {
                if (selectionStart != value)
                {
                    selectionStart = value;
                }
            }
        }

        [Browsable(false)]
        [Category("Runtime")]
        [Description("Sets/Gets the selected end date/time during runtime.")]
        public DateTime SelectionEnd
        {
            get { return selectionEnd; }
            set { selectionEnd = value; }
        }

        [Browsable(false)]
        [Category("Runtime")]
        [Description("Sets/Gets the Active Tool during runtime.")]
        public ITool ActiveTool
        {
            get { return activeTool; }
            set { activeTool = value; }
        }

        [Browsable(false)]
        [Category("Runtime")]
        [Description("Determines wether an appointment is being edited during runtime.")]
        public bool CurrentlyEditing
        {
            get
            {
                return editbox.Visible;
            }
        }

        [Browsable(false)]
        [Category("Runtime")]
        [Description("Determines wether the selected appointment is a new appointment during runtime.")]
        public bool SelectedAppointmentIsNew
        {
            get
            {
                return selectedAppointmentIsNew;
            }
        }

        
        #endregion Runtime Only Properties

        #region Working Hours

        [Category("Working Hours")]
        [Description("Start Hour for working day.")]
        [DefaultValue(8)]
        public int WorkingHourStart
        {
            get
            {
                return workingHourStart;
            }
            set
            {
                workingHourStart = value;
                UpdateWorkingHours();
            }
        }

        [Category("Working Hours")]
        [Description("Start Minute for working day.")]
        [DefaultValue(30)]
        public int WorkingMinuteStart
        {
            get
            {
                return workingMinuteStart;
            }
            set
            {
                workingMinuteStart = value;
                UpdateWorkingHours();
            }
        }

        [Category("Working Hours")]
        [Description("End Hour for working day.")]
        [DefaultValue(18)]
        public int WorkingHourEnd
        {
            get
            {
                return workingHourEnd;
            }
            set
            {
                workingHourEnd = value;
                UpdateWorkingHours();
            }
        }

        [Category("Working Hours")]
        [Description("End Minute for working day.")]
        [DefaultValue(30)]
        public int WorkingMinuteEnd
        {
            get { return workingMinuteEnd; }
            set
            {
                workingMinuteEnd = value;
                UpdateWorkingHours();
            }
        }

        #endregion Working Hours

        #region Default Properties now Hidden
        /// <summary>
        /// no longer needed so hide it
        /// </summary>
        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
            }
        }

        #endregion Default Properties now Hidden

        #region Behaviour Properties

        [Category("Behavior")]
        [Description("Start Date for SingleView, selected date for TeamView")]
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                if (startDate.Date != value.Date)
                {
                    startDate = value;
                    OnStartDateChanged();
                }
            }
        }

        [Description("Determines wether scrolling is enabled or not.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool AllowScroll
        {
            get
            {
                return allowScroll;
            }
            set
            {
                allowScroll = value;
                OnAllowScrollChanged();
            }
        }

        [Description("How Many Rows to Scroll with one movement of the mouse wheel/scroll bar (1 = 1/4 hour; 4 = 1 hour)")]
        [Category("Behavior")]
        [DefaultValue(4)]
        public int ScrollRows
        {
            get
            {
                return _scrollRows;
            }

            set
            {
                _scrollRows = value;

                if (_scrollRows < 1)
                    _scrollRows = 1;

                if (_scrollRows > slotsPerHour)
                    _scrollRows = slotsPerHour;

                scrollBarV.SmallChange = halfHourHeight * _scrollRows;
                scrollBarV.LargeChange = halfHourHeight * _scrollRows;
            }
        }


        [Category("Behavior")]
        [Description("ContextMenuStrip for Header section")]
        public ContextMenuStrip ContextMenuHeader
        {
            get { return _ContextMenuHeader; }
            set { _ContextMenuHeader = value; }
        }

        [Category("Behavior")]
        [Description("ContextMenuStrip for All Day section")]
        public ContextMenuStrip ContextMenuAllDay
        {
            get { return _ContextMenuAllDay; }
            set { _ContextMenuAllDay = value; }
        }

        [Category("Behavior")]
        [Description("ContextMenuStrip for main diary section")]
        public ContextMenuStrip ContextMenuDiary
        {
            get { return _ContextMenuDiary; }
            set { _ContextMenuDiary = value; }
        }



        #endregion Behaviour Properties

        #region Appointment Properties

        [Category("Appointments")]
        [Description("Determines wether In Place Editing is permitted.")]
        [DefaultValue(true)]
        public bool AllowInplaceEditing
        {
            get
            {
                return allowInplaceEditing;
            }
            set
            {
                allowInplaceEditing = value;
            }
        }

        [Description("Determines wether the right mouse button will select an appointment")]
        [Category("Appointments")]
        [DefaultValue(false)]
        public bool RightMouseSelect
        {
            get
            {
                return _rightMouseSelect;
            }

            set
            {
                _rightMouseSelect = value;
            }
        }
        
        [DefaultValue(true)]
        [Category("Appointments")]
        [Description("Determines wether a new appointment can be created by typing.")]
        public bool AllowNew
        {
            get
            {
                return allowNew;
            }
            set
            {
                allowNew = value;
            }
        }

        [Category("Appointments")]
        [Description("Determines wether the appointment text is always shown for long appointments.")]
        public bool AlwaysShowAppointmentText
        {
            get
            {
                return _alwaysShowAppointmentText;
            }

            set
            {
                _alwaysShowAppointmentText = value;
                Refresh();
            }
        }

        [Category("Appointments")]
        [Description("Determines wether the borders are shown for all appointments.")]
        [DefaultValue(true)]
        public bool DrawAllAppointmentBorders
        {
            get
            {
                return drawAllAppBorder;
            }
            set
            {
                drawAllAppBorder = value;
                Invalidate();
            }
        }

        [Category("Appointments")]
        [Description("Determines wether appointments can be resized")]
        [DefaultValue(true)]
        public bool AllowAppointmentResize
        {
            get
            {
                return _allowAppointmentResize;
            }
            set
            {
                _allowAppointmentResize = value;
                Invalidate();
            }
        }

        #endregion Appointment Properties

        #region Tooltip

        [Category("Tooltips")]
        [Description("Determines how long to wait before a tooltip event is fired.")]
        [DefaultValue(2500)]
        public int TooltipDelay
        {
            get
            {
                return _timerTooltip.Interval;
            }

            set
            {
                if (value > 0)
                    _timerTooltip.Interval = value;
            }
        }

        #endregion Tooltip

        #region Internal Properties

        /// <summary>
        /// Retrieves the *actual* header height
        /// </summary>
        internal int HeaderHeight
        {
            get
            {
                return dayHeadersHeight + allDayEventsHeaderHeight;
            }
        }

        #endregion Internal Properties

        #endregion Properties

        #region Private Methods

        #region Event Update Methods

        private bool Between(int value, int left, int right)
        {
            return value > left && value < right;
        }

        /// <summary>
        /// Returns a value ensuring it is at least Minimum.
        /// </summary>
        /// <param name="minimum">Minimum value to return</param>
        /// <param name="value">Value to compare</param>
        /// <returns>if value less than minimum then returns minimum, otherwise returns value</returns>
        private int MinValue(int minimum, int value)
        {
            if (minimum > 0 && value < minimum)
                return minimum;
            else
                return value;
        }

        private void OnHalfHourHeightChanged()
        {
            AdjustScrollbar();
            Invalidate();
        }

        private void OnRendererChanged()
        {
            Font = renderer.BaseFont;
            Invalidate();
        }

        protected virtual void OnDaysToShowChanged()
        {
            if (CurrentlyEditing)
                FinishEditing(true);

            Invalidate();
        }

        protected virtual void OnStartDateChanged()
        {
            startDate = startDate.Date;

            SelectedAppointment = null;
            selectedAppointmentIsNew = false;
            selection = SelectionType.DateRange;

            Invalidate();
        }


        protected virtual void OnStartHourChanged()
        {
            if ((startHour * slotsPerHour * halfHourHeight) > scrollBarV.Maximum) //maximum is lower on larger forms
            {
                scrollBarV.Value = scrollBarV.Maximum;
            }
            else
            {
                scrollBarV.Value = startHour * slotsPerHour * halfHourHeight;
            }

            Invalidate();
        }

        private void UpdateWorkingHours()
        {
            workStart = new DateTime(1, 1, 1, workingHourStart, workingMinuteStart, 0);
            workEnd = new DateTime(1, 1, 1, workingHourEnd, workingMinuteEnd, 0);

            Invalidate();
        }

        /// <summary>
        /// Returns the visible rectangle for appointment columns
        /// </summary>
        /// <returns>Rectangle with co-ordinates of Dayview</returns>
        private Rectangle GetVisibleRectangle()
        {
            // Calculate visible rectangle
            Rectangle Result = new Rectangle(0, 0, Width - scrollBarV.Width, Height);

            Result.X += hourLabelWidth + hourLabelIndent;
            Result.Y += HeaderHeight;
            Result.Width -= hourLabelWidth + hourLabelIndent;

            return Result;
        }
        private void OnAllowScrollChanged()
        {
            scrollBarV.Visible = AllowScroll;
        }

        #endregion Event Update Methods


        #endregion Private Methods

        #region Event Handlers

        void editbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                FinishEditing(true);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                FinishEditing(false);

                if (selectedAppointmentIsNew)
                    RaiseNewAppointment();
                else
                    RaiseAppointmentUpdated();

                Refresh();
            }
        }

        void selectionTool_Complete(object sender, EventArgs e)
        {
            if (SelectedAppointment != null)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(EnterEditMode));
            }
        }

        void scrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            Invalidate();

            if (editbox.Visible)
                //scroll text box too
                editbox.Top += e.OldValue - e.NewValue;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
            AdjustScrollbar();
        }

        private void AdjustScrollbar()
        {
            scrollBarV.Maximum = (slotsPerHour * halfHourHeight * 24) - (Height - HeaderHeight); // 24 is hours in a day (not the 25 originally put there
            scrollBarV.Minimum = 0;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Flicker free
        }

        protected override void OnLeave(EventArgs e)
        {
            _timerTooltip.Enabled = false;
            base.OnLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Capture focus
            Focus();

            if (CurrentlyEditing)
            {
                FinishEditing(false);
            }

            if (selectedAppointmentIsNew)
            {
                RaiseNewAppointment();
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                return;

            ITool newTool = null;

            Appointment appointment = GetAppointmentAt(e.X, e.Y);

            if (e.Y < HeaderHeight && e.Y > dayHeadersHeight && appointment == null)
            {
                if (SelectedAppointment != null)
                {
                    SelectedAppointment = null;
                    Invalidate();
                }

                newTool = drawTool;
                selection = SelectionType.None;

                base.OnMouseDown(e);
                return;
            }

            if (appointment == null)
            {
                if (SelectedAppointment != null)
                {
                    SelectedAppointment = null;
                    Invalidate();
                }

                newTool = drawTool;
                selection = SelectionType.DateRange;
            }
            else
            {
                newTool = selectionTool;
                SelectedAppointment = appointment;
                selection = SelectionType.Appointment;

                Invalidate();
            }

            if (activeTool != null)
            {
                activeTool.MouseDown(e);
            }

            if ((activeTool != newTool) && (newTool != null))
            {
                newTool.Reset();
                newTool.MouseDown(e);
            }

            activeTool = newTool;
            _timerTooltip.Enabled = false;

            base.OnMouseDown(e);
        }

        protected override void Dispose(bool disposing)
        {
            _timerTooltip.Enabled = false;
            base.Dispose(disposing);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (activeTool != null)
                activeTool.MouseMove(e);

            base.OnMouseMove(e);

            //tooltip code
            if (Between(e.X, _oldPoint.X - 10, _oldPoint.X + 10) && Between(e.Y, _oldPoint.Y - 10, _oldPoint.Y + 10))
                return;

            if (_moveStart)
            {
                _tooltip.Hide(this);
                _oldPoint = new Point(e.X, e.Y);

                _timerTooltip.Enabled = e.Button == System.Windows.Forms.MouseButtons.None;
            }
            else
            {
                _moveStart = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (activeTool != null)
                activeTool.MouseUp(e);

            base.OnMouseUp(e);
        }

        protected override void OnClick(EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;

            if (args.Button == MouseButtons.Right)
            {

                MouseLocation location = MouseOver(out int col, out DateTime date);

                if (location == MouseLocation.Header)
                {
                    if (ContextMenuHeader != null)
                    {
                        RaiseBeforeShowContextMenu(new ContextMenuEventArgs(date, col, ContextMenuHeader));
                        ContextMenuHeader.Show(MousePosition);
                    }
                }
                else
                {
                    if (location == MouseLocation.Diary)
                    {
                        if (ContextMenuDiary != null)
                        {
                            RaiseBeforeShowContextMenu(new ContextMenuEventArgs(date, col, ContextMenuDiary));
                            ContextMenuDiary.Show(MousePosition);
                        }
                    }
                    else
                    {
                        if (ContextMenuAllDay != null)
                        {
                            RaiseBeforeShowContextMenu(new ContextMenuEventArgs(date, col, ContextMenuAllDay));
                            ContextMenuAllDay.Show(MousePosition);
                        }
                    }

                }
            }

            base.OnClick(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            int Column = 0;
            DateTime EditDate = DateTime.Now;

            if (GetMouseOverHeader(ref Column, ref EditDate))
            {
                //ShowHardConfirm("Edit User", String.Format("Edit hours for {0} on {1}", newtherapist.EmployeeName, EditDate.ToShortDateString()));
                RaiseDoubleClickHeader(new HeaderClickEventArgs(EditDate, Column));
            }
            else
                base.OnDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            _tooltip.Hide(this);

            if (e.Delta < 0)
            {//mouse wheel scroll down
                ScrollMe(true);
            }
            else
            {//mouse wheel scroll up
                ScrollMe(false);
            }
        }

        readonly System.Collections.Hashtable cachedAppointments = new System.Collections.Hashtable();

        protected virtual void OnResolveAppointments(ResolveAppointmentsEventArgs args)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Resolve app");
#endif
            ResolveAppointments?.Invoke(this, args);


            allDayEventsHeaderHeight = 0;

            // cache resolved appointments in hashtable by days.
            cachedAppointments.Clear();
            appointmentViews.Clear();
            longappointmentViews.Clear();

            if (selectedAppointmentIsNew && (SelectedAppointment != null))
            {
                if ((SelectedAppointment.StartDate > args.StartDate) && (SelectedAppointment.StartDate < args.EndDate))
                {
                    args.Appointments.Add(SelectedAppointment);
                }
            }

            foreach (Appointment appointment in args.Appointments)
            {
                int key;  // key = day in singleview and column in multiview
                AppointmentList list;

                if (ViewType == DayViewType.SingleView)
                {
                    if (appointment.StartDate.Day == appointment.EndDate.Day && !appointment.AllDayEvent)
                    {
                        key = appointment.StartDate.Day;
                    }
                    else
                    {
                        key = -1;
                    }
                }
                else // multiview
                {
                    if (appointment.AllDayEvent)
                        key = -1;
                    else
                        key = appointment.Column;
                }

                list = (AppointmentList)cachedAppointments[key];

                if (list == null)
                {
                    list = new AppointmentList();
                    cachedAppointments[key] = list;
                }

                list.Add(appointment);
            }
        }

        internal void RaiseBeforeShowContextMenu(ContextMenuEventArgs e)
        {
            BeforeContextMenuShow?.Invoke(this, e);
        }

        internal void RaiseAppointmentSelected(Appointment appointment)
        {
            AppointmentSelected?.Invoke(this, new AppointmentSelectedEventArgs(appointment, true));
        }

        internal void RaiseAppointmentDeSelected(Appointment appointment)
        {
            AppointmentSelected?.Invoke(this, new AppointmentSelectedEventArgs(appointment, false));
        }

        /// <summary>
        /// Gets the header text for the column in Team View
        /// </summary>
        /// <param name="e">class</param>
        internal void RaiseMultiCountHeader(TeamViewGetEventArgs e)
        {
            MultiHeader?.Invoke(this, e);
        }

        /// <summary>
        /// Gets the number of columns to show
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseMultiCount(TeamViewCountEventArgs e)
        {
            MultiCount?.Invoke(this, e);
        }

        internal void RaiseWorkingHours(WorkingHoursEventArgs e)
        {
            WorkingHours?.Invoke(this, e);
        }

        internal void RaiseTooltip(TooltipEventArgs e)
        {
            ToolTipShow?.Invoke(this, e);
        }

        internal void RaiseDoubleClickHeader(HeaderClickEventArgs e)
        {
            HeaderClicked?.Invoke(this, e);
        }

        internal void RaiseSelectionChanged(EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        internal void RaiseBeforeAppointmentMoved(AppointmentMoveEventArgs e)
        {
            BeforeAppointmentMove?.Invoke(this, e);
        }

        internal void RaiseAppointmentMove(AppointmentEventArgs e)
        {
            AppointmentMove?.Invoke(this, e);
        }

        internal void RaiseAppointmentMoved(AppointmentEventArgs e)
        {
            AppointmentMoved?.Invoke(this, e);
        }

        internal void RaiseAfterDrawHeader(AfterDrawHeaderEventArgs e)
        {
            AfterDrawHeader?.Invoke(this, e);
        }

        internal void RaiseAfterDrawAppointment(AfterDrawAppointmentEventArgs e)
        {
            AfterDrawAppointment?.Invoke(this, e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (allowNew && char.IsLetterOrDigit(e.KeyChar))
            {
                if (Selection == SelectionType.DateRange)
                {
                    if (!selectedAppointmentIsNew)
                        EnterNewAppointmentMode(e.KeyChar);
                }
            }
        }

        private void EnterNewAppointmentMode(char key)
        {
            Appointment appointment = new Appointment();
            appointment.StartDate = selectionStart;
            appointment.EndDate = selectionEnd;
            appointment.Column = SelectedColumn;
            appointment.Title = key.ToString();

            //Calculate appointment rectangle
            Rectangle appRect = new Rectangle();
            appRect = GetHourRangeRectangle(selectionStart, selectionEnd, appRect);
            appRect.Width = (Width - (hourLabelWidth + hourLabelIndent + scrollBarV.Width)) / columnCount;
            appRect.X = hourLabelWidth + hourLabelIndent + (appRect.Width * selectedColumn);

            //Add appointment to appointmentViews dictionary
            AppointmentView view = new AppointmentView();
            view.Rectangle = appRect;
            view.Appointment = appointment;

            appointmentViews[appointment] = view;

            SelectedAppointment = appointment;
            selectedAppointmentIsNew = true;
            activeTool = selectionTool;

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(EnterEditMode));
        }

        private delegate void StartEditModeDelegate(object state);

        private void EnterEditMode(object state)
        {
            if (!allowInplaceEditing)
                return;

            if (InvokeRequired)
            {
                Appointment selectedApp = SelectedAppointment;

                //System.Threading.Thread.Sleep(200);

                if (selectedApp == SelectedAppointment)
                    Invoke(new StartEditModeDelegate(EnterEditMode), state);
            }
            else
            {
                StartEditing();
            }
        }

        internal void RaiseNewAppointment()
        {
            NewAppointmentEventArgs args = new NewAppointmentEventArgs(SelectedAppointment.Title, SelectedAppointment.StartDate, SelectedAppointment.EndDate, SelectedAppointment.Column);

            NewAppointment?.Invoke(this, args);

            SelectedAppointment = null;
            selectedAppointmentIsNew = false;

            Invalidate();
        }

        private void RaiseAppointmentUpdated()
        {
            AppointmentEventArgs args = new AppointmentEventArgs(SelectedAppointment);

            AppointmentUpdated?.Invoke(this, args);

            SelectedAppointment = null;
            selectedAppointmentIsNew = false;

            Invalidate();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Forces a refresh of the DayView and re-draws
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            Invalidate();
        }

        /// <summary>
        /// Determines wether dates intersect
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="finishDate"></param>
        /// <param name="checkDateStart"></param>
        /// <param name="checkDateFinish"></param>
        /// <returns></returns>
        public bool DateWithin(DateTime dateStart, DateTime finishDate, DateTime checkDateStart, DateTime checkDateFinish)
        {
            bool Result = checkDateStart >= dateStart && checkDateStart <= finishDate;

            if (!Result)
                Result = checkDateFinish <= finishDate && checkDateFinish >= dateStart;

            return Result;
        }


        /// <summary>
        /// Prints a copy of the calendar
        /// </summary>
        /// <param name="e"></param>
        public void Print(PaintEventArgs e)
        {
            TeamViewCountEventArgs multiCount = new TeamViewCountEventArgs(1);
            RaiseMultiCount(multiCount);

            using (SolidBrush backBrush = new SolidBrush(renderer.BackColor))
                e.Graphics.FillRectangle(backBrush, e.ClipRectangle);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Calculate visible rectangle
            Rectangle rectangle = e.ClipRectangle;

            Rectangle hourLabelRectangle = rectangle;

            hourLabelRectangle.Y += HeaderHeight;

            DrawHourLabels(e, hourLabelRectangle);

            Rectangle daysRectangle = rectangle;
            daysRectangle.X += hourLabelWidth;
            daysRectangle.Y += HeaderHeight;
            daysRectangle.Width -= hourLabelWidth;

            if (e.ClipRectangle.IntersectsWith(daysRectangle))
            {
                DrawDays(e, daysRectangle);
            }

            Rectangle headerRectangle = rectangle;

            headerRectangle.X += hourLabelWidth;
            headerRectangle.Width -= hourLabelWidth;
            headerRectangle.Height = dayHeadersHeight;

            if (e.ClipRectangle.IntersectsWith(headerRectangle))
                DrawDayHeaders(e, headerRectangle);
        }
 
        /// <summary>
        /// Scroll's DayView up or down
        /// </summary>
        /// <param name="down"></param>
        public void ScrollMe(bool down)
        {
            if (AllowScroll)
            {
                int newScrollValue;

                if (down)
                {//mouse wheel scroll down
                    newScrollValue = scrollBarV.Value + scrollBarV.SmallChange;

                    if (newScrollValue < scrollBarV.Maximum)
                        scrollBarV.Value = newScrollValue;
                    else
                        scrollBarV.Value = scrollBarV.Maximum;
                }
                else
                {//mouse wheel scroll up
                    newScrollValue = scrollBarV.Value - scrollBarV.SmallChange;

                    if (newScrollValue > scrollBarV.Minimum)
                        scrollBarV.Value = newScrollValue;
                    else
                        scrollBarV.Value = scrollBarV.Minimum;
                }

                Invalidate();
            }
        }

        public Rectangle GetTrueRectangle()
        {
            Rectangle truerect;

            truerect = ClientRectangle;
            truerect.X += hourLabelWidth + hourLabelIndent;
            truerect.Width -= scrollBarV.Width + hourLabelWidth + hourLabelIndent;
            truerect.Y += HeaderHeight;
            truerect.Height -= HeaderHeight;

            return truerect;
        }

        public Rectangle GetFullDayApptsRectangle()
        {
            Rectangle fulldayrect;
            fulldayrect = ClientRectangle;
            fulldayrect.Height = HeaderHeight - dayHeadersHeight;
            fulldayrect.Y += dayHeadersHeight;
            fulldayrect.Width -= hourLabelWidth + hourLabelIndent + scrollBarV.Width;
            fulldayrect.X += hourLabelWidth + hourLabelIndent;

            return fulldayrect;
        }

        /// <summary>
        /// Starts inline editing of an appointment
        /// </summary>
        public void StartEditing()
        {
            if (!SelectedAppointment.Locked && appointmentViews.ContainsKey(SelectedAppointment))
            {
                Rectangle editBounds = appointmentViews[SelectedAppointment].Rectangle;

                editBounds.Inflate(-3, -3);
                editBounds.X += appointmentGripWidth - 2;
                editBounds.Width -= appointmentGripWidth - 5;

                editbox.Bounds = editBounds;
                editbox.Text = SelectedAppointment.Title;
                editbox.Visible = true;
                editbox.SelectionStart = editbox.Text.Length;
                editbox.SelectionLength = 0;

                editbox.Focus();
            }
        }

        /// <summary>
        /// Finishes editing an appointment
        /// </summary>
        /// <param name="cancel">if true the edit's are cancelled</param>
        public void FinishEditing(bool cancel)
        {
            editbox.Visible = false;

            if (!cancel)
            {
                if (SelectedAppointment != null)
                    SelectedAppointment.Title = editbox.Text;
            }
            else
            {
                if (selectedAppointmentIsNew)
                {
                    SelectedAppointment = null;
                    selectedAppointmentIsNew = false;
                }
            }

            Invalidate();
            Focus();
        }

        /// <summary>
        /// Retrieves the time where the cursor resides
        /// </summary>
        /// <param name="Column">Column number</param>
        /// <returns>DateTime object with date/time of cell</returns>
        public DateTime GetTimeAt(ref int Column)
        {
            return GetTimeAt(_oldPoint.X, _oldPoint.Y, ref Column);
        }

        /// <summary>
        /// Determines if the mouse is over the header (any column)
        /// </summary>
        /// <returns></returns>
        public bool IsMouseOverHeader()
        {
            Point currPos = PointToClient(MousePosition);
            return currPos.Y < dayHeadersHeight;
        }

        /// <summary>
        /// Determines what area of the diary the mouse is over.
        /// </summary>
        /// <param name="Column">The column, if over a column</param>
        /// <param name="Date">Date for column, if over a column</param>
        /// <returns>MouseLocation enum</returns>
        public MouseLocation MouseOver(out int Column, out DateTime Date)
        {
            MouseLocation Result = MouseLocation.Diary;

            Point currPos = PointToClient(MousePosition);

            if (currPos.X < hourLabelWidth)
                Result = MouseLocation.Times;
            else
            {
                if (currPos.Y > dayHeadersHeight && currPos.Y < (dayHeadersHeight + allDayEventsHeaderHeight))
                {
                    Result = MouseLocation.AllDay;
                }
                else
                {
                    if (currPos.Y < dayHeadersHeight)
                    {
                        Result = MouseLocation.Header;
                    }
                }
            }

            if (Result != MouseLocation.Times)
            {
                TeamViewCountEventArgs multiCount = new TeamViewCountEventArgs(1);
                RaiseMultiCount(multiCount);

                int dayWidth = (Width - (scrollBarV.Width + hourLabelWidth + hourLabelIndent)) / multiCount.Count;

                int hour = (currPos.Y - HeaderHeight + scrollBarV.Value) / halfHourHeight;
                currPos.X -= hourLabelWidth;

                DateTime date = startDate;

                if (ViewType == DayViewType.SingleView)
                {
                    Date = date.AddDays(currPos.X / dayWidth).Date;
                }
                else
                {
                    Date = startDate.Date;
                }

                Column = currPos.X / dayWidth;

                if (Column >= multiCount.Count)
                    Column--;
            }
            else
            {
                Column = -1;
                Date = startDate;
            }

            return Result;
        }

        /// <summary>
        /// Determines if the mouse cursor is over the Header
        /// </summary>
        /// <param name="Column">The column where the mouse is over</param>
        /// <param name="Date">The date for the column (DayView), the current date in TeamView</param>
        /// <returns>bool, true if the mouse is over the header, otherwise false</returns>
        public bool GetMouseOverHeader(ref int Column, ref DateTime Date)
        {
            Point currPos = PointToClient(MousePosition);

            TeamViewCountEventArgs multiCount = new TeamViewCountEventArgs(1);
            RaiseMultiCount(multiCount);

            int dayWidth = (Width - (scrollBarV.Width + hourLabelWidth + hourLabelIndent)) / (multiCount.Count == 0 ? 1 : multiCount.Count);

            int hour = (currPos.Y - HeaderHeight + scrollBarV.Value) / halfHourHeight;
            currPos.X -= hourLabelWidth;

            DateTime date = startDate;

            if (ViewType == DayViewType.SingleView)
            {
                Date = date.AddDays(currPos.X / dayWidth).Date;
                TimeSpan span = Date - startDate;
                Column = span.Days + 1;
            }
            else
            {
                Date = startDate.Date;
                Column = currPos.X / dayWidth;

                if (Column >= multiCount.Count)
                    Column--;
            }

            return currPos.Y < dayHeadersHeight;
        }

        public void GetColumnFromMousePosition(out int Column, out DateTime Date)
        {
            Point currPos = PointToClient(MousePosition);

            TeamViewCountEventArgs multiCount = new TeamViewCountEventArgs(1);
            RaiseMultiCount(multiCount);

            int dayWidth = (Width - (scrollBarV.Width + hourLabelWidth + hourLabelIndent)) / multiCount.Count;

            int hour = (currPos.Y - HeaderHeight + scrollBarV.Value) / halfHourHeight;
            currPos.X -= hourLabelWidth;

            DateTime date = startDate;

            if (ViewType == DayViewType.SingleView)
            {
                Date = date.AddDays(currPos.X / dayWidth).Date;
            }
            else
            {
                Date = startDate.Date;
            }

            Column = currPos.X / dayWidth;

            if (Column >= multiCount.Count)
                Column--;
        }

        public DateTime GetTimeAt(int x, int y, ref int Column)
        {
            TeamViewCountEventArgs multiCount = new TeamViewCountEventArgs(1);
            RaiseMultiCount(multiCount);

            int dayWidth = (Width - (scrollBarV.Width + hourLabelWidth + hourLabelIndent)) / (multiCount.Count == 0 ? 1 : multiCount.Count);

            int hour = (y - HeaderHeight + scrollBarV.Value) / halfHourHeight;
            x -= hourLabelWidth;

            DateTime date = startDate;

            date = date.Date;

            if (ViewType == DayViewType.SingleView)
            {
                date = date.AddDays(x / dayWidth);
            }
            else
            {
                Column = x / dayWidth;

                if (Column >= multiCount.Count)
                    Column--;
            }

            if ((hour > 0) && (hour < 24 * slotsPerHour))
                date = date.AddMinutes(hour * (60 / slotsPerHour));

            return date;
        }

        public Appointment GetAppointmentAt(int x, int y)
        {

            foreach (AppointmentView view in appointmentViews.Values)
                if (view.Rectangle.Contains(x, y))
                    return view.Appointment;

            foreach (AppointmentView view in longappointmentViews.Values)
                if (view.Rectangle.Contains(x, y))
                    return view.Appointment;

            return null;
        }

        #endregion

        #region Drawing Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            if (CurrentlyEditing)
            {
                FinishEditing(false);
            }

            TeamViewCountEventArgs multiCount = new TeamViewCountEventArgs(1);
            RaiseMultiCount(multiCount);
            
            //store number of columns
            columnCount = multiCount.Count;

            // resolve appointments on visible date range.
            ResolveAppointmentsEventArgs args = new ResolveAppointmentsEventArgs(StartDate, ViewType == DayViewType.SingleView ? StartDate.AddDays(multiCount.Count) : startDate.AddDays(0));
            OnResolveAppointments(args);

            using (SolidBrush backBrush = new SolidBrush(renderer.BackColor))
                e.Graphics.FillRectangle(backBrush, ClientRectangle);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Calculate visible rectangle
            Rectangle rectangle = new Rectangle(0, 0, Width - scrollBarV.Width, Height);


            Rectangle daysRectangle = GetVisibleRectangle();

            if (e.ClipRectangle.IntersectsWith(daysRectangle))
            {
                DrawDays(e, daysRectangle);
            }

            Rectangle hourLabelRectangle = rectangle;

            hourLabelRectangle.Y += HeaderHeight;

            DrawHourLabels(e, hourLabelRectangle);

            Rectangle headerRectangle = rectangle;

            headerRectangle.X += hourLabelWidth + hourLabelIndent;
            headerRectangle.Width -= hourLabelWidth + hourLabelIndent;
            headerRectangle.Height = dayHeadersHeight;

            if (multiCount.Count > 0 && e.ClipRectangle.IntersectsWith(headerRectangle))
                DrawDayHeaders(e, headerRectangle);

            Rectangle scrollrect = rectangle;

            if (!AllowScroll)
            {
                scrollrect.X = headerRectangle.Width + hourLabelWidth + hourLabelIndent;
                scrollrect.Width = scrollBarV.Width;
                using (SolidBrush backBrush = new SolidBrush(renderer.BackColor))
                    e.Graphics.FillRectangle(backBrush, scrollrect);
            }

            AdjustScrollbar();
        }

        private void DrawHourLabels(PaintEventArgs e, Rectangle rect)
        {
            e.Graphics.SetClip(rect);

            for (int m_Hour = 0; m_Hour < 24; m_Hour++)
            {
                Rectangle hourRectangle = rect;

                hourRectangle.Y = rect.Y + (m_Hour * slotsPerHour * halfHourHeight) - scrollBarV.Value;
                hourRectangle.X += hourLabelIndent;
                hourRectangle.Width = hourLabelWidth;

                //Highlight the current time in the hour bar
                if (highlightCurrentTime && DateTime.Now.Hour == m_Hour)
                {
                    //hourRectangle.Y = rect.Y + (m_Hour * slotsPerHour * halfHourHeight) - scrollbar.Value;
                    int minuteLine = (int)Math.Round((double)(slotsPerHour * halfHourHeight) / 60 * DateTime.Now.Minute, 0, MidpointRounding.ToEven);

                    Pen pen = new Pen(Color.Red);
                    e.Graphics.DrawLine(pen, hourRectangle.Left, hourRectangle.Y + minuteLine, hourRectangle.Width, hourRectangle.Y + minuteLine);

                    float merge = 0.75f;

                    for (int i = 1; i < 10; ++i)
                    {
                        pen = new Pen(AbstractRenderer.InterpolateColors(renderer.BackColor, Color.Gold, merge));
                        e.Graphics.DrawLine(pen, hourRectangle.Left, hourRectangle.Y + (minuteLine - i), hourRectangle.Width, hourRectangle.Y + (minuteLine - i));
                        merge -= 0.05f;
                    }
                }

                renderer.DrawHourLabel(e.Graphics, hourRectangle, m_Hour, ampmdisplay);

                for (int slot = 0; slot < slotsPerHour; slot++)
                {
                    int Minute = (int)(60 / slotsPerHour) * slot;
                    renderer.DrawMinuteLine(e.Graphics, hourRectangle, Minute);                   
                    hourRectangle.Y += halfHourHeight;
                }
            }

            e.Graphics.ResetClip();
        }

        private void DrawDayHeaders(PaintEventArgs e, Rectangle rect)
        {
            TeamViewCountEventArgs multiCount = new TeamViewCountEventArgs(1);
            RaiseMultiCount(multiCount);

            int dayWidth = rect.Width / multiCount.Count;

            // one day header rectangle
            Rectangle dayHeaderRectangle = new Rectangle(rect.Left, rect.Top, dayWidth, rect.Height);
            DateTime headerDate = startDate;

            for (int day = 0; day < multiCount.Count; day++)
            {
                if (day == 0)
                    dayHeaderRectangle.Width += (rect.Width % multiCount.Count) - 1;

                TeamViewGetEventArgs args = new TeamViewGetEventArgs(day);
                RaiseMultiCountHeader(args);

                renderer.DrawDayHeader(e.Graphics, dayHeaderRectangle, args.HeaderText, headerDate);
                RaiseAfterDrawHeader(new AfterDrawHeaderEventArgs(e.Graphics, dayHeaderRectangle, day, headerDate));

                dayHeaderRectangle.X += dayWidth;

                if (ViewType == DayViewType.SingleView)
                    headerDate = headerDate.AddDays(1);
            }
        }

        private Rectangle GetHourRangeRectangle(DateTime start, DateTime end, Rectangle baseRectangle)
        {
            Rectangle rect = baseRectangle;

            int startY;
            int endY;

            startY = (start.Hour * halfHourHeight * slotsPerHour) + (start.Minute * halfHourHeight / (60 / slotsPerHour));
            endY = (end.Hour * halfHourHeight * slotsPerHour) + (end.Minute * halfHourHeight / (60 / slotsPerHour));

            rect.Y = startY - scrollBarV.Value + HeaderHeight;

            rect.Height = System.Math.Max(1, endY - startY -1);

            return rect;
        }

        private void DrawDay(PaintEventArgs e, Rectangle rect, DateTime time, int Column)
        {
            renderer.DrawDayBackground(e.Graphics, rect);

            Rectangle workingHoursRectangle;
            bool canWork = true;

            if (WorkingHours == null)
            {
                workingHoursRectangle = GetHourRangeRectangle(workStart, workEnd, rect); //default to settings
                canWork = !((time.DayOfWeek == DayOfWeek.Saturday) || (time.DayOfWeek == DayOfWeek.Sunday));//weekends off -> no working hours
            }
            else
            {
                //fire event to get working hours
                WorkingHoursEventArgs args = new WorkingHoursEventArgs(time, Column);
                WorkingHours(this, args);

                DateTime StartTime = new DateTime(1, 1, 1, args.WorkingHourStart, args.WorkingMinuteStart, 0);
                DateTime EndTime = new DateTime(1, 1, 1, args.WorkingHourFinish, args.WorkingMinuteFinish, 0);
                workingHoursRectangle = GetHourRangeRectangle(StartTime, EndTime, rect);
                canWork = args.CanWork;
            }
            
            if (workingHoursRectangle.Y < HeaderHeight)
            {
                workingHoursRectangle.Height -= HeaderHeight - workingHoursRectangle.Y;
                workingHoursRectangle.Y = HeaderHeight;
            }

            if (canWork)
                renderer.DrawHourRange(e.Graphics, workingHoursRectangle, false, false, canWork);

            if (selection == SelectionType.DateRange)
            {
                Rectangle selectionRectangle = selectionRectangle = GetHourRangeRectangle(selectionStart, selectionEnd, rect);
                bool Selected = false;

                if ((ViewType == DayViewType.SingleView) && (time.Day == selectionStart.Day))
                {
                    Selected = true;
                }
                else
                {
                    if (ViewType == DayViewType.TeamView)
                    {
                        Selected = Column == selectedColumn;
                    }
                    else
                    {
                        Selected = false;
                    }
                }

                if (selectionRectangle.Top + 1 > HeaderHeight && Selected)
                {
                    renderer.DrawHourRange(e.Graphics, selectionRectangle, false, Selected, canWork);
                }
            }

            e.Graphics.SetClip(rect);
            Color color1 = renderer.HourSeperatorColor;
            Color color2 = renderer.HalfHourSeperatorColor;

            for (int hour = 0; hour < 24 * slotsPerHour; hour++)
            {
                int y = rect.Top + (hour * halfHourHeight) - scrollBarV.Value;

                using (Pen pen = new Pen((hour % slotsPerHour) == 0 ? color1 : color2))
                    e.Graphics.DrawLine(pen, rect.Left, y, rect.Right, y);

                if (y > rect.Bottom)
                    break;
            }

            renderer.DrawDayGripper(e.Graphics, rect, appointmentGripWidth,
                ViewType == DayViewType.SingleView ? time : time );

            e.Graphics.ResetClip();
            AppointmentList appointments = null;

            if (ViewType == DayViewType.SingleView)
            {
                appointments = (AppointmentList)cachedAppointments[time.Day];
            }
            else
            {
                appointments = (AppointmentList)cachedAppointments[Column];
            }

            if (appointments != null)
            {
                List<string> groups = new List<string>();

                foreach (Appointment app in appointments)
                    if (!groups.Contains(app.Group))
                        groups.Add(app.Group);

                Rectangle rect2 = rect;
                rect2.Width = rect2.Width / groups.Count;

                groups.Sort();

                foreach (string group in groups)
                {
                    DrawAppointments(e, rect2, time, group, Column);

                    rect2.X += rect2.Width;
                }
            }
        }

        internal Dictionary<Appointment, AppointmentView> appointmentViews = new Dictionary<Appointment, AppointmentView>();
        internal Dictionary<Appointment, AppointmentView> longappointmentViews = new Dictionary<Appointment, AppointmentView>();

        private void DrawAppointments(PaintEventArgs e, Rectangle rect, DateTime time, string group, int Column)
        {
            DateTime timeStart = time.Date;
            DateTime timeEnd = timeStart.AddHours(24);
            timeEnd = timeEnd.AddSeconds(-1);

            AppointmentList appointments = null;

            if (ViewType == DayViewType.SingleView)
            {
                appointments = (AppointmentList)cachedAppointments[time.Day];
            }
            else
            {
                appointments = (AppointmentList)cachedAppointments[Column];
            }

            if (appointments != null)
            {
                AppointmentLayout[] layout = GetMaxParalelAppointments(appointments, slotsPerHour);

                List<Appointment> drawnItems = new List<Appointment>();

                for (int timeBlock = 0; timeBlock < 24 * slotsPerHour; timeBlock++)
                {
                    AppointmentLayout hourLayout = layout[timeBlock];

                    if ((hourLayout != null) && (hourLayout.Count > 0))
                    {
                        for (int appIndex = 0; appIndex < hourLayout.Count; appIndex++)
                        {
                            Appointment appointment = hourLayout.Appointments[appIndex];

                            if (appointment.Group != group)
                                continue;

                            if (drawnItems.IndexOf(appointment) < 0)
                            {
                                Rectangle appRect = rect;
                                int appointmentWidth;
                                AppointmentView view;

                                appointmentWidth = rect.Width / appointment.conflictCount;

                                appRect.Width = appointmentWidth;
                                appRect = GetHourRangeRectangle(appointment.StartDate, appointment.EndDate, appRect);

                                // multiple passes looking for overlaps, depending on conflict count
                                for (int i = 0; i < appointment.conflictCount; i++)
                                {
                                    foreach (Appointment app in drawnItems)
                                    {
                                        // appts stored sequentially, if null no more to process
                                        if (app == null)
                                            break;

                                        if ((app.Group == appointment.Group) && appointmentViews.ContainsKey(app))
                                        {
                                            view = appointmentViews[app];

                                            if (view.Rectangle.IntersectsWith(appRect))
                                            //if (view.Rectangle.Contains(new Point(nextX, appRect.Y)))
                                            {
                                                appRect.X += appointmentWidth;
                                            }
                                        }
                                    }
                                }

                                appRect.Width = appointmentWidth - 5;

                                DateTime appstart = appointment.StartDate;
                                DateTime append = appointment.EndDate;

                                // Draw the appts boxes depending on the height display mode                           

                                // If small appts are to be drawn in half-hour blocks
                                if (AppHeightMode == AppHeightDrawMode.FullHalfHourBlocksShort && appointment.EndDate.Subtract(appointment.StartDate).TotalMinutes < (60 / slotsPerHour))
                                {
                                    // Round the start/end time to the last/next halfhour
                                    appstart = appointment.StartDate.AddMinutes(-appointment.StartDate.Minute);
                                    append = appointment.EndDate.AddMinutes((60 / slotsPerHour) - appointment.EndDate.Minute);

                                    // Make sure we've rounded it to the correct halfhour :)
                                    if (appointment.StartDate.Minute >= (60 / slotsPerHour))
                                        appstart = appstart.AddMinutes(60 / slotsPerHour);
                                    if (appointment.EndDate.Minute > (60 / slotsPerHour))
                                        append = append.AddMinutes(60 / slotsPerHour);
                                }

                                // This is basically the same as previous mode, but for all appts
                                else if (AppHeightMode == AppHeightDrawMode.FullHalfHourBlocksAll)
                                {
                                    appstart = appointment.StartDate.AddMinutes(-appointment.StartDate.Minute);
                                    if (appointment.EndDate.Minute != 0 && appointment.EndDate.Minute != (60 / slotsPerHour))
                                        append = appointment.EndDate.AddMinutes((60 / slotsPerHour) - appointment.EndDate.Minute);
                                    else
                                        append = appointment.EndDate;

                                    if (appointment.StartDate.Minute >= (60 / slotsPerHour))
                                        appstart = appstart.AddMinutes(60 / slotsPerHour);
                                    if (appointment.EndDate.Minute > (60 / slotsPerHour))
                                        append = append.AddMinutes(60 / slotsPerHour);
                                }

                                // Based on previous code
                                else if (AppHeightMode == AppHeightDrawMode.EndHalfHourBlocksShort && appointment.EndDate.Subtract(appointment.StartDate).TotalMinutes < (60 / slotsPerHour))
                                {
                                    // Round the end time to the next halfhour
                                    append = appointment.EndDate.AddMinutes((60 / slotsPerHour) - appointment.EndDate.Minute);

                                    // Make sure we've rounded it to the correct halfhour :)
                                    if (appointment.EndDate.Minute > (60 / slotsPerHour))
                                        append = append.AddMinutes(60 / slotsPerHour);
                                }

                                else if (AppHeightMode == AppHeightDrawMode.EndHalfHourBlocksAll)
                                {
                                    // Round the end time to the next halfhour
                                    if (appointment.EndDate.Minute != 0 && appointment.EndDate.Minute != (60 / slotsPerHour))
                                        append = appointment.EndDate.AddMinutes((60 / slotsPerHour) - appointment.EndDate.Minute);
                                    else
                                        append = appointment.EndDate;
                                    // Make sure we've rounded it to the correct halfhour :)
                                    if (appointment.EndDate.Minute > (60 / slotsPerHour))
                                        append = append.AddMinutes(60 / slotsPerHour);
                                }

                                appRect = GetHourRangeRectangle(appstart, append, appRect);

                                view = new AppointmentView();
                                view.Rectangle = appRect;
                                view.Appointment = appointment;

                                appointmentViews[appointment] = view;

                                e.Graphics.SetClip(rect);

                                if (DrawAllAppointmentBorders)
                                    appointment.DrawBorder = true;

                                // Procedure for gripper rectangle is always the same
                                Rectangle gripRect = GetHourRangeRectangle(appointment.StartDate, appointment.EndDate, appRect);
                                gripRect.Width = appointmentGripWidth;

                                renderer.DrawAppointment(e.Graphics, appRect, appointment, appointment == SelectedAppointment, gripRect);

                                e.Graphics.ResetClip();

                                drawnItems.Add(appointment);
                            }
                        }
                    }
                }
            }
        }

        private static AppointmentLayout[] GetMaxParalelAppointments(List<Appointment> appointments, int slotsPerHour)
        {
            int maxAppointments = appointments.Count > MAX_APPOINTMENTS_CONFLICTS ? MAX_APPOINTMENTS_CONFLICTS : appointments.Count;
            AppointmentLayout[] Result = new AppointmentLayout[24 * slotsPerHour];

            foreach (Appointment appointment in appointments)
            {
                appointment.conflictCount = 1;
            }

            foreach (Appointment appointment in appointments)
            {
                int startBlock = (appointment.StartDate.Hour * slotsPerHour) + (appointment.StartDate.Minute / (60 / slotsPerHour));
                int endBlock = (appointment.EndDate.Hour * slotsPerHour) + (appointment.EndDate.Minute / (60 / slotsPerHour));

                // Added to allow small parts been displayed
                if (endBlock == startBlock)
                {
                    if (endBlock < 24 * slotsPerHour)
                        endBlock++;
                    else
                        startBlock--;
                }

                for (int timeSlot = startBlock; timeSlot < endBlock; timeSlot++)
                {
                    AppointmentLayout layout = Result[timeSlot];

                    if (layout == null)
                    {
                        layout = new AppointmentLayout();
                        layout.Appointments = new Appointment[maxAppointments];
                        Result[timeSlot] = layout;
                    }

                    if (layout.Count >= maxAppointments)
                        break;

                    layout.Appointments[layout.Count] = appointment;

                    layout.Count++;

                    List<string> groups = new List<string>();

                    foreach (Appointment app2 in layout.Appointments)
                    {
                        if ((app2 != null) && (!groups.Contains(app2.Group)))
                            groups.Add(app2.Group);
                    }

                    layout.Groups = groups;

                    // update conflicts
                    foreach (Appointment app2 in layout.Appointments)
                    {
                        if ((app2 != null) && (app2.Group == appointment.Group))
                            if (app2.conflictCount < layout.Count)
                                app2.conflictCount = layout.Count - (layout.Groups.Count - 1);
                    }
                }
            }

            // first pass get the maximum conflicts for an appointment
            foreach (Appointment appt in appointments)
            {
                if (appt == null)
                    continue;

                int maxConflicts = appt.conflictCount;

                foreach (AppointmentLayout lo in Result)
                {
                    if (lo == null)
                        continue;

                    if (lo.Contains(appt))
                    {
                        foreach (Appointment ap in lo.Appointments)
                        {
                            if (ap != null && ap != appt && ap.conflictCount < maxConflicts)
                                ap.conflictCount = maxConflicts;
                        }
                    }
                }
            }

            return Result;
        }

        private void DrawDays(PaintEventArgs e, Rectangle rect)
        {
            TeamViewCountEventArgs multiCount = new TeamViewCountEventArgs(1);
            RaiseMultiCount(multiCount);

            //if there is nothing to view?
            int dayWidth = rect.Width / (multiCount.Count == 0 ? 1 : multiCount.Count);

            AppointmentList longAppointments = (AppointmentList)cachedAppointments[-1];

            AppointmentList drawnLongApps = new AppointmentList();

            AppointmentView view;

            int y = dayHeadersHeight;
            bool intersect = false;

            List<int> layers = new List<int>();

            if (longAppointments != null)
            {
                foreach (Appointment appointment in longAppointments)
                {
                    appointment.Layer = 0;

                    if (drawnLongApps.Count != 0)
                    {
                        foreach (Appointment app in drawnLongApps)
                            if (!layers.Contains(app.Layer))
                                layers.Add(app.Layer);

                        foreach (int lay in layers)
                        {
                            foreach (Appointment app in drawnLongApps)
                            {
                                if (app.Layer == lay)
                                {
                                    if (app.AllDayEvent && appointment.AllDayEvent)
                                    {
                                        intersect = DateWithin(appointment.StartDate, appointment.EndDate, app.StartDate, app.EndDate);
                                    }
                                    else
                                    {
                                        if (appointment.StartDate.Date >= app.EndDate.Date || appointment.EndDate.Date <= app.StartDate.Date)
                                            intersect = false;
                                        else
                                        {
                                            intersect = true;
                                            break;
                                        }
                                    }
                                }

                                appointment.Layer = lay;
                            }

                            if (!intersect)
                                break;
                        }

                        if (intersect)
                            appointment.Layer = layers.Count;
                    }

                    drawnLongApps.Add(appointment); // changed by Gimlei
                }

                foreach (Appointment app in drawnLongApps)
                    if (!layers.Contains(app.Layer))
                        layers.Add(app.Layer);

                allDayEventsHeaderHeight = (layers.Count * (horizontalAppointmentHeight + 5)) + 5;

                Rectangle backRectangle = rect;
                backRectangle.Y = y;
                backRectangle.Height = allDayEventsHeaderHeight;

                renderer.DrawAllDayBackground(e.Graphics, backRectangle);

                foreach (Appointment appointment in longAppointments)
                {
                    Rectangle appointmenRect = rect;
                    int spanDays = appointment.EndDate.Subtract(appointment.StartDate).Days;

                    //spanDays must be at least 1 day otherwise it won't show
                    if (spanDays == 0)
                        ++spanDays;

                    if ((appointment.EndDate.Day != appointment.StartDate.Day && appointment.EndDate.TimeOfDay < appointment.StartDate.TimeOfDay)
                        || (appointment.AllDayEvent && appointment.EndDate.Date != appointment.StartDate.Date && appointment.StartDate >= startDate))
                        ++spanDays;

                    appointmenRect.Height = horizontalAppointmentHeight;

                    if (ViewType == DayViewType.SingleView)
                    {
                        appointmenRect.Width = (dayWidth * spanDays) - 5;
                        appointmenRect.X += appointment.StartDate.Subtract(startDate).Days * dayWidth; // changed by Gimlei
                    }
                    else
                    {
                        appointmenRect.Width = (dayWidth * multiCount.Count) - 5;
                        appointmenRect.X += appointment.StartDate.Subtract(startDate).Days * dayWidth;
                    }

                    if (appointmenRect.X < 0)
                        appointmenRect.X = 53;

                    appointmenRect.Y = y + (appointment.Layer * (horizontalAppointmentHeight + 5)) + 5; // changed by Gimlei

                    view = new AppointmentView();
                    view.Rectangle = appointmenRect;
                    view.Appointment = appointment;

                    longappointmentViews[appointment] = view;

                    Rectangle gripRect = appointmenRect;
                    gripRect.Width = appointmentGripWidth;

                    renderer.DrawAppointment(e.Graphics, appointmenRect, appointment, appointment == SelectedAppointment, gripRect);
                }
            }

            DateTime time = startDate;
            Rectangle rectangle = rect;
            rectangle.Width = dayWidth;
            rectangle.Y += allDayEventsHeaderHeight;
            rectangle.Height -= allDayEventsHeaderHeight;

            appointmentViews.Clear();
            layers.Clear();

            for (int day = 0; day < multiCount.Count; day++)
            {
                if (day == 0)
                    rectangle.Width += (rect.Width % multiCount.Count) - 1;

                DrawDay(e, rectangle, time, day);

                rectangle.X += dayWidth;

                if (ViewType == DayViewType.SingleView)
                    time = time.AddDays(1);
            }
        }

        #endregion

        #region Internal Utility Classes

        internal class AppointmentLayout
        {
            public int Count { get; set; }
            public List<string> Groups;
            public Appointment[] Appointments;

            public bool Contains(Appointment appointment)
            {
                foreach (Appointment appt in Appointments)
                {
                    if (appt == appointment)
                        return true;
                }

                return false;
            }

#if DEBUG
            public override string ToString()
            {
                return String.Format("AppointmentLayout, Count = {0}", Count);
            }
#endif
        }

        internal class AppointmentView
        {
            public Appointment Appointment;
            public Rectangle Rectangle;
        }


        #endregion

        #region Events

        [Description("Event raised when appointment selected/deselected.")]
        [Category("Appointments")]
        public event AppointmentSelectedEventHandler AppointmentSelected;

        [Description("Event raised when the selection has changed.")]
        [Category("Appointments")]
        public event EventHandler SelectionChanged;

        [Description("Event raised when layout has changed and appointments for current view need to be retrieved.")]
        [Category("Appointments")]
        public event ResolveAppointmentsEventHandler ResolveAppointments;

        [Description("Event raised when a new appointment is created.")]
        [Category("Appointments")]
        public event NewAppointmentEventHandler NewAppointment;

        [Description("Event raised when an appointment is Edited directly in the calendar.")]
        [Category("Appointments")]
        public event EventHandler<AppointmentEventArgs> AppointmentUpdated;

        [Description("Event raised whilst an appointment is being moved.")]
        [Category("Appointments")]
        public event EventHandler<AppointmentEventArgs> AppointmentMove;

        [Description("Event reaised prior to appointment being moved.")]
        [Category("Appointments")]
        public event BeforeMoveAppointmentEventHandler BeforeAppointmentMove;

        [Description("Event reaised after an appointment has been moved.")]
        [Category("Appointments")]
        public event EventHandler<AppointmentEventArgs> AppointmentMoved;

        [Description("Event raised to determine the column count when ViewType is TeamView.")]
        [Category("Team View")]
        public event MultiCountEventHandler MultiCount;

        [Description("Event raised to retrieve the Header Text when ViewType is TeamView.")]
        [Category("Team View")]
        public event MultiGetEventHandler MultiHeader;

        [Description("Event raised prior to showing a context menu.")]
        [Category("Behaviour")]
        public event ContextMenuEventHandler BeforeContextMenuShow;

        [Description("Event raised when a tooltip should be shown.")]
        [Category("Tooltip")]
        public event TooltipEventHandler ToolTipShow;

        [Description("Event raised to retrieve working hours.")]
        [Category("Working Hours")]
        public event WorkingHoursEventHandler WorkingHours;

        [Description("Event raised after an appointment has been drawn, allows owner drawing to occur.")]
        [Category("Drawing")]
        public event AfterDrawAppointmentEventHandler AfterDrawAppointment;

        [Description("Event raised after an appointment header has been drawn, allows owner drawing to occur.")]
        [Category("Drawing")]
        public event AfterDrawHeaderEventHandler AfterDrawHeader;

        [Description("Event raised when user double clicks the column header.")]
        [Category("Header")]
        public event HeaderClickEventHandler HeaderClicked;

        #endregion Events

        #region Timer / Tooltip

        private void timerTooltip_Tick(object sender, EventArgs e)
        {
            _timerTooltip.Enabled = false;
            _moveStart = false;

            if (!RectangleToScreen(ClientRectangle).Contains(Cursor.Position))
                return;

            Point currPos = PointToClient(MousePosition);
            Appointment appt = GetAppointmentAt(currPos.X, currPos.Y);
            int Column = -1;
            TooltipEventArgs args = null;
            DateTime CurrentDateTime = DateTime.Now;

            if (GetMouseOverHeader(ref Column, ref CurrentDateTime))
            {
                args = new TooltipEventArgs(null, CurrentDateTime, Column, true);
            }
            else
            {
                CurrentDateTime = GetTimeAt(currPos.X, currPos.Y, ref Column);
                args = new TooltipEventArgs(appt, CurrentDateTime, Column, false);
            }

            if (Visible && (Column > -1))
                RaiseTooltip(args);

            if (!args.AllowShow)
                return;

            currPos.X += 5;
            currPos.Y += 5;
            _tooltip.ToolTipIcon = args.Icon;
            _tooltip.IsBalloon = args.ShowBalloon;
            _tooltip.ToolTipTitle = args.Title;
            _tooltip.UseAnimation = false;
            _tooltip.Show(args.Text, this, currPos);
        }

        #endregion Timer / Tooltip
    }
}
