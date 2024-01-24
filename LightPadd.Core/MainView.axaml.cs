using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LightPadd.Core;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Environment.Exit(0);
    }
}
