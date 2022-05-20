using System;
using System.IO;
using System.Reflection;

namespace Gumayusi_Orbwalker.Core.Settings
{
    public class AppSettings
    {
        private string _onfigFileName = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\Config.xml";
        private Settings _config = new Settings();

        public Settings Config
        {
            get { return _config; }
            set { _config = value; }
        }

        // Load configuration file
        public void LoadConfig()
        {
            if (File.Exists(_onfigFileName))
            {
                StreamReader srReader = File.OpenText(_onfigFileName);
                Type tType = _config.GetType();
                System.Xml.Serialization.XmlSerializer xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);
                object oData = xsSerializer.Deserialize(srReader);
                _config = (Settings)oData;
                srReader.Close();
            }
        }

        // Save configuration file
        public void SaveConfig()
        {
            StreamWriter swWriter = File.CreateText(_onfigFileName);
            Type tType = _config.GetType();
            if (tType.IsSerializable)
            {
                System.Xml.Serialization.XmlSerializer xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);
                xsSerializer.Serialize(swWriter, _config);
                swWriter.Close();
            }
        }
    }
}
