using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace AudioSniff.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TopBar_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        this.BeginMoveDrag(e);
    }

    private void StartRecording_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Debug.WriteLine("Start Recording pressed");
        // WASAPI
    }

    private void Close_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void Minimize_Click(object? sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
}