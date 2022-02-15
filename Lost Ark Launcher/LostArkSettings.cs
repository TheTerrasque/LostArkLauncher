using System.Xml.Linq;
using System.Xml.XPath;

namespace Lost_Ark_Launcher
{
    internal class LostArkSettings
    {
        public LostArkSettings(string path)
        {
            FilePath = path;
        }

        public string FilePath { get; }

        public string? GetXmlPathValue(string path)
        {
            return GetXmlPath(path)?.Value;
        }

        public XElement? GetXmlPath(string path)
        {
            var config = OpenConfigFile();
            if (config != null)
            {
                var region = config.XPathSelectElement(path);
                if (region != null)
                {
                    return region;
                }
            }
            return null;
        }

        public void SetXmlPath(string path, string value)
        {
            var val = GetXmlPath(path);
            if (val != null)
                val.Value = value;
            Save(val.Document);
        }

        private XDocument OpenConfigFile()
        {
            return XDocument.Load(FilePath);
        }

        public void Save(XDocument xml)
        {
            xml.Save(FilePath);
        }

    }
}
