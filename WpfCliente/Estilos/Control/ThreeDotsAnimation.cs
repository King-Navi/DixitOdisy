﻿using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace WpfCliente.Estilos
{
    /// <summary>
    /// Este codigo fue tomado de un autor externo
    /// </summary>
    /// <ref>https://github.com/madhawapolkotuwa/WPF.LoadingAnimations/blob/master/MpCoding.WPF.LoadingAnimations/Controls/DonutSpinner.cs</ref>
    public class ThreeDotsAnimation : Control
    {
        static ThreeDotsAnimation()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ThreeDotsAnimation), new FrameworkPropertyMetadata(typeof(ThreeDotsAnimation)));
        }
        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(ThreeDotsAnimation), new PropertyMetadata(Brushes.LightBlue));
        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }
        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(ThreeDotsAnimation), new PropertyMetadata(1.0));
        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(Duration), typeof(ThreeDotsAnimation), new PropertyMetadata(default(Duration), CalculateSpeedRatio));
        private static void CalculateSpeedRatio(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (d is ThreeDotsAnimation control)
                {
                    double spr = 1.0 / control.Duration.TimeSpan.TotalSeconds;
                    control.SpeedRatio = spr;
                }
            }
            catch
            {
                if (d is ThreeDotsAnimation control)
                    control.SpeedRatio = 1.0;
            }
        }
    }
}
