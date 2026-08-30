using System;
using System.Drawing;
using System.Windows.Forms;
using SDRSharp.Common;

namespace SDRSharp.AntennaSwitch
{
    public sealed class AntennaSwitchPanel : UserControl
    {
        private readonly OrangePiClient _client = new();
        private readonly TextBox _ip = new() { Text = "192.168.1.50", Width = 120 };
        private readonly NumericUpDown _port = new() { Minimum = 1, Maximum = 65535, Value = 5000, Width = 70 };
        private readonly Label _status = new() { Text = "Stan: Rozłączono", AutoSize = true };
        private readonly Label _temperature = new() { Text = "Temperatura: --.- °C", AutoSize = true };
        private readonly Label _humidity = new() { Text = "Wilgotność: --.- %", AutoSize = true };
        private readonly Label _pressure = new() { Text = "Ciśnienie: ----.- hPa", AutoSize = true };
        private readonly Button[] _antennaButtons = new Button[4];
        private readonly Timer _timer;

        public AntennaSwitchPanel(ISharpControl? control)
        {
            AutoSize = true;
            Padding = new Padding(8);
            var root = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true };
            root.Controls.Add(new Label { Text = "PRZEŁĄCZNIK ANTENOWY", AutoSize = true, Font = new Font(Font, FontStyle.Bold) });
            root.Controls.Add(Row("Adres IP Orange Pi:", _ip));
            root.Controls.Add(Row("Port:", _port));
            var connect = new Button { Text = "POŁĄCZ", AutoSize = true };
            connect.Click += (_, _) => Connect();
            root.Controls.Add(connect);
            root.Controls.Add(_status);
            root.Controls.Add(new Label { Text = "GPIO A: PE12 / pin 29", AutoSize = true });
            root.Controls.Add(new Label { Text = "GPIO B: PE13 / pin 31", AutoSize = true });
            for (int i = 0; i < 4; i++)
            {
                int antenna = i + 1;
                var b = new Button { Text = $"ANTENA {antenna}", Width = 110, Enabled = false };
                b.Click += (_, _) => SelectAntenna(antenna);
                _antennaButtons[i] = b;
                root.Controls.Add(b);
            }
            root.Controls.Add(new Label { Text = "POMIARY", AutoSize = true, Font = new Font(Font, FontStyle.Bold), Margin = new Padding(3, 12, 3, 3) });
            root.Controls.Add(_temperature);
            root.Controls.Add(_humidity);
            root.Controls.Add(_pressure);
            Controls.Add(root);
            _timer = new Timer { Interval = 5000 };
            _timer.Tick += (_, _) => { if (_client.IsConnected) RefreshMeasurements(); };
        }

        private static Control Row(string text, Control editor)
        {
            var p = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
            p.Controls.Add(new Label { Text = text, Width = 140, AutoSize = false });
            p.Controls.Add(editor);
            return p;
        }

        private void Connect()
        {
            _client.Host = _ip.Text.Trim();
            _client.Port = (int)_port.Value;
            bool ok = _client.Connect();
            _status.Text = ok ? "Stan: Połączono" : "Stan: Rozłączono";
            foreach (var button in _antennaButtons) button.Enabled = ok;
            if (ok) { _timer.Start(); RefreshMeasurements(); }
            else _timer.Stop();
        }

        private void SelectAntenna(int antenna)
        {
            // HMC7992: ANT1=00, ANT2=01, ANT3=10, ANT4=11.
            int a = (antenna - 1) / 2;
            int b = (antenna - 1) % 2;
            _client.SendCommand($"GPIO A={a} B={b}");
        }

        private void RefreshMeasurements()
        {
            // Temperatura/wilgotność/ciśnienie pozostają na razie tylko w panelu.
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _timer?.Dispose();
            base.Dispose(disposing);
        }
    }
}
