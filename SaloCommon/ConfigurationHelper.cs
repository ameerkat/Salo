using IniParser;
using IniParser.Model;
using System.Collections.Generic;

namespace Salo
{
    public static class ConfigurationHelper
    {
        public static Configuration LoadConfigurationFromIni(string iniPath)
        {
            FileIniDataParser parser = new FileIniDataParser();
            IniData data = parser.ReadFile(iniPath);
            Configuration configuration = new Configuration();
            foreach (var key in data.Sections["Configuration"])
            {
                configuration.Settings.Add(new cStringStringKeyValuePair(key.KeyName, key.Value));
            }

            if (!string.IsNullOrWhiteSpace(data.Sections["Meta"]["Name"]))
            {
                configuration.Name = data.Sections["Meta"]["Name"];
            }

            if (!string.IsNullOrWhiteSpace(data.Sections["Meta"]["Parent"]))
            {
                configuration.Parent = new Configuration {Name = data.Sections["Meta"]["Parent"]};
            }

            return configuration;
        }
    }
}
