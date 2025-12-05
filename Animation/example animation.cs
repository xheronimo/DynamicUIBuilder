
// ============================================================================
// EJEMPLOS DE USO
// ============================================================================
/*
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Animation.Easings;
using DynamicUI.Animation;
using DynamicUI.Logging;

public class AnimationExamples
{
    public static async Task Main()
    {
        var logger = new UILogger();
        logger.AddTarget(new ConsoleLogTarget());

        var engine = new AnimationEngine(logger);

        // Ejemplo 1: Fade in simple
        var fadeDescriptor = new AnimationDescriptor
        {
            PropertyName = "Opacity",
            FromValue = 0.0,
            ToValue = 1.0,
            Duration = TimeSpan.FromMilliseconds(1000),
            EasingFunction = new QuadraticEaseOut()
        };

        var button = new Button { Content = "Click Me", Opacity = 0 };
        await engine.AnimateAsync(button, fadeDescriptor);

        // Ejemplo 2: Usando el builder
        var moveDescriptor = AnimationBuilder.Create()
            .Property("X")
            .From(0)
            .To(200)
            .Duration(2000)
            .Delay(500)
            .Easing(new BounceEaseOut())
            .Build();

        await engine.AnimateAsync(button, moveDescriptor);

        // Ejemplo 3: Animación con repetición
        var pulseDescriptor = AnimationBuilder.Create()
            .Property("Width")
            .From(100)
            .To(150)
            .Duration(1000)
            .Repeat(3)
            .AutoReverse(true)
            .Easing(new ElasticEaseOut())
            .Build();

        await engine.AnimateAsync(button, pulseDescriptor);

        // Ejemplo 4: Ver todas las funciones de easing disponibles
        var easings = engine.GetAvailableEasings();
        Console.WriteLine($"Funciones de easing disponibles: {easings.Count}");
        foreach (var easing in easings.Keys)
        {
            Console.WriteLine($"  - {easing}");
        }
    }
}

// Ejemplo de uso en archivo de texto:
/*
# interface.txt

# Fade in
TextBlock Text="Hola"; Animate:Opacity=0->1,1000,0,QuadraticEaseOut

# Movimiento con rebote
Button Text="Click"; X=0; Animate:X=0->200,2000,0,BounceEaseOut

# Animación repetida con auto-reverse
Border Background=Red; Animate:Width=100->300,1500,0,ElasticEaseOut,3,true

# Múltiples animaciones en el mismo control
Image Source="logo.png"; Animate:Opacity=0->1,1000; Animate:Y=0->50,2000,500,SineEaseInOut
*/
