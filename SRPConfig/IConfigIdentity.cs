using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace SRPConfig
{
    // This section defines and implements basic functionality of configs

    public interface IConfigIdentity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public abstract int Id
        { get; set; }

        public abstract string? Name
        { get; set; }
    }

    public class ConfigIdentity : SelfInit, IConfigIdentity, INotifyPropertyChanged
    {
        private const string defaultName = "Default";
        protected const string defaultErrorMsg = "This value ({0}) must be between {1}-{2}";
  
        public delegate bool Init();
        public virtual event Init? NotifyInit;

        [DefaultValue(0)]
        [Browsable(false)]
        public virtual int Id
        { get; set; }

        protected string? name;
        [DefaultValue(defaultName)]
        [Browsable(false)]
        public virtual string? Name
        { get => name; set { name = value; NotifyInit?.Invoke(); } }

        // Validation flag
        // Must be set if some of config's values are wrong
        [DefaultValue(false), Range(typeof(bool), "true", "true", 
            ErrorMessage = "Parameters are incomatible. Model is not valid.")]
        [Browsable(false)]
        [NotMapped, JsonIgnore, XmlIgnore]
        public bool Valid
        { get; set; }

        // Resets identities for adding to DB
        public void InitForAdd(string Name)
        {
            Id = 0;
            this.Name = Name;
        }

        // Perform initialization from another thread
        public bool? NotifyInitInvoke()
        {
            return NotifyInit?.Invoke();
        }
    
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
