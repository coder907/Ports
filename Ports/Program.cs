using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Serilog;

using Ports.Configuration;
using Ports.Firewall;

namespace Ports
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs\\.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Reading configuration ...");
                var config = Config.Get();

                Log.Information("Accessing firewall policy ...");
                var firewallManager = FirewallManager.Instance;

                Log.Information("Starting UI ...");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main(config));
            }
            catch (ConfigException ex)
            {
                Log.Error($"{ex}");

            }
            catch (Exception ex)
            {
                Log.Error($"General error. {ex}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
