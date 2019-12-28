using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetFwTypeLib;
using Ports.Configuration;

namespace Ports
{
    public partial class Main : Form
    {
        private readonly Config config;
        private readonly INetFwPolicy2 firewallPolicy;
        private readonly IList<Button> buttons = new List<Button>();

        public Main(Config config, INetFwPolicy2 firewallPolicy)
        {
            this.config = config;
            this.firewallPolicy = firewallPolicy;

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
            var entry = config.Entries.FirstOrDefault(ent => ent.Name == button.Tag.ToString());
            var entryName = entry.Name.ToLowerInvariant();

            if (entryName == "public")
            {
                var profileType = NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC;

                if (toggle)
                {
                    ToggleProfileRule(profileType);
                }

                UpdateButtonColor(button, firewallPolicy.DefaultOutboundAction[profileType] == NET_FW_ACTION_.NET_FW_ACTION_ALLOW);
            }
            else if (entryName == "private")
            {
                var profileType = NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE;

                if (toggle)
                {
                    ToggleProfileRule(profileType);
                }

                UpdateButtonColor(button, firewallPolicy.DefaultOutboundAction[profileType] == NET_FW_ACTION_.NET_FW_ACTION_ALLOW);
            }
            else
            {
                if (toggle)
                {
                    ToggleRules(entry.Rules);
                }

                UpdateButtonColor(button, firewallPolicy.Rules.Item(entry.Rules[0]).Enabled);
            }
        }

        private void ToggleProfileRule(NET_FW_PROFILE_TYPE2_ profileType)
        {
            if (firewallPolicy.DefaultOutboundAction[profileType] == NET_FW_ACTION_.NET_FW_ACTION_BLOCK)
            {
                firewallPolicy.DefaultOutboundAction[profileType] = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            }
            else
            {
                firewallPolicy.DefaultOutboundAction[profileType] = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            }
        }

        private void ToggleRules(IEnumerable<string> rules)
        {
            foreach (var name in rules)
            {
                var rule = firewallPolicy.Rules.Item(name);
                rule.Enabled = !rule.Enabled;
            }
        }

        private void UpdateButtonColor(Button button, bool condition)
        {
            if (condition)
            {
                button.ForeColor = Color.Green;
            }
            else
            {
                button.ForeColor = Color.Red;
            }
        }
    }
}
