using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PsychoApp
{
    public static class ComboBoxArrowBehavior
    {
        public static readonly DependencyProperty TargetArrowProperty =
            DependencyProperty.RegisterAttached(
                "TargetArrow",
                typeof(Path),
                typeof(ComboBoxArrowBehavior),
                new PropertyMetadata(null, OnTargetArrowChanged));

        public static void SetTargetArrow(DependencyObject element, Path value)
        {
            element.SetValue(TargetArrowProperty, value);
        }

        public static Path GetTargetArrow(DependencyObject element)
        {
            return (Path)element.GetValue(TargetArrowProperty);
        }

        private static void OnTargetArrowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBox comboBox)
            {
                comboBox.DropDownOpened -= ComboBox_DropDownOpened;
                comboBox.DropDownClosed -= ComboBox_DropDownClosed;

                if (e.NewValue is Path)
                {
                    comboBox.DropDownOpened += ComboBox_DropDownOpened;
                    comboBox.DropDownClosed += ComboBox_DropDownClosed;
                }
            }
        }

        private static void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                var arrow = GetTargetArrow(comboBox);
                RotateArrow(arrow, 90);
            }
        }

        private static void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                var arrow = GetTargetArrow(comboBox);
                RotateArrow(arrow, 0);
            }
        }

        private static void RotateArrow(Path arrow, double angle)
        {
            if (arrow == null) return;

            if (!(arrow.RenderTransform is RotateTransform rotate))
            {
                rotate = new RotateTransform(0, arrow.Width / 2, arrow.Height / 2);
                arrow.RenderTransform = rotate;
            }

            var anim = new DoubleAnimation(angle, TimeSpan.FromMilliseconds(150));
            rotate.BeginAnimation(RotateTransform.AngleProperty, anim);
        }
    }
}
