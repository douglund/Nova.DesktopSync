using System;

namespace Nova.Windows.DesktopSync
{
    public struct PropertyValue
    {
        public string Name { get; set; }

        public object Value { get; set; }
        
        public PropertyValue(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Value = value;
        }
    }
}