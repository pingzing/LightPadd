using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using System;

namespace LightPadd.Core.Controls;

public partial class LcarsHeader : TemplatedControl
{
    public LcarsHeader()
    {        
        
    }

    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty = 
        ContentControl.ContentTemplateProperty.AddOwner<LcarsHeader>();
    public IDataTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public static readonly StyledProperty<object?> ContentProperty = 
        ContentControl.ContentProperty.AddOwner<LcarsHeader>();
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty = 
        ContentControl.HorizontalContentAlignmentProperty.AddOwner<LcarsHeader>();
    public HorizontalAlignment HorizontalContentAlignment
    {
        get => GetValue(HorizontalContentAlignmentProperty);
        set => SetValue(HorizontalContentAlignmentProperty, value);
    }

}