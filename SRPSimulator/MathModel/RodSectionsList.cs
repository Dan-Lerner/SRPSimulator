using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace SRPSimulator.MathModel
{
    // A set of classes for displaing collections as expandable items in property greed

    public partial class ExpandablePropertyList<T> : 
        ObservableCollection<T>, ICustomTypeDescriptor 
        where T : class, new()
    {
        public ExpandablePropertyList() 
            : base()
        {
            expandableName = "";
            childPrefix = "";
        }

        public ExpandablePropertyList(string expandableName, string childPrefix)
            : base()
        {
            this.expandableName = expandableName;
            this.childPrefix = childPrefix;
        }

        internal void Resize(int count)
        {
            while (Count < count)
                Add(new T());

            while (Count > count)
                RemoveAt(Count - 1);
        }

        // Implementation of ICustomTypeDescriptor:

        // Default behavior
        public string GetClassName() 
        { return TypeDescriptor.GetClassName(this, true); }
        public AttributeCollection GetAttributes() 
        { return TypeDescriptor.GetAttributes(this, true); }
        public string GetComponentName() 
        { return TypeDescriptor.GetComponentName(this, true); }
        public TypeConverter GetConverter() 
        { return TypeDescriptor.GetConverter(this, true); }
        public EventDescriptor GetDefaultEvent() 
        { return TypeDescriptor.GetDefaultEvent(this, true); }
        public PropertyDescriptor GetDefaultProperty() 
        { return TypeDescriptor.GetDefaultProperty(this, true); }
        public object GetEditor(Type editorBaseType) 
        { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
        public EventDescriptorCollection GetEvents(Attribute[] attributes) 
        { return TypeDescriptor.GetEvents(this, attributes, true); }
        public EventDescriptorCollection GetEvents() 
        { return TypeDescriptor.GetEvents(this, true); }
        public object GetPropertyOwner(PropertyDescriptor pd) 
        { return this; }

        // Gets collection as set of properties

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int ii = 0; ii < Count; ii++) {
                ExpandablePropertyDescriptor<T> pd = new(this, ii, expandableName, childPrefix);
                pds.Add(pd);
            }

            return pds;
        }

        private string expandableName;
        private string childPrefix;
    }

    public class ExpandablePropertyDescriptor<T> : PropertyDescriptor 
        where T : class, new()
    {
        public override AttributeCollection Attributes
        { get => new AttributeCollection(null); }

        public override Type ComponentType
        { get => collection.GetType(); }

        // Sets the Name
        public override string DisplayName
        { 
            get {
                //T section = collection[index];
                return childPrefix + "№" + index.ToString(); 
            } 
        }

        public override bool IsReadOnly
        { get => false; }

        public override string Name
        { get => "#" + index.ToString(); }

        public override Type PropertyType
        { get => collection[index].GetType(); }
        
        public override bool IsLocalizable 
        { get => false; }

        public override bool SupportsChangeEvents 
        { get => false; }

        public override string Description
        {
            get {
                StringBuilder sb = new StringBuilder();
                sb.Append(expandableName);
                return sb.ToString();
            }
        }

        internal ExpandablePropertyDescriptor(
            ExpandablePropertyList<T> collection, 
            int index, 
            string expandableName, 
            string childPrefix) 
            : base("#" + index.ToString(), null)
        {
            this.collection = collection;
            this.index = index;
            this.expandableName = expandableName;
            this.childPrefix = childPrefix;
        }

        public override bool CanResetValue(object component) => true; 

        public override object GetValue(object component) => collection[index];

        public override void ResetValue(object component)
        { }

        public override bool ShouldSerializeValue(object component) => true; 

        public override void SetValue(object component, object value)
        {
            collection[index] = value as T;
        }

        private ExpandablePropertyList<T> collection = null;
        private int index = -1;
        private string expandableName;
        private string childPrefix;
    }

    internal class ExpandablePropertyConverter<T> : ExpandableObjectConverter where T : class, new()
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value, 
            Type destType            )
        {
            // Supresses default titles
            if (destType == typeof(string) && (value is T || value is ExpandablePropertyList<T>))
                return "";

            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
