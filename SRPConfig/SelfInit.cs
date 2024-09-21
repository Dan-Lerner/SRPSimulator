using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.Xml;

namespace SRPConfig
{
    // This class provides basic functionaliy for config jbjects by means of reflection:
    // - resets value to defaults
    // - memberwise copy
    // - serialization/deserialization to/from file

    public class SelfInit
    {
        private const string defaultFolder = @"\config\";

        public SelfInit() { ResetToDefault(); }

        // Modified flag
        // Must be set in case of changing of any configuration property
        [Browsable(false)]
        [NotMapped, JsonIgnore]
        public bool Modified
        { get; set; } = false;

        public bool ResetToDefault()
		{
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this)) {
				DefaultValueAttribute? defAttr = 
                    property.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
				if (defAttr is null)
					continue;
				property.SetValue(this, defAttr.Value);
			}
            Modified = false;
            return true;
        }

        public bool Copy(object? objectFrom)
        {
            if (objectFrom is null || GetType() != objectFrom?.GetType())
                return false;
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
                property.SetValue(this, property.GetValue(objectFrom));
            Modified = false;
            return false;
        }

        public override bool Equals(Object? objectFrom)
        {
            if (GetType() != objectFrom?.GetType())
                return false;

            foreach (PropertyDescriptor? property in TypeDescriptor.GetProperties(this)) {
                var value = property?.GetValue(this);
                var valueFrom = property?.GetValue(objectFrom);
                if (!value?.Equals(valueFrom) ?? false)
                    return false;
            }
            return true;
        }

        public override int GetHashCode() => base.GetHashCode();

        // Serialization/deserialization to/from XML file
        public virtual bool Save(string path = defaultFolder)
        {
            Type type = GetType();
            XmlSerializer xmlSerializer = new(type);
            string pathConfig = Environment.CurrentDirectory + path;
            if (!Directory.Exists(pathConfig))
                Directory.CreateDirectory(pathConfig);
            string fileXML = pathConfig + type.ToString() + ".xml";
            XmlWriterSettings settings = new() {
                Indent = true,
                IndentChars = ("\t"),
                OmitXmlDeclaration = true
            };
            using (XmlWriter writer = XmlWriter.Create(fileXML, settings)) {
                xmlSerializer.Serialize(writer, this);
            }
            return true;
        }
    
        public bool Load(string path = defaultFolder)
        {
            Type type = GetType();
            XmlSerializer xmlSerializer = new(type);
            string pathConfig = Environment.CurrentDirectory + path;
            if (!Directory.Exists(pathConfig))
                Directory.CreateDirectory(pathConfig);
            string fileXML = pathConfig + GetType().ToString() + ".xml";
            try {
                using (XmlReader reader = XmlReader.Create(fileXML)) {
                    var result = xmlSerializer.Deserialize(reader);
                    if (result is null)
                        return false;
                    bool Modified = (result as ConfigIdentity)!.Modified;
                    Copy(result);
                    this.Modified = Modified;
                }

                return true;
            }
            catch (IOException) {
            }
            catch (InvalidOperationException) {
            }
            return false;
        }
    }
}
