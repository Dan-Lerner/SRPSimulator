using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SRPConfig;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace SRPSimulator.MathModel
{
    [DisplayName("Rod"), Description("Rod"), Category("SRP")]
    public class RodConfigEmbedded : RodConfig
    {
        // Config's parameters
        //--------------------
        //

        [DisplayName("Count of sections"), Description("Count of sections"), Category("Rod")]
        [DefaultValue(0), Range(1, 10, ErrorMessage = "This value ({0}) must be between {1}-{2}")]
        [JsonIgnore, XmlIgnore]
        public override int Count
        { 
            get => sections.Count; 
            set 
            {
                if (sections.Count != value)
                {
                    while (sections.Count < value)
                    {
                        RodSectionConfigEmbeded section = new();
                        section.NotifyInit += InitRod;
                        sections.Add(section);
                    }

                    while (sections.Count > value)
                    { sections.RemoveAt(sections.Count - 1); }

                    NotifyInitInvoke();
                }
            } 
        }

        private ExpandablePropertyList<RodSectionConfigEmbeded> sections = new();
        [DisplayName("Rod sections"), Description("Rod sections"), Category("Rod")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandablePropertyConverter<RodSectionConfigEmbeded>))]
        public new ExpandablePropertyList<RodSectionConfigEmbeded> RodSectionConfigs
        { 
            get => sections; 
            set
            {
                sections = value; 
                foreach (var section in sections)
                {
                    section.NotifyInit += InitRod;
                }
            }
        }

        private bool InitRod()
        {
            NotifyInitInvoke();
            return true;
        }
    }

    [DisplayName("Rod section"), Description("Rod section characteristics"), Category("SRP")]
    [TypeConverter(typeof(ExpandablePropertyConverter<RodSectionConfigEmbeded>))]
    public class RodSectionConfigEmbeded : RodSectionConfig
    {
        // Config identities
        //--------------------
        //

        [Browsable(false)]
        public override string Name
        { get; set; }

        [Browsable(false)]
        public override int RodConfigId
        { get; set; }

        // Config's parameters
        //--------------------
        //

        private double moduleJung;
        [DisplayName("Jung module"), Description("Jung module for rod matheril in Pa"), Category("Rod matherial")]
        [DefaultValue(moduleJungDef), Range(1E+11, 1E+12, ErrorMessage = "This value ({0}) must be between {1}-{2}")]
        [Browsable(true)]
        public override double ModuleJung
        { get => moduleJung; set { moduleJung = value; NotifyInitInvoke(); } }

        private double density;
        [DisplayName("Density"), Description("Density of rod matheril in kg/m3"), Category("Rod matherial")]
        [DefaultValue(densityDef), Range(1000.0, 20000.0, ErrorMessage = "This value ({0}) must be between {1}-{2}")]
        [Browsable(true)]
        public override double Density
        { get => density; set { density = value; NotifyInitInvoke(); } }

        private double length;
        [DisplayName("Length"), Description("Length of rod section in m"), Category("Rod section geometry")]
        [DefaultValue(100), Range(1, 10000, ErrorMessage = "This value ({0}) must be between {1}-{2}")]
        [Browsable(true)]
        public override double Length
        { get => length; set { length = value; NotifyInitInvoke(); } }

        private double d;
        [DisplayName("Rod diameter"), Description("Rod diameter in mm"), Category("Rod section geometry")]
        [DefaultValue(22), Range(1, 300, ErrorMessage = "This value ({0}) must be between {1}-{2}")]
        [Browsable(true)]
        public override double D
		{ get => d; set { d = value; NotifyInitInvoke(); } }
    }

    [DisplayName("Rod"), Description("Rod"), Category("SRP")]
    class Rod : Configurable
    {
        // Embedded objects
        //-----------------

        private List<RodSection> sections = new();
        public List<RodSection> Sections
        { 
            get => sections;
            set => sections = value;
        }

        // Output values
        //-------------------

        // Whole rod length, m
        private double length = 0;
        internal double Length
        { get => length; }

        // Whole rod volume, m3
        private double v = 0;
        internal double V
        { get => v; }

        // Whole rod mass, kg
        private double mass = 0;
        internal double Mass
        { get => mass; }

        // Whole rod weight, Newtons
        private double weight = 0;
        internal double Weight
        { get => weight; }

        // Reduced tension coeff for the whole rod
        // X1=FL1/(S1E)
        // X=FL1/(S1E)+FL2/(S2E)+...
        // tensionK = L1/(S1E)+L2/(S2E)+...
        // X=F*tensionK;
        private double tensionK;
        internal double TensionK
        { get => tensionK; }

        // Constructors
        //-------------------
        //

        public Rod()
        {
            Config = new RodConfigEmbedded();
            (config as RodConfigEmbedded).Count = 4;
        }

        internal override bool Init()
        {
            RodConfigEmbedded configInit = config as RodConfigEmbedded;

            length = 0;
            v = 0;
            mass = 0;
            weight = 0;
            tensionK = 0;

            // If the number of sections has changed then sets a new list of sections
            if (configInit.Count != sections.Count)
            {
                sections.Clear();
                for (short ii = 0; ii < configInit.Count; ii++)
                {
                    sections.Add(new RodSection(configInit.RodSectionConfigs[ii]));
                }
            }

            foreach (var section in sections)
            {
                if (!section.InitSection())
                {
                    configInit.Valid = false;
                    return false;
                }
            }

            // Values initialization
			interS = new double[configInit.Count];
            for (short ii = 0; ii < sections.Count; ii++)
            {
                if (sections[ii].Config.Length == 0)
                {
                    break;
                }

                if ((ii + 1) < sections.Count && sections[ii + 1].Config.Length > 0)
                {
                    interS[ii] = sections[ii + 1].S - sections[ii].S;
                }

                length += sections[ii].Config.Length;
                v += sections[ii].V;
                mass += sections[ii].Mass;
                weight += sections[ii].Weight;

                tensionK += sections[ii].Config.Length / (sections[ii].S * sections[ii].Config.ModuleJung);
            }

            configInit.Modified = true;
            configInit.Valid = true;

            return true;
        }

        public double[] interS;
    };

    public class RodSection
    {
        // Config
        private RodSectionConfigEmbeded config;
        public RodSectionConfigEmbeded Config
        { get => config; }

        // Output values
        //----------------------

        // Mass of section, kg
        private double mass;
        internal double Mass
        { get => mass; }

        // Weight, Newtons
        private double weight;
        internal double Weight
        { get => weight; }

        // Volume, m3
        private double v;
        internal double V
        { get => v; }

        // Cross sectional area, m2
        private double s;
        internal double S
        { get => s; }

        // Constructors
        //-------------------
        //

        public RodSection(RodSectionConfigEmbeded config)
        {
            this.config = (RodSectionConfigEmbeded)config;
            InitSection();
        }

        internal bool InitSection()
        {
            // Scaling of config parameters
            moduleJung = config.ModuleJung;
            density = config.Density;
            length = config.Length;
            d = config.D * PhysicsStuff.MILLI;

            s = Math.PI * d * d / 4.0;
			v = s * length;
			mass = density * v;
			weight = PhysicsStuff.g * mass;

            return true;
		}

        // Inner fields for calculations and states
        //-----------------------------------------

        // Scaled confObject parameters
        private double moduleJung;
        private double density;
        private double length;
        private double d;
    };
}
