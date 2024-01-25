using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using System;

namespace LightPadd.Core.Controls;

public partial class LcarsHeader : UserControl
{
    static LcarsHeader()
    {
        TextProperty.Changed.AddClassHandler<LcarsHeader>(TextChanged);
    }

    public LcarsHeader()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<object> HeaderContentProperty = AvaloniaProperty.Register<LcarsHeader, object>(nameof(HeaderContent));
    public object HeaderContent
    {
        get => GetValue(HeaderContentProperty); 
        set => SetValue(HeaderContentProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate> HeaderContentTemplateProperty = AvaloniaProperty.Register<LcarsHeader, IDataTemplate>(nameof(HeaderContentTemplate));
    public IDataTemplate HeaderContentTemplate
    {
        get => GetValue(HeaderContentTemplateProperty);
        set => SetValue(HeaderContentTemplateProperty, value);
    }

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<LcarsHeader, string>(nameof(Text), string.Empty);
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    private static void TextChanged(LcarsHeader _this, AvaloniaPropertyChangedEventArgs args)
    {
        _this.HeaderContent = args.NewValue ?? string.Empty;
        _this.HeaderContentTemplate = _this.DataTemplates[0];
    }

}