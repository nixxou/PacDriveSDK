namespace PacDriveDemo
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lvwDevices = new System.Windows.Forms.ListView();
            this.colDeviceType = new System.Windows.Forms.ColumnHeader();
            this.colVendorId = new System.Windows.Forms.ColumnHeader();
            this.colProductId = new System.Windows.Forms.ColumnHeader();
            this.colVersionNumber = new System.Windows.Forms.ColumnHeader();
            this.colVendorName = new System.Windows.Forms.ColumnHeader();
            this.colProductName = new System.Windows.Forms.ColumnHeader();
            this.colSerialNumber = new System.Windows.Forms.ColumnHeader();
            this.colDevicePath = new System.Windows.Forms.ColumnHeader();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.butAllLEDsOff = new System.Windows.Forms.Button();
            this.pnlLEDButtons = new System.Windows.Forms.Panel();
            this.butAllLEDsOn = new System.Windows.Forms.Button();
            this.butAllLEDsRandom = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rdoSeconds_0_5 = new System.Windows.Forms.RadioButton();
            this.rdoSeconds_1 = new System.Windows.Forms.RadioButton();
            this.rdoSeconds_2 = new System.Windows.Forms.RadioButton();
            this.lblFlashSpeed = new System.Windows.Forms.Label();
            this.rdoAlwaysOn = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.trkIntensity = new System.Windows.Forms.TrackBar();
            this.nudLEDNumber = new System.Windows.Forms.NumericUpDown();
            this.lblLEDNumber = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.butProgramUHID = new System.Windows.Forms.Button();
            this.lblScriptStepDelay = new System.Windows.Forms.Label();
            this.lblFadeTime = new System.Windows.Forms.Label();
            this.trkScriptStepDelay = new System.Windows.Forms.TrackBar();
            this.trkFadeTime = new System.Windows.Forms.TrackBar();
            this.butRunScript = new System.Windows.Forms.Button();
            this.butClearFlash = new System.Windows.Forms.Button();
            this.butStopRecording = new System.Windows.Forms.Button();
            this.butStartRecording = new System.Windows.Forms.Button();
            this.butSetDeviceID = new System.Windows.Forms.Button();
            this.lblDeviceId = new System.Windows.Forms.Label();
            this.nudDeviceId = new System.Windows.Forms.NumericUpDown();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lblState = new System.Windows.Forms.Label();
            this.butGetState = new System.Windows.Forms.Button();
            this.lblReleased = new System.Windows.Forms.Label();
            this.butReleased = new System.Windows.Forms.Button();
            this.lblPressed = new System.Windows.Forms.Label();
            this.butPressed = new System.Windows.Forms.Button();
            this.butSetTemporary = new System.Windows.Forms.Button();
            this.butSetPermanent = new System.Windows.Forms.Button();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkIntensity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLEDNumber)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkScriptStepDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkFadeTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDeviceId)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lvwDevices);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(481, 359);
            this.splitContainer1.SplitterDistance = 107;
            this.splitContainer1.TabIndex = 5;
            // 
            // lvwDevices
            // 
            this.lvwDevices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDeviceType,
            this.colVendorId,
            this.colProductId,
            this.colVersionNumber,
            this.colVendorName,
            this.colProductName,
            this.colSerialNumber,
            this.colDevicePath});
            this.lvwDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwDevices.FullRowSelect = true;
            this.lvwDevices.GridLines = true;
            this.lvwDevices.HideSelection = false;
            this.lvwDevices.Location = new System.Drawing.Point(0, 0);
            this.lvwDevices.Name = "lvwDevices";
            this.lvwDevices.Size = new System.Drawing.Size(481, 107);
            this.lvwDevices.TabIndex = 2;
            this.lvwDevices.UseCompatibleStateImageBehavior = false;
            this.lvwDevices.View = System.Windows.Forms.View.Details;
            // 
            // colDeviceType
            // 
            this.colDeviceType.Text = "Device Type";
            this.colDeviceType.Width = 83;
            // 
            // colVendorId
            // 
            this.colVendorId.Text = "Vendor Id";
            this.colVendorId.Width = 100;
            // 
            // colProductId
            // 
            this.colProductId.Text = "Product Id";
            this.colProductId.Width = 100;
            // 
            // colVersionNumber
            // 
            this.colVersionNumber.Text = "Version Number";
            this.colVersionNumber.Width = 100;
            // 
            // colVendorName
            // 
            this.colVendorName.Text = "Vendor Name";
            this.colVendorName.Width = 100;
            // 
            // colProductName
            // 
            this.colProductName.Text = "Product Name";
            this.colProductName.Width = 100;
            // 
            // colSerialNumber
            // 
            this.colSerialNumber.Text = "Serial Number";
            this.colSerialNumber.Width = 100;
            // 
            // colDevicePath
            // 
            this.colDevicePath.Text = "Device Path";
            this.colDevicePath.Width = 100;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(481, 248);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.butAllLEDsOff);
            this.tabPage1.Controls.Add(this.pnlLEDButtons);
            this.tabPage1.Controls.Add(this.butAllLEDsOn);
            this.tabPage1.Controls.Add(this.butAllLEDsRandom);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(473, 222);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "State";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // butAllLEDsOff
            // 
            this.butAllLEDsOff.Location = new System.Drawing.Point(128, 13);
            this.butAllLEDsOff.Name = "butAllLEDsOff";
            this.butAllLEDsOff.Size = new System.Drawing.Size(106, 32);
            this.butAllLEDsOff.TabIndex = 27;
            this.butAllLEDsOff.Text = "All LEDs Off";
            this.butAllLEDsOff.UseVisualStyleBackColor = true;
            this.butAllLEDsOff.Click += new System.EventHandler(this.butAllLEDsOff_Click);
            // 
            // pnlLEDButtons
            // 
            this.pnlLEDButtons.Location = new System.Drawing.Point(8, 51);
            this.pnlLEDButtons.Name = "pnlLEDButtons";
            this.pnlLEDButtons.Size = new System.Drawing.Size(457, 163);
            this.pnlLEDButtons.TabIndex = 0;
            // 
            // butAllLEDsOn
            // 
            this.butAllLEDsOn.Location = new System.Drawing.Point(16, 13);
            this.butAllLEDsOn.Name = "butAllLEDsOn";
            this.butAllLEDsOn.Size = new System.Drawing.Size(106, 32);
            this.butAllLEDsOn.TabIndex = 26;
            this.butAllLEDsOn.Text = "All LEDs On";
            this.butAllLEDsOn.UseVisualStyleBackColor = true;
            this.butAllLEDsOn.Click += new System.EventHandler(this.butAllLEDsOn_Click);
            // 
            // butAllLEDsRandom
            // 
            this.butAllLEDsRandom.Location = new System.Drawing.Point(240, 13);
            this.butAllLEDsRandom.Name = "butAllLEDsRandom";
            this.butAllLEDsRandom.Size = new System.Drawing.Size(215, 32);
            this.butAllLEDsRandom.TabIndex = 25;
            this.butAllLEDsRandom.Text = "All LEDs Random";
            this.butAllLEDsRandom.UseVisualStyleBackColor = true;
            this.butAllLEDsRandom.Click += new System.EventHandler(this.butAllLEDsRandom_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.rdoSeconds_0_5);
            this.tabPage2.Controls.Add(this.rdoSeconds_1);
            this.tabPage2.Controls.Add(this.rdoSeconds_2);
            this.tabPage2.Controls.Add(this.lblFlashSpeed);
            this.tabPage2.Controls.Add(this.rdoAlwaysOn);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.trkIntensity);
            this.tabPage2.Controls.Add(this.nudLEDNumber);
            this.tabPage2.Controls.Add(this.lblLEDNumber);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(473, 222);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Intensity";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // rdoSeconds_0_5
            // 
            this.rdoSeconds_0_5.AutoSize = true;
            this.rdoSeconds_0_5.Location = new System.Drawing.Point(207, 200);
            this.rdoSeconds_0_5.Name = "rdoSeconds_0_5";
            this.rdoSeconds_0_5.Size = new System.Drawing.Size(85, 17);
            this.rdoSeconds_0_5.TabIndex = 20;
            this.rdoSeconds_0_5.Text = "0.5 Seconds";
            this.rdoSeconds_0_5.UseVisualStyleBackColor = true;
            this.rdoSeconds_0_5.CheckedChanged += new System.EventHandler(this.rdoFlashSpeed_CheckedChanged);
            // 
            // rdoSeconds_1
            // 
            this.rdoSeconds_1.AutoSize = true;
            this.rdoSeconds_1.Location = new System.Drawing.Point(207, 177);
            this.rdoSeconds_1.Name = "rdoSeconds_1";
            this.rdoSeconds_1.Size = new System.Drawing.Size(71, 17);
            this.rdoSeconds_1.TabIndex = 19;
            this.rdoSeconds_1.Text = "1 Second";
            this.rdoSeconds_1.UseVisualStyleBackColor = true;
            this.rdoSeconds_1.CheckedChanged += new System.EventHandler(this.rdoFlashSpeed_CheckedChanged);
            // 
            // rdoSeconds_2
            // 
            this.rdoSeconds_2.AutoSize = true;
            this.rdoSeconds_2.Location = new System.Drawing.Point(207, 154);
            this.rdoSeconds_2.Name = "rdoSeconds_2";
            this.rdoSeconds_2.Size = new System.Drawing.Size(76, 17);
            this.rdoSeconds_2.TabIndex = 18;
            this.rdoSeconds_2.Text = "2 Seconds";
            this.rdoSeconds_2.UseVisualStyleBackColor = true;
            this.rdoSeconds_2.CheckedChanged += new System.EventHandler(this.rdoFlashSpeed_CheckedChanged);
            // 
            // lblFlashSpeed
            // 
            this.lblFlashSpeed.AutoSize = true;
            this.lblFlashSpeed.Location = new System.Drawing.Point(122, 133);
            this.lblFlashSpeed.Name = "lblFlashSpeed";
            this.lblFlashSpeed.Size = new System.Drawing.Size(69, 13);
            this.lblFlashSpeed.TabIndex = 17;
            this.lblFlashSpeed.Text = "Flash Speed:";
            // 
            // rdoAlwaysOn
            // 
            this.rdoAlwaysOn.AutoSize = true;
            this.rdoAlwaysOn.Checked = true;
            this.rdoAlwaysOn.Location = new System.Drawing.Point(207, 131);
            this.rdoAlwaysOn.Name = "rdoAlwaysOn";
            this.rdoAlwaysOn.Size = new System.Drawing.Size(75, 17);
            this.rdoAlwaysOn.TabIndex = 16;
            this.rdoAlwaysOn.TabStop = true;
            this.rdoAlwaysOn.Text = "Always On";
            this.rdoAlwaysOn.UseVisualStyleBackColor = true;
            this.rdoAlwaysOn.CheckedChanged += new System.EventHandler(this.rdoFlashSpeed_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Intensity:";
            // 
            // trkIntensity
            // 
            this.trkIntensity.AutoSize = false;
            this.trkIntensity.Location = new System.Drawing.Point(99, 80);
            this.trkIntensity.Maximum = 255;
            this.trkIntensity.Name = "trkIntensity";
            this.trkIntensity.Size = new System.Drawing.Size(330, 31);
            this.trkIntensity.TabIndex = 13;
            this.trkIntensity.TickFrequency = 4;
            this.trkIntensity.Value = 255;
            this.trkIntensity.Scroll += new System.EventHandler(this.trkIntensity_Scroll);
            // 
            // nudLEDNumber
            // 
            this.nudLEDNumber.Location = new System.Drawing.Point(250, 34);
            this.nudLEDNumber.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.nudLEDNumber.Name = "nudLEDNumber";
            this.nudLEDNumber.ReadOnly = true;
            this.nudLEDNumber.Size = new System.Drawing.Size(71, 20);
            this.nudLEDNumber.TabIndex = 14;
            // 
            // lblLEDNumber
            // 
            this.lblLEDNumber.AutoSize = true;
            this.lblLEDNumber.Location = new System.Drawing.Point(126, 36);
            this.lblLEDNumber.Name = "lblLEDNumber";
            this.lblLEDNumber.Size = new System.Drawing.Size(109, 13);
            this.lblLEDNumber.TabIndex = 15;
            this.lblLEDNumber.Text = "LED Number (0 = All):";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.butProgramUHID);
            this.tabPage3.Controls.Add(this.lblScriptStepDelay);
            this.tabPage3.Controls.Add(this.lblFadeTime);
            this.tabPage3.Controls.Add(this.trkScriptStepDelay);
            this.tabPage3.Controls.Add(this.trkFadeTime);
            this.tabPage3.Controls.Add(this.butRunScript);
            this.tabPage3.Controls.Add(this.butClearFlash);
            this.tabPage3.Controls.Add(this.butStopRecording);
            this.tabPage3.Controls.Add(this.butStartRecording);
            this.tabPage3.Controls.Add(this.butSetDeviceID);
            this.tabPage3.Controls.Add(this.lblDeviceId);
            this.tabPage3.Controls.Add(this.nudDeviceId);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(473, 222);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Settings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // butProgramUHID
            // 
            this.butProgramUHID.Location = new System.Drawing.Point(302, 171);
            this.butProgramUHID.Name = "butProgramUHID";
            this.butProgramUHID.Size = new System.Drawing.Size(140, 33);
            this.butProgramUHID.TabIndex = 34;
            this.butProgramUHID.Text = "Program UHID";
            this.butProgramUHID.UseVisualStyleBackColor = true;
            this.butProgramUHID.Click += new System.EventHandler(this.butProgramUHID_Click);
            // 
            // lblScriptStepDelay
            // 
            this.lblScriptStepDelay.AutoSize = true;
            this.lblScriptStepDelay.Location = new System.Drawing.Point(80, 148);
            this.lblScriptStepDelay.Name = "lblScriptStepDelay";
            this.lblScriptStepDelay.Size = new System.Drawing.Size(89, 13);
            this.lblScriptStepDelay.TabIndex = 33;
            this.lblScriptStepDelay.Text = "Script Step Delay";
            // 
            // lblFadeTime
            // 
            this.lblFadeTime.AutoSize = true;
            this.lblFadeTime.Location = new System.Drawing.Point(92, 203);
            this.lblFadeTime.Name = "lblFadeTime";
            this.lblFadeTime.Size = new System.Drawing.Size(57, 13);
            this.lblFadeTime.TabIndex = 32;
            this.lblFadeTime.Text = "Fade Time";
            // 
            // trkScriptStepDelay
            // 
            this.trkScriptStepDelay.AutoSize = false;
            this.trkScriptStepDelay.Location = new System.Drawing.Point(20, 116);
            this.trkScriptStepDelay.Maximum = 255;
            this.trkScriptStepDelay.Name = "trkScriptStepDelay";
            this.trkScriptStepDelay.Size = new System.Drawing.Size(198, 29);
            this.trkScriptStepDelay.TabIndex = 28;
            this.trkScriptStepDelay.TickFrequency = 8;
            this.trkScriptStepDelay.Scroll += new System.EventHandler(this.trkScriptStepDelay_Scroll);
            // 
            // trkFadeTime
            // 
            this.trkFadeTime.AutoSize = false;
            this.trkFadeTime.Location = new System.Drawing.Point(21, 171);
            this.trkFadeTime.Maximum = 255;
            this.trkFadeTime.Name = "trkFadeTime";
            this.trkFadeTime.Size = new System.Drawing.Size(197, 29);
            this.trkFadeTime.TabIndex = 27;
            this.trkFadeTime.TickFrequency = 8;
            this.trkFadeTime.Scroll += new System.EventHandler(this.trkFadeTime_Scroll);
            // 
            // butRunScript
            // 
            this.butRunScript.Location = new System.Drawing.Point(131, 79);
            this.butRunScript.Name = "butRunScript";
            this.butRunScript.Size = new System.Drawing.Size(87, 23);
            this.butRunScript.TabIndex = 26;
            this.butRunScript.Text = "RunScript";
            this.butRunScript.UseVisualStyleBackColor = true;
            this.butRunScript.Click += new System.EventHandler(this.butRunScript_Click);
            // 
            // butClearFlash
            // 
            this.butClearFlash.Location = new System.Drawing.Point(20, 79);
            this.butClearFlash.Name = "butClearFlash";
            this.butClearFlash.Size = new System.Drawing.Size(105, 23);
            this.butClearFlash.TabIndex = 25;
            this.butClearFlash.Text = "Clear Flash";
            this.butClearFlash.UseVisualStyleBackColor = true;
            this.butClearFlash.Click += new System.EventHandler(this.butClearFlash_Click);
            // 
            // butStopRecording
            // 
            this.butStopRecording.Location = new System.Drawing.Point(20, 49);
            this.butStopRecording.Name = "butStopRecording";
            this.butStopRecording.Size = new System.Drawing.Size(198, 24);
            this.butStopRecording.TabIndex = 24;
            this.butStopRecording.Text = "Stop Recording Script to Flash";
            this.butStopRecording.UseVisualStyleBackColor = true;
            this.butStopRecording.Click += new System.EventHandler(this.butStopRecording_Click);
            // 
            // butStartRecording
            // 
            this.butStartRecording.Location = new System.Drawing.Point(20, 20);
            this.butStartRecording.Name = "butStartRecording";
            this.butStartRecording.Size = new System.Drawing.Size(198, 23);
            this.butStartRecording.TabIndex = 23;
            this.butStartRecording.Text = "Start Recording Script to Flash";
            this.butStartRecording.UseVisualStyleBackColor = true;
            this.butStartRecording.Click += new System.EventHandler(this.butStartRecording_Click);
            // 
            // butSetDeviceID
            // 
            this.butSetDeviceID.Location = new System.Drawing.Point(303, 49);
            this.butSetDeviceID.Name = "butSetDeviceID";
            this.butSetDeviceID.Size = new System.Drawing.Size(140, 33);
            this.butSetDeviceID.TabIndex = 21;
            this.butSetDeviceID.Text = "Set Device ID";
            this.butSetDeviceID.UseVisualStyleBackColor = true;
            this.butSetDeviceID.Click += new System.EventHandler(this.butSetDeviceID_Click);
            // 
            // lblDeviceId
            // 
            this.lblDeviceId.AutoSize = true;
            this.lblDeviceId.Location = new System.Drawing.Point(300, 25);
            this.lblDeviceId.Name = "lblDeviceId";
            this.lblDeviceId.Size = new System.Drawing.Size(56, 13);
            this.lblDeviceId.TabIndex = 20;
            this.lblDeviceId.Text = "Device Id:";
            // 
            // nudDeviceId
            // 
            this.nudDeviceId.Location = new System.Drawing.Point(371, 23);
            this.nudDeviceId.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nudDeviceId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDeviceId.Name = "nudDeviceId";
            this.nudDeviceId.ReadOnly = true;
            this.nudDeviceId.Size = new System.Drawing.Size(71, 20);
            this.nudDeviceId.TabIndex = 19;
            this.nudDeviceId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lblState);
            this.tabPage4.Controls.Add(this.butGetState);
            this.tabPage4.Controls.Add(this.lblReleased);
            this.tabPage4.Controls.Add(this.butReleased);
            this.tabPage4.Controls.Add(this.lblPressed);
            this.tabPage4.Controls.Add(this.butPressed);
            this.tabPage4.Controls.Add(this.butSetTemporary);
            this.tabPage4.Controls.Add(this.butSetPermanent);
            this.tabPage4.Controls.Add(this.txtUrl);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(473, 222);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "USB Button";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(366, 82);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(52, 13);
            this.lblState.TabIndex = 8;
            this.lblState.Text = "Released";
            // 
            // butGetState
            // 
            this.butGetState.Location = new System.Drawing.Point(285, 77);
            this.butGetState.Name = "butGetState";
            this.butGetState.Size = new System.Drawing.Size(75, 23);
            this.butGetState.TabIndex = 7;
            this.butGetState.Text = "Get State";
            this.butGetState.UseVisualStyleBackColor = true;
            this.butGetState.Click += new System.EventHandler(this.butGetState_Click);
            // 
            // lblReleased
            // 
            this.lblReleased.AutoSize = true;
            this.lblReleased.Location = new System.Drawing.Point(207, 42);
            this.lblReleased.Name = "lblReleased";
            this.lblReleased.Size = new System.Drawing.Size(52, 13);
            this.lblReleased.TabIndex = 6;
            this.lblReleased.Text = "Released";
            // 
            // butReleased
            // 
            this.butReleased.BackColor = System.Drawing.Color.Red;
            this.butReleased.Location = new System.Drawing.Point(211, 58);
            this.butReleased.Name = "butReleased";
            this.butReleased.Size = new System.Drawing.Size(43, 42);
            this.butReleased.TabIndex = 5;
            this.butReleased.UseVisualStyleBackColor = false;
            this.butReleased.Click += new System.EventHandler(this.butReleased_Click);
            // 
            // lblPressed
            // 
            this.lblPressed.AutoSize = true;
            this.lblPressed.Location = new System.Drawing.Point(78, 42);
            this.lblPressed.Name = "lblPressed";
            this.lblPressed.Size = new System.Drawing.Size(45, 13);
            this.lblPressed.TabIndex = 4;
            this.lblPressed.Text = "Pressed";
            // 
            // butPressed
            // 
            this.butPressed.BackColor = System.Drawing.Color.Lime;
            this.butPressed.Location = new System.Drawing.Point(78, 58);
            this.butPressed.Name = "butPressed";
            this.butPressed.Size = new System.Drawing.Size(43, 42);
            this.butPressed.TabIndex = 3;
            this.butPressed.UseVisualStyleBackColor = false;
            this.butPressed.Click += new System.EventHandler(this.butPressed_Click);
            // 
            // butSetTemporary
            // 
            this.butSetTemporary.Location = new System.Drawing.Point(263, 178);
            this.butSetTemporary.Name = "butSetTemporary";
            this.butSetTemporary.Size = new System.Drawing.Size(126, 29);
            this.butSetTemporary.TabIndex = 2;
            this.butSetTemporary.Text = "Set Temporary";
            this.butSetTemporary.UseVisualStyleBackColor = true;
            this.butSetTemporary.Click += new System.EventHandler(this.butSetTemporary_Click);
            // 
            // butSetPermanent
            // 
            this.butSetPermanent.Location = new System.Drawing.Point(78, 178);
            this.butSetPermanent.Name = "butSetPermanent";
            this.butSetPermanent.Size = new System.Drawing.Size(126, 29);
            this.butSetPermanent.TabIndex = 1;
            this.butSetPermanent.Text = "Set Permanent";
            this.butSetPermanent.UseVisualStyleBackColor = true;
            this.butSetPermanent.Click += new System.EventHandler(this.butSetPermanent_Click);
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(40, 128);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(394, 20);
            this.txtUrl.TabIndex = 0;
            this.txtUrl.Text = "This is a test";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 359);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PacDrive Demo";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkIntensity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLEDNumber)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkScriptStepDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkFadeTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDeviceId)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView lvwDevices;
        private System.Windows.Forms.ColumnHeader colVendorId;
        private System.Windows.Forms.ColumnHeader colProductId;
        private System.Windows.Forms.ColumnHeader colVersionNumber;
        private System.Windows.Forms.ColumnHeader colVendorName;
        private System.Windows.Forms.ColumnHeader colProductName;
        private System.Windows.Forms.ColumnHeader colSerialNumber;
        private System.Windows.Forms.ColumnHeader colDevicePath;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label lblLEDNumber;
        private System.Windows.Forms.NumericUpDown nudLEDNumber;
        private System.Windows.Forms.TrackBar trkIntensity;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlLEDButtons;
        private System.Windows.Forms.TabPage tabPage3;
        internal System.Windows.Forms.TrackBar trkScriptStepDelay;
        internal System.Windows.Forms.TrackBar trkFadeTime;
        internal System.Windows.Forms.Button butRunScript;
        internal System.Windows.Forms.Button butClearFlash;
        internal System.Windows.Forms.Button butStopRecording;
        internal System.Windows.Forms.Button butStartRecording;
        private System.Windows.Forms.Button butSetDeviceID;
        private System.Windows.Forms.Label lblDeviceId;
        private System.Windows.Forms.NumericUpDown nudDeviceId;
        private System.Windows.Forms.Label lblScriptStepDelay;
        private System.Windows.Forms.Label lblFadeTime;
        private System.Windows.Forms.Button butProgramUHID;
        private System.Windows.Forms.ColumnHeader colDeviceType;
        private System.Windows.Forms.RadioButton rdoSeconds_0_5;
        private System.Windows.Forms.RadioButton rdoSeconds_1;
        private System.Windows.Forms.RadioButton rdoSeconds_2;
        private System.Windows.Forms.Label lblFlashSpeed;
        private System.Windows.Forms.RadioButton rdoAlwaysOn;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button butSetTemporary;
        private System.Windows.Forms.Button butSetPermanent;
        private System.Windows.Forms.Label lblReleased;
        private System.Windows.Forms.Button butReleased;
        private System.Windows.Forms.Label lblPressed;
        private System.Windows.Forms.Button butPressed;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Button butGetState;
		private System.Windows.Forms.Button butAllLEDsOff;
		private System.Windows.Forms.Button butAllLEDsOn;
		private System.Windows.Forms.Button butAllLEDsRandom;

    }
}

