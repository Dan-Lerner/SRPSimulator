using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SRPSimulator
{
    internal class ButtonExtension
    {
        public static readonly DependencyProperty IconProperty;

        static ButtonExtension()
        {
            var metadata = new FrameworkPropertyMetadata(null);
            IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(FrameworkElement), typeof(ButtonExtension), metadata);
        }

        public static FrameworkElement GetIcon(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(IconProperty, value);
        }
    }
}
