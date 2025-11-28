using AudioSniff.Core;
using AudioSniff.Infrastructure;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.IO;

namespace AudioSniff.App;

public partial class MainWindow : Window
{
    private IAudioRecorder recorder = new AudioRecorder();

    private static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private string filePath = Path.Combine(desktopPath, $"recording-{DateTime.Now.ToString("yyyyMMddHHmm")}.wav");

    public MainWindow()
    {
        InitializeComponent();
    }

    private void TopBar_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        this.BeginMoveDrag(e);
    }

    private void Recording_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!recorder.IsRecording)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            recorder.Start(filePath);
            StartStopButton.Content = "Stop";
        }
        else
        {
            recorder.Stop();
            StartStopButton.Content = "Record";
        }
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