using SRPConfig;

namespace SRPSimulator.MathModel
{
    // The base class for objects containing configuration

    public abstract class Configurable
    {
        protected Configurable(ConfigIdentity config)
        {
            Config = config;
        }

        protected ConfigIdentity config;
        public virtual ConfigIdentity Config {
            get => config;
            set {
                config = value;
                if (config != null) {
                    config.NotifyInit += Init;
                    Init();
                }
            }
        }

        internal abstract bool Init();
    }
}
