// ============================================================================
// ARCHIVO 1: Animation/AnimationDescriptor.cs
// ============================================================================
using System;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace DynamicUI.Animation
{
    /// <summary>
    /// Descriptor de animación con todas sus propiedades
    /// </summary>
    public class AnimationDescriptor
    {
        public string PropertyName { get; set; }
        public object FromValue { get; set; }
        public object ToValue { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan Delay { get; set; }
        public Easing EasingFunction { get; set; }
        public bool AutoReverse { get; set; }
        public int RepeatCount { get; set; } = 1;
        public FillMode FillMode { get; set; } = FillMode.Forward;
    }
}