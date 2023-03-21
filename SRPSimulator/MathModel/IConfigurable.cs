using SRPConfig;

namespace SRPSimulator.MathModel
{
    // The base class for objects with configuration

    public abstract class Configurable
    {
        protected ConfigIdentity config;
        public virtual ConfigIdentity Config
        {
            get => config;
            set
            {
                config = value; 
                config.NotifyInit += Init;
                Init();
            }
        }

        internal abstract bool Init();
    }
}
