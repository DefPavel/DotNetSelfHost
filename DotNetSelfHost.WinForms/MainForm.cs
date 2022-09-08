using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Net5SelfHost.WinForms
{
    public partial class MainForm : Form
    {
        private readonly SynchronizationContext _syncRoot;

        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(HandleRef hWnd);

        public MainForm()
        {
            InitializeComponent();
            _syncRoot = SynchronizationContext.Current;
             notifyIcon1.Click += new EventHandler(notifyIcon1_Click);
            toolStripItemExit.Click += new EventHandler(ExitApp);
            Closed += (a, b) =>
            {
                if (notifyIcon1 != null)
                {
                    notifyIcon1.Dispose();
                    notifyIcon1.Icon = null;
                    notifyIcon1.Visible = false;
                }
            };
        }

        private void ExitApp(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            SetForegroundWindow(new HandleRef(this, this.Handle));
            int x = MousePosition.X;
            int y = MousePosition.Y;
            x -= 10;
            y -= 40;
            contextMenuStrip1.Show(x, y);

        }

        public string NameText
        {
            get { return nameTextBox.Text; }
            set { _syncRoot.Post(SetName, value); }
        }

        private void SetName(object arg)
        {
            if (arg is string name)
                nameTextBox.Text = name;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
