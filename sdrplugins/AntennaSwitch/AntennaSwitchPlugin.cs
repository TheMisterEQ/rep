using System.Windows.Forms;
using SDRSharp.Common;
using SDRSharp.Radio;

namespace SDRSharp.AntennaSwitch
{
    public sealed class AntennaSwitchPlugin : ISharpPlugin, ICanLazyLoadGui, ISupportStatus, IExtendedNameProvider
    {
        private AntennaSwitchPanel? _gui;
        private ISharpControl? _control;

        public string DisplayName => "Przełącznik antenowy";
        public string Category => "Misc";
        public string MenuItemName => DisplayName;
        public bool IsActive => _gui != null && _gui.Visible;

        public UserControl Gui
        {
            get { LoadGui(); return _gui!; }
        }

        public void LoadGui()
        {
            if (_gui == null)
                _gui = new AntennaSwitchPanel(_control);
        }

        public void Initialize(ISharpControl control) => _control = control;

        public void Close()
        {
            _gui?.Dispose();
            _gui = null;
        }
    }
}
