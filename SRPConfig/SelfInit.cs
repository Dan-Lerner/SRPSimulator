using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.Xml;

namespace SRPConfig
{
    // This class provides some of basic functionaliy for config:
    // - resets value to defaults
    // - memberwise copy
    // - serialization

    public class SelfInit
    {
        private const string defaultFolder = @"\config\";

        // Constructors
        //--------------
        //

        public SelfInit()
        {
            ResetToDefault();
        }

        // Modified flag
        // Must be set in case of changing any configuration property
        [Browsable(false)]
        [NotMapped, JsonIgnore]
        public bool Modified
        { get; set; } = false;

        // Methods
        //-------------
        //

        public bool ResetToDefault()
		{
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
			{
				DefaultValueAttribute? defAttr = property.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;

				if (defAttr is null)
				{
					continue;
				}

				property.SetValue(this, defAttr.Value);
			}

            Modified = false;

            return true;
        }

        public bool Copy(object? objectFrom)
        {
            if (objectFrom is null || GetType() != objectFrom?.GetType())
            {
                return false;
            }

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
            {
                property.SetValue(this, property.GetValue(objectFrom));
            }

            Modified = false;

            return false;
        }

        public override bool Equals(Object? objectFrom)
        {
            if (GetType() != objectFrom?.GetType())
            {
                return false;
            }

            foreach (PropertyDescriptor? property in TypeDescriptor.GetProperties(this))
            {
                var value = property?.GetValue(this);
                var valueFrom = property?.GetValue(objectFrom);

                if (!value?.Equals(valueFrom) ?? false)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // Serialization/deserialization to/from XML
        //-------------------------------------------
        //

        public virtual bool Save(string path = defaultFolder)
        {
            Type type = GetType();

            XmlSerializer xmlSerializer = new(type);

            string pathConfig = Environment.CurrentDirectory + path;
            string fileXML = pathConfig + type.ToString() + ".xml";

            XmlWriterSettings settings = new()
            {
                Indent = true,
                IndentChars = ("\t"),
                OmitXmlDeclaration = true
            };

            try
            {
                using (XmlWriter writer = XmlWriter.Create(fileXML, settings))
                {
                    xmlSerializer.Serialize(writer, this);
                }

                return true;
            }
            catch (IOException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return false;
        }
    
        public bool Load(string path = defaultFolder)
        {
            Type type = GetType();
            XmlSerializer xmlSerializer = new(type);

            string pathConfig = Environment.CurrentDirectory + path;
            string fileXML = pathConfig + GetType().ToString() + ".xml";

            try
            {
                using (XmlReader reader = XmlReader.Create(fileXML))
                {
                    var result = xmlSerializer.Deserialize(reader);
                    if (result is null)
                    {
                        return false;
                    }

                    bool Modified = (result as ConfigIdentity)!.Modified;
                    Copy(result);
                    this.Modified = Modified;
                }

                return true;
            }
            catch (IOException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return false;
        }
    }
}
