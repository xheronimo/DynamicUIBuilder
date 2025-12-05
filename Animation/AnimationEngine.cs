
// ============================================================================
// ARCHIVO 2: Animation/AnimationEngine.cs
// ============================================================================
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Styling;
using DynamicUI.Logging;

namespace DynamicUI.Animation
{
    /// <summary>
    /// Motor de animaciones que ejecuta animaciones de Avalonia
    /// </summary>
    public class AnimationEngine
    {
        private readonly IUILogger _logger;
        private readonly Dictionary<string, Easing> _easingFunctions;

        public AnimationEngine(IUILogger logger)
        {
            _logger = logger;
            _easingFunctions = InitializeEasingFunctions();
        }

        private Dictionary<string, Easing> InitializeEasingFunctions()
        {
            return new Dictionary<string, Easing>(StringComparer.OrdinalIgnoreCase)
            {
                // Linear
                { "Linear", new LinearEasing() },
                
                // Quadratic
                { "QuadraticEaseIn", new QuadraticEaseIn() },
                { "QuadraticEaseOut", new QuadraticEaseOut() },
                { "QuadraticEaseInOut", new QuadraticEaseInOut() },
                { "QuadIn", new QuadraticEaseIn() },
                { "QuadOut", new QuadraticEaseOut() },
                { "QuadInOut", new QuadraticEaseInOut() },
                
                // Cubic
                { "CubicEaseIn", new CubicEaseIn() },
                { "CubicEaseOut", new CubicEaseOut() },
                { "CubicEaseInOut", new CubicEaseInOut() },
                { "CubicIn", new CubicEaseIn() },
                { "CubicOut", new CubicEaseOut() },
                { "CubicInOut", new CubicEaseInOut() },
                
                // Quartic
                { "QuarticEaseIn", new QuarticEaseIn() },
                { "QuarticEaseOut", new QuarticEaseOut() },
                { "QuarticEaseInOut", new QuarticEaseInOut() },
                { "QuartIn", new QuarticEaseIn() },
                { "QuartOut", new QuarticEaseOut() },
                { "QuartInOut", new QuarticEaseInOut() },
                
                // Quintic
                { "QuinticEaseIn", new QuinticEaseIn() },
                { "QuinticEaseOut", new QuinticEaseOut() },
                { "QuinticEaseInOut", new QuinticEaseInOut() },
                { "QuintIn", new QuinticEaseIn() },
                { "QuintOut", new QuinticEaseOut() },
                { "QuintInOut", new QuinticEaseInOut() },
                
                // Sine
                { "SineEaseIn", new SineEaseIn() },
                { "SineEaseOut", new SineEaseOut() },
                { "SineEaseInOut", new SineEaseInOut() },
                { "SineIn", new SineEaseIn() },
                { "SineOut", new SineEaseOut() },
                { "SineInOut", new SineEaseInOut() },
                
                // Exponential
                { "ExponentialEaseIn", new ExponentialEaseIn() },
                { "ExponentialEaseOut", new ExponentialEaseOut() },
                { "ExponentialEaseInOut", new ExponentialEaseInOut() },
                { "ExpoIn", new ExponentialEaseIn() },
                { "ExpoOut", new ExponentialEaseOut() },
                { "ExpoInOut", new ExponentialEaseInOut() },
                
                // Circular
                { "CircularEaseIn", new CircularEaseIn() },
                { "CircularEaseOut", new CircularEaseOut() },
                { "CircularEaseInOut", new CircularEaseInOut() },
                { "CircIn", new CircularEaseIn() },
                { "CircOut", new CircularEaseOut() },
                { "CircInOut", new CircularEaseInOut() },
                
                // Back
                { "BackEaseIn", new BackEaseIn() },
                { "BackEaseOut", new BackEaseOut() },
                { "BackEaseInOut", new BackEaseInOut() },
                { "BackIn", new BackEaseIn() },
                { "BackOut", new BackEaseOut() },
                { "BackInOut", new BackEaseInOut() },
                
                // Elastic
                { "ElasticEaseIn", new ElasticEaseIn() },
                { "ElasticEaseOut", new ElasticEaseOut() },
                { "ElasticEaseInOut", new ElasticEaseInOut() },
                { "ElasticIn", new ElasticEaseIn() },
                { "ElasticOut", new ElasticEaseOut() },
                { "ElasticInOut", new ElasticEaseInOut() },
                
                // Bounce
                { "BounceEaseIn", new BounceEaseIn() },
                { "BounceEaseOut", new BounceEaseOut() },
                { "BounceEaseInOut", new BounceEaseInOut() },
                { "BounceIn", new BounceEaseIn() },
                { "BounceOut", new BounceEaseOut() },
                { "BounceInOut", new BounceEaseInOut() }
            };
        }

        /// <summary>
        /// Obtiene una función de easing por nombre
        /// </summary>
        public Easing GetEasing(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new LinearEasing();

            return _easingFunctions.TryGetValue(name, out var easing) 
                ? easing 
                : new LinearEasing();
        }

        /// <summary>
        /// Ejecuta una animación de forma asíncrona
        /// </summary>
        public async Task AnimateAsync(Control control, AnimationDescriptor descriptor)
        {
            try
            {
                var property = GetAnimatableProperty(control, descriptor.PropertyName);
                if (property == null)
                {
                    _logger.LogWarning($"Propiedad '{descriptor.PropertyName}' no es animable en {control.GetType().Name}");
                    return;
                }

                var animation = CreateAnimation(descriptor, property);
                await animation.RunAsync(control);
                
                _logger.LogDebug($"Animación completada: {descriptor.PropertyName} en {control.GetType().Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error ejecutando animación: {ex.Message}", ex);
            }
        }

        private Avalonia.Animation.Animation CreateAnimation(AnimationDescriptor descriptor, AvaloniaProperty property)
        {
            var animation = new Avalonia.Animation.Animation
            {
                Duration = descriptor.Duration,
                Delay = descriptor.Delay,
                FillMode = descriptor.FillMode,
                Easing = descriptor.EasingFunction
            };

            // Configurar iteraciones
            if (descriptor.RepeatCount > 1)
            {
                animation.IterationCount = new IterationCount((ulong)descriptor.RepeatCount);
            }

            // Configurar dirección
            if (descriptor.AutoReverse)
            {
                animation.PlaybackDirection = PlaybackDirection.Alternate;
            }

            // Crear keyframe para el valor final
            var keyFrame = new KeyFrame
            {
                Cue = new Cue(1.0)
            };

            keyFrame.Setters.Add(new Setter(property, descriptor.ToValue));
            animation.Children.Add(keyFrame);

            // Si hay valor inicial, crear keyframe para el inicio
            if (descriptor.FromValue != null)
            {
                var startKeyFrame = new KeyFrame
                {
                    Cue = new Cue(0.0)
                };
                startKeyFrame.Setters.Add(new Setter(property, descriptor.FromValue));
                animation.Children.Insert(0, startKeyFrame);
            }

            return animation;
        }

        /// <summary>
        /// Obtiene la propiedad animable de un control
        /// </summary>
        private AvaloniaProperty GetAnimatableProperty(Control control, string propertyName)
        {
            var type = control.GetType();
            var fieldName = propertyName + "Property";
            
            var field = type.GetField(fieldName, 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Static | 
                System.Reflection.BindingFlags.FlattenHierarchy);

            return field?.GetValue(null) as AvaloniaProperty;
        }

        /// <summary>
        /// Obtiene todas las funciones de easing disponibles
        /// </summary>
        public IReadOnlyDictionary<string, Easing> GetAvailableEasings() => _easingFunctions;
    }
}