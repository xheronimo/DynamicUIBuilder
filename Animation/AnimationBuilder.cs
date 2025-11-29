
// ============================================================================
// ARCHIVO 5: Animation/AnimationBuilder.cs (Helper)
// ============================================================================
using System;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace DynamicUI.Animation
{
    /// <summary>
    /// Builder fluent para crear animaciones de forma programática
    /// </summary>
    public class AnimationBuilder
    {
        private readonly AnimationDescriptor _descriptor = new();

        public AnimationBuilder Property(string propertyName)
        {
            _descriptor.PropertyName = propertyName;
            return this;
        }

        public AnimationBuilder From(object value)
        {
            _descriptor.FromValue = value;
            return this;
        }

        public AnimationBuilder To(object value)
        {
            _descriptor.ToValue = value;
            return this;
        }

        public AnimationBuilder Duration(int milliseconds)
        {
            _descriptor.Duration = TimeSpan.FromMilliseconds(milliseconds);
            return this;
        }

        public AnimationBuilder Delay(int milliseconds)
        {
            _descriptor.Delay = TimeSpan.FromMilliseconds(milliseconds);
            return this;
        }

        public AnimationBuilder Easing(Easing easing)
        {
            _descriptor.EasingFunction = easing;
            return this;
        }

        public AnimationBuilder Repeat(int count)
        {
            _descriptor.RepeatCount = count;
            return this;
        }

        public AnimationBuilder AutoReverse(bool value = true)
        {
            _descriptor.AutoReverse = value;
            return this;
        }

        public AnimationDescriptor Build()
        {
            if (string.IsNullOrEmpty(_descriptor.PropertyName))
                throw new InvalidOperationException("PropertyName es requerido");
            
            if (_descriptor.ToValue == null)
                throw new InvalidOperationException("ToValue es requerido");
            
            if (_descriptor.Duration == TimeSpan.Zero)
                throw new InvalidOperationException("Duration es requerido");

            return _descriptor;
        }

        public static AnimationBuilder Create() => new AnimationBuilder();
    }
}