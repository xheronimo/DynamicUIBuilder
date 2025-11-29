// ============================================================================
// ARCHIVO 1: Controls/Custom/AutoFitTextBlock.cs
// ============================================================================
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace DynamicUI.Controls.Custom
{
    /// <summary>
    /// TextBlock que ajusta automáticamente el tamaño de fuente para caber en su contenedor
    /// con modo debug para visualizar información
    /// </summary>
    public class AutoFitTextBlock : TextBlock
    {
        public static readonly StyledProperty<double> MinFontSizeProperty =
            AvaloniaProperty.Register<AutoFitTextBlock, double>(nameof(MinFontSize), 8.0);

        public static readonly StyledProperty<double> MaxFontSizeProperty =
            AvaloniaProperty.Register<AutoFitTextBlock, double>(nameof(MaxFontSize), 72.0);

        public static readonly StyledProperty<bool> AutoFitEnabledProperty =
            AvaloniaProperty.Register<AutoFitTextBlock, bool>(nameof(AutoFitEnabled), true);

        public static readonly StyledProperty<bool> ShowDebugInfoProperty =
            AvaloniaProperty.Register<AutoFitTextBlock, bool>(nameof(ShowDebugInfo), false);

        public static readonly StyledProperty<IBrush> DebugBorderBrushProperty =
            AvaloniaProperty.Register<AutoFitTextBlock, IBrush>(nameof(DebugBorderBrush), Brushes.Red);

        public static readonly StyledProperty<IBrush> DebugBackgroundProperty =
            AvaloniaProperty.Register<AutoFitTextBlock, IBrush>(nameof(DebugBackground), 
                new SolidColorBrush(Color.FromArgb(50, 255, 255, 0))); // Amarillo semi-transparente

        private Border _debugBorder;
        private TextBlock _debugText;
        private Panel _debugPanel;

        public double MinFontSize
        {
            get => GetValue(MinFontSizeProperty);
            set => SetValue(MinFontSizeProperty, value);
        }

        public double MaxFontSize
        {
            get => GetValue(MaxFontSizeProperty);
            set => SetValue(MaxFontSizeProperty, value);
        }

        public bool AutoFitEnabled
        {
            get => GetValue(AutoFitEnabledProperty);
            set => SetValue(AutoFitEnabledProperty, value);
        }

        public bool ShowDebugInfo
        {
            get => GetValue(ShowDebugInfoProperty);
            set => SetValue(ShowDebugInfoProperty, value);
        }

        public IBrush DebugBorderBrush
        {
            get => GetValue(DebugBorderBrushProperty);
            set => SetValue(DebugBorderBrushProperty, value);
        }

        public IBrush DebugBackground
        {
            get => GetValue(DebugBackgroundProperty);
            set => SetValue(DebugBackgroundProperty, value);
        }

        public AutoFitTextBlock()
        {
            // Suscribirse a cambios de propiedades
            this.GetObservable(TextProperty).Subscribe(_ => OnContentChanged());
            this.GetObservable(BoundsProperty).Subscribe(_ => OnContentChanged());
            this.GetObservable(MinFontSizeProperty).Subscribe(_ => OnContentChanged());
            this.GetObservable(MaxFontSizeProperty).Subscribe(_ => OnContentChanged());
            this.GetObservable(AutoFitEnabledProperty).Subscribe(_ => OnContentChanged());
            this.GetObservable(ShowDebugInfoProperty).Subscribe(_ => UpdateDebugVisualization());
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ShowDebugInfoProperty)
            {
                UpdateDebugVisualization();
            }
        }

        private void OnContentChanged()
        {
            if (AutoFitEnabled)
            {
                AdjustFontSize();
            }
            UpdateDebugInfo();
        }

        private void AdjustFontSize()
        {
            if (string.IsNullOrEmpty(Text) || Bounds.Width <= 0 || Bounds.Height <= 0)
                return;

            var availableWidth = Bounds.Width - Padding.Left - Padding.Right;
            var availableHeight = Bounds.Height - Padding.Top - Padding.Bottom;

            if (availableWidth <= 0 || availableHeight <= 0)
                return;

            // Búsqueda binaria del tamaño de fuente óptimo
            double low = MinFontSize;
            double high = MaxFontSize;
            double bestSize = MinFontSize;

            while (high - low > 0.5)
            {
                double mid = (low + high) / 2.0;
                var testSize = MeasureText(Text, mid);

                if (testSize.Width <= availableWidth && testSize.Height <= availableHeight)
                {
                    bestSize = mid;
                    low = mid;
                }
                else
                {
                    high = mid;
                }
            }

            FontSize = bestSize;
        }

        private Size MeasureText(string text, double fontSize)
        {
            var typeface = new Typeface(FontFamily, FontStyle, FontWeight);
            var formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private void UpdateDebugVisualization()
        {
            if (ShowDebugInfo)
            {
                CreateDebugVisualization();
            }
            else
            {
                RemoveDebugVisualization();
            }
        }

        private void CreateDebugVisualization()
        {
            if (_debugBorder != null)
                return;

            // Crear borde de debug
            _debugBorder = new Border
            {
                BorderBrush = DebugBorderBrush,
                BorderThickness = new Thickness(2),
                Background = DebugBackground,
                IsHitTestVisible = false
            };

            // Crear texto de debug
            _debugText = new TextBlock
            {
                FontSize = 10,
                Foreground = Brushes.Red,
                FontWeight = FontWeight.Bold,
                IsHitTestVisible = false,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                Margin = new Thickness(2)
            };

            // Crear panel contenedor
            _debugPanel = new Panel();
            
            // Intentar agregar al parent
            if (Parent is Panel parentPanel)
            {
                _debugBorder.Width = Bounds.Width;
                _debugBorder.Height = Bounds.Height;
                Canvas.SetLeft(_debugBorder, Canvas.GetLeft(this));
                Canvas.SetTop(_debugBorder, Canvas.GetTop(this));
                Canvas.SetLeft(_debugText, Canvas.GetLeft(this));
                Canvas.SetTop(_debugText, Canvas.GetTop(this) - 15);

                parentPanel.Children.Add(_debugBorder);
                parentPanel.Children.Add(_debugText);
            }

            UpdateDebugInfo();
        }

        private void RemoveDebugVisualization()
        {
            if (_debugBorder != null && Parent is Panel parentPanel)
            {
                parentPanel.Children.Remove(_debugBorder);
                parentPanel.Children.Remove(_debugText);
                _debugBorder = null;
                _debugText = null;
            }
        }

        private void UpdateDebugInfo()
        {
            if (_debugText == null || !ShowDebugInfo)
                return;

            var textSize = MeasureText(Text ?? "", FontSize);
            var availableWidth = Bounds.Width - Padding.Left - Padding.Right;
            var availableHeight = Bounds.Height - Padding.Top - Padding.Bottom;

            // Obtener el nombre del control si existe
            var controlName = !string.IsNullOrEmpty(Name) ? $"[{Name}] " : "";

            _debugText.Text = $"{controlName}AutoFitTextBlock | Font: {FontSize:F1}pt | Text: {textSize.Width:F0}x{textSize.Height:F0} | Available: {availableWidth:F0}x{availableHeight:F0}";

            // Actualizar posición del borde
            if (_debugBorder != null)
            {
                _debugBorder.Width = Bounds.Width;
                _debugBorder.Height = Bounds.Height;
                Canvas.SetLeft(_debugBorder, Canvas.GetLeft(this));
                Canvas.SetTop(_debugBorder, Canvas.GetTop(this));
                Canvas.SetLeft(_debugText, Canvas.GetLeft(this));
                Canvas.SetTop(_debugText, Canvas.GetTop(this) - 15);
            }
        }

        /// <summary>
        /// Obtiene información de debug como string
        /// </summary>
        public string GetDebugInfo()
        {
            var textSize = MeasureText(Text ?? "", FontSize);
            var availableWidth = Bounds.Width - Padding.Left - Padding.Right;
            var availableHeight = Bounds.Height - Padding.Top - Padding.Bottom;
            var controlName = !string.IsNullOrEmpty(Name) ? Name : "<sin nombre>";

            return $@"AutoFitTextBlock Debug Info:
- Name: ""{controlName}""
- Text: ""{Text}""
- Current FontSize: {FontSize:F2}pt
- Min/Max FontSize: {MinFontSize:F2}pt / {MaxFontSize:F2}pt
- Text Dimensions: {textSize.Width:F2} x {textSize.Height:F2}
- Available Space: {availableWidth:F2} x {availableHeight:F2}
- Control Bounds: {Bounds.Width:F2} x {Bounds.Height:F2}
- AutoFit Enabled: {AutoFitEnabled}
- Padding: {Padding}";
        }
    }
}