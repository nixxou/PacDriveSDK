using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PacDriveDemo
{
    public partial class Form1 : Form
    {
        private PacDrive m_pacDrive = null;

		private Size m_ledButtonSize = new Size(12, 8);
		private CheckBox[] m_checkBoxArray = null;

		private Random m_random = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
			m_checkBoxArray = new CheckBox[PacDrive.MAX_LEDCOUNT];
			m_random = new Random();

            CreateLEDButtons();

            m_pacDrive = new PacDrive(this);
			m_pacDrive.OnUsbDeviceAttached += new PacDrive.UsbDeviceAttachedDelegate(OnUsbDeviceAttached);
			m_pacDrive.OnUsbDeviceRemoved += new PacDrive.UsbDeviceRemovedDelegate(OnUsbDeviceRemoved);
            m_pacDrive.Initialize();

			m_pacDrive.SetLEDIntensityAll(PacDrive.MAX_INTENSITY);
			m_pacDrive.SetLEDStateAll(true);

            UpdateDeviceList();
        }

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_pacDrive.SetLEDIntensityAll(0);
			m_pacDrive.SetLEDStateAll(false);

			m_pacDrive.Shutdown();
		}

		private void CreateLEDButtons()
		{
			for (int y = 0; y < m_ledButtonSize.Height; y++)
			{
				for (int x = 0; x < m_ledButtonSize.Width; x++)
				{
					int id = y * m_ledButtonSize.Width + x;
					Size size = new Size(pnlLEDButtons.Width / m_ledButtonSize.Width, pnlLEDButtons.Height / m_ledButtonSize.Height);

					m_checkBoxArray[id] = new CheckBox();
					m_checkBoxArray[id].Location = new Point(x * size.Width, y * size.Height);
					m_checkBoxArray[id].Size = size;
					m_checkBoxArray[id].Appearance = Appearance.Button;
					m_checkBoxArray[id].Text = id.ToString();
					m_checkBoxArray[id].TextAlign = ContentAlignment.MiddleCenter;
					m_checkBoxArray[id].Checked = true;
					m_checkBoxArray[id].Click += new EventHandler(LEDButton_Click);

					pnlLEDButtons.Controls.Add(m_checkBoxArray[id]);
				}
			}
		}

        private void UpdateDeviceList()
        {
            lvwDevices.Items.Clear();

			for (int i = 0; i < m_pacDrive.DeviceCount; i++)
                lvwDevices.Items.Add(new ListViewItem(new string[] { m_pacDrive.GetDeviceType(i).ToString(), m_pacDrive.GetVendorId(i).ToString(), m_pacDrive.GetProductId(i).ToString(), m_pacDrive.GetVersionNumber(i).ToString(), m_pacDrive.GetVendorName(i), m_pacDrive.GetProductName(i), m_pacDrive.GetSerialNumber(i), m_pacDrive.GetDevicePath(i) }));

            for (int i = 0; i < lvwDevices.Columns.Count; i++)
                lvwDevices.Columns[i].Width = -2;

            if (lvwDevices.Items.Count > 0)
            {
                lvwDevices.SelectedIndices.Clear();
                lvwDevices.SelectedIndices.Add(0);
            }
        }

		private void OnUsbDeviceAttached(int id)
        {
            UpdateDeviceList();
        }

		private void OnUsbDeviceRemoved(int id)
        {
            UpdateDeviceList();
        }

		private void LEDButton_Click(object sender, EventArgs e)
		{
			if (lvwDevices.SelectedIndices.Count == 0)
				return;

			int deviceId = lvwDevices.SelectedIndices[0];

			CheckBox checkBox = (CheckBox)sender;
			int ledIndex = Int32.Parse(checkBox.Text);

			m_pacDrive.LEDState[deviceId][ledIndex] = checkBox.Checked;

			m_pacDrive.SetLEDStates(deviceId);
		}

		private void trkIntensity_Scroll(object sender, EventArgs e)
        {
            if (lvwDevices.SelectedIndices.Count == 0)
                return;

			int deviceId = lvwDevices.SelectedIndices[0];

            if ((int)nudLEDNumber.Value == 0)
            {
				for (int i = 0; i < PacDrive.MAX_LEDCOUNT; i++)
					m_pacDrive.LEDIntensity[deviceId][i] = (byte)trkIntensity.Value;
            }
            else
                m_pacDrive.LEDIntensity[deviceId][(int)nudLEDNumber.Value - 1] = (byte)trkIntensity.Value;

            m_pacDrive.SetLEDIntensity(deviceId);
        }

        private void butStartRecording_Click(object sender, EventArgs e)
        {
            if (lvwDevices.SelectedIndices.Count == 0)
                return;

			int deviceId = lvwDevices.SelectedIndices[0];

			m_pacDrive.StartScriptRecording(deviceId);
        }

		private void butStopRecording_Click(object sender, EventArgs e)
        {
            if (lvwDevices.SelectedIndices.Count == 0)
                return;

			int deviceId = lvwDevices.SelectedIndices[0];

			m_pacDrive.StopScriptRecording(deviceId);
        }

		private void butClearFlash_Click(object sender, EventArgs e)
        {
            if (lvwDevices.SelectedIndices.Count == 0)
                return;

			int deviceId = lvwDevices.SelectedIndices[0];

			m_pacDrive.ClearFlash(deviceId);
        }

		private void butRunScript_Click(object sender, EventArgs e)
        {
            if (lvwDevices.SelectedIndices.Count == 0)
                return;

			int deviceId = lvwDevices.SelectedIndices[0];

			m_pacDrive.RunScript(deviceId);
        }

        private void trkFadeTime_Scroll(object sender, EventArgs e)
        {
            if (lvwDevices.SelectedIndices.Count == 0)
                return;

			int deviceId = lvwDevices.SelectedIndices[0];

			m_pacDrive.SetLEDFadeTime(deviceId, (byte)trkFadeTime.Value);
        }

        private void rdoFlashSpeed_CheckedChanged(object sender, EventArgs e)
        {
            if (lvwDevices.SelectedIndices.Count == 0)
                return;

			int deviceId = lvwDevices.SelectedIndices[0];
            PacDrive.FlashSpeed flashSpeed = 0;

            if (rdoAlwaysOn.Checked)
                flashSpeed = PacDrive.FlashSpeed.AlwaysOn;
            else if (rdoSeconds_2.Checked)
                flashSpeed = PacDrive.FlashSpeed.Seconds_2;
            else if (rdoSeconds_1.Checked)
                flashSpeed = PacDrive.FlashSpeed.Seconds_1;
            else if (rdoSeconds_0_5.Checked)
                flashSpeed = PacDrive.FlashSpeed.Seconds_0_5;

            if(nudLEDNumber.Value == 0)
				m_pacDrive.SetLEDFlashSpeeds(deviceId, flashSpeed);
            else
				m_pacDrive.SetLEDFlashSpeed(deviceId, (byte)nudLEDNumber.Value - 1, flashSpeed);
        }

        private void trkScriptStepDelay_Scroll(object sender, EventArgs e)
        {
            if (lvwDevices.SelectedIndices.Count == 0)
                return;

			int deviceId = lvwDevices.SelectedIndices[0];

			m_pacDrive.SetScriptStepDelay(deviceId, (byte)trkScriptStepDelay.Value);
        }

        private void butSetDeviceID_Click(object sender, EventArgs e)
        {
            if (lvwDevices.SelectedIndices.Count == 0)
                return;

			int deviceId = lvwDevices.SelectedIndices[0];

			m_pacDrive.SetDeviceId(deviceId, (int)nudDeviceId.Value);
        }

        private void butProgramUHID_Click(object sender, EventArgs e)
        {
			if (lvwDevices.SelectedIndices.Count == 0)
				return;

			int deviceId = lvwDevices.SelectedIndices[0];

			if (m_pacDrive.GetDeviceType(deviceId) != PacDrive.DeviceType.UHID)
				return;

			OpenFileDialog fileDialog = new OpenFileDialog();

			fileDialog.Filter = "Raw Files (*.raw)|*.raw";
			fileDialog.RestoreDirectory = true;

			if (fileDialog.ShowDialog() == DialogResult.OK)
				m_pacDrive.ProgramUHid(deviceId, fileDialog.FileName);
        }

        private void butSetPermanent_Click(object sender, EventArgs e)
        {
			if (lvwDevices.SelectedIndices.Count == 0)
				return;

			int deviceId = lvwDevices.SelectedIndices[0];

			if (m_pacDrive.GetDeviceType(deviceId) != PacDrive.DeviceType.USBButton)
				return;

            byte[] data = new byte[62];
            byte[] releasedColorData = GetUSBButtonColor(butReleased.BackColor);
            byte[] pressedColorData = GetUSBButtonColor(butPressed.BackColor);
            byte[] strData = GetUSBButtonData(txtUrl.Text);

            data[0] = 0x00; // Mode (00 = Alternate, 01 = Extended, 02 = both)
            data[1] = 0x00; // Spare

            Buffer.BlockCopy(releasedColorData, 0, data, 2, pressedColorData.Length);
            Buffer.BlockCopy(pressedColorData, 0, data, 5, pressedColorData.Length);
            Buffer.BlockCopy(strData, 0, data, 8, strData.Length);

			m_pacDrive.SetUSBButtonConfigurePermanent(deviceId, data);
        }

        private void butSetTemporary_Click(object sender, EventArgs e)
        {
			if (lvwDevices.SelectedIndices.Count == 0)
				return;

			int deviceId = lvwDevices.SelectedIndices[0];

			if (m_pacDrive.GetDeviceType(deviceId) != PacDrive.DeviceType.USBButton)
				return;

            byte[] data = new byte[62];
            byte[] releasedColorData = GetUSBButtonColor(butReleased.BackColor);
            byte[] pressedColorData = GetUSBButtonColor(butPressed.BackColor);
            byte[] strData = GetUSBButtonData(txtUrl.Text);

            data[0] = 0x00; // Mode (00 = Alternate, 01 = Extended, 02 = both)
            data[1] = 0x00; // Spare

            Buffer.BlockCopy(releasedColorData, 0, data, 2, pressedColorData.Length);
            Buffer.BlockCopy(pressedColorData, 0, data, 5, pressedColorData.Length);
            Buffer.BlockCopy(strData, 0, data, 8, strData.Length);

			m_pacDrive.SetUSBButtonConfigureTemporary(deviceId, data);
        }

        private byte[] GetUSBButtonColor(Color color)
        {
            byte[] data = new byte[3];

            data[0] = color.R;
            data[1] = color.G;
            data[2] = color.B;

            return data;
        }

        private byte[] GetUSBButtonData(string str)
        {
            byte[] data = new byte[str.Length];

            for (int i = 0; i < str.Length; i++)
            {
                char chr = str.ToUpper()[i];

                if (chr >= 'A' && chr <= 'Z')
                    data[i] = (byte)(chr - 61);
                else if (chr == ' ')
                    data[i] = 0x2C;
            }

            return data;
        }

        private void butPressed_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog1 = new ColorDialog())
            {
                DialogResult result = colorDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    butPressed.BackColor = colorDialog1.Color;
                }
            }
        }

        private void butReleased_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog1 = new ColorDialog())
            {
                DialogResult result = colorDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    butReleased.BackColor = colorDialog1.Color;
                }
            }
        }

        private void butGetState_Click(object sender, EventArgs e)
        {
			if (lvwDevices.SelectedIndices.Count == 0)
				return;

			int deviceId = lvwDevices.SelectedIndices[0];

			if (m_pacDrive.GetDeviceType(deviceId) != PacDrive.DeviceType.USBButton)
				return;

			bool state = false;

			if (m_pacDrive.GetUSBButtonState(deviceId, ref state))
				lblState.Text = state ? "Pressed" : "Released";
        }

		private void butAllLEDsOn_Click(object sender, EventArgs e)
		{
			if (lvwDevices.SelectedIndices.Count == 0)
				return;

			int deviceId = lvwDevices.SelectedIndices[0];

			SetLEDStateAll(deviceId, true);
		}

		private void butAllLEDsOff_Click(object sender, EventArgs e)
		{
			if (lvwDevices.SelectedIndices.Count == 0)
				return;

			int deviceId = lvwDevices.SelectedIndices[0];

			SetLEDStateAll(deviceId, false);
		}

		private void butAllLEDsRandom_Click(object sender, EventArgs e)
		{
			if (lvwDevices.SelectedIndices.Count == 0)
				return;

			int deviceId = lvwDevices.SelectedIndices[0];

			for (int i = 0; i < PacDrive.MAX_LEDCOUNT; i++)
			{
				bool value = (m_random.Next(2) == 1 ? true : false);
				m_checkBoxArray[i].Checked = value;
				m_pacDrive.LEDState[deviceId][i] = value;
			}

            m_pacDrive.SetLEDStates(deviceId);
		}

		private void SetLEDStateAll(int deviceId, bool value)
		{
			for (int i = 0; i < PacDrive.MAX_LEDCOUNT; i++)
			{
				m_checkBoxArray[i].Checked = value;
				m_pacDrive.LEDState[deviceId][i] = value;
			}

			m_pacDrive.SetLEDStates(deviceId);
		}
    }
}