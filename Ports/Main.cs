using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Ports.Configuration;
using Ports.Firewall;
using Ports.Utils;

namespace Ports
{
    public partial class Main : Form
    {
        private readonly Config config;
        private readonly IList<Button> buttons = new List<Button>();

        public Main(Config config)
        {
            this.config = config;

            InitializeComponent();
            CreateUI();
        }

        private void CreateUI()
        {
            var fp = new FlowLayoutPanel();
            fp.SuspendLayout();
            SuspendLayout();

            foreach (var entry in config.Entries)
            {
                var button = new Button
                {
                    Text = entry.Description,
                    Width = config.ButtonWidth,
                    Height = config.ButtonHeight,
                    Tag = entry.Name
                };
                button.Click += Button_Click;

                buttons.Add(button);
            }

            fp.Controls.AddRange(buttons.ToArray());
            fp.Dock = DockStyle.Fill;

            Icon = IconExtractor.Extract("shell32.dll", 9, true);
            Location = Properties.Settings.Default.WindowLocation;
            Size = Properties.Settings.Default.WindowSize;
            Controls.Add(fp);

            fp.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            foreach (var button in buttons)
            {
                UpdateButton(button);
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.WindowLocation = Location;
            Properties.Settings.Default.WindowSize = Size;
            Properties.Settings.Default.Save();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            UpdateButton((Button)sender, true);
        }

        private void UpdateButton(Button button, bool toggle = false)
        {
            var entry = config.Entries.First(ent => ent.Name == button.Tag.ToString());

            if (toggle)
            {
                FirewallManager.Instance.Toggle(entry);
            }

            button.ForeColor = FirewallManager.Instance.IsEnabled(entry) ? Color.Green : Color.Red;
        }
    }
}
