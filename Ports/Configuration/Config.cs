using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Ports.Configuration
{
    public class Config
    {
        private static Config instance;

        #region Configurable Properties

        public int CellPadding { get; set; } = 10;

        public int ButtonWidth { get; set; } = 75;

        public int ButtonHeight { get; set; } = 75;

        public IList<EntryConfig> Entries { get; set; }

        #endregion

        public static string CurrentDirectory
        {
            get
            {
                var exeFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#if DEBUG
                var projectFolder = Regex.Split(exeFolder, @"\\bin\\Debug")[0];
                return projectFolder;
#else
                return exeFolder;
#endif                
            }
        }

        public static Config Get(string fileName = "Config.json")
        {
            if (instance == null)
            {
                var path = Path.Combine(CurrentDirectory, fileName);
                var json = File.ReadAllText(path);

                instance = JsonConvert.DeserializeObject<Config>(json);

                instance.Validate();
            }

            return instance;
        }

        public void Validate()
        {

        }
    }
}
