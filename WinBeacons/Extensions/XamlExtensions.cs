using Windows.UI.Xaml;

namespace WinBeacons.Extensions
{
    /// <summary>
    ///     Attachable properties to use instead of common ValueConverters
    /// </summary>
    public class XamlExtensions : DependencyObject
    {
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
            "IsVisible", typeof(bool), typeof(XamlExtensions),
            new PropertyMetadata(true,
                (o, e) =>
                {
                    ((UIElement)o).Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
                }));

        public static void SetIsVisible(DependencyObject element, bool value)
        {
            element.SetValue(IsVisibleProperty, value);
        }

        public static bool GetIsVisible(DependencyObject element) => (bool)element.GetValue(IsVisibleProperty);

        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.RegisterAttached(
            "IsCollapsed", typeof(bool), typeof(XamlExtensions),
            new PropertyMetadata(default(bool),
                (o, e) =>
                {
                    ((UIElement)o).Visibility = (bool)e.NewValue ? Visibility.Collapsed : Visibility.Visible;
                }));

        public static bool GetIsCollapsed(DependencyObject element)
        {
            return (bool)element.GetValue(IsCollapsedProperty);
        }

        public static void SetIsCollapsed(DependencyObject element, bool value)
        {
            element.SetValue(IsCollapsedProperty, value);
        }
    }
}