using AudioSniff.Core;
using AudioSniff.Infrastructure;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Diagnostics;
using System.IO;

namespace AudioSniff.App;

public partial class MainWindow : Window
{
    private IAudioRecorder recorder = new AudioRecorder();

    private static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private string? filePath = null;

    private string GetAutomaticFilePath() => Path.Combine(desktopPath, $"recording-{DateTime.Now.ToString("yyMMddHHmm")}.wav");

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
            if (filePath is null)
            {
                recorder.Start(GetAutomaticFilePath());
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                recorder.Start(filePath);
            }
            StartStopButton.Content = "Stop";
        }
        else
        {
            recorder.Stop();
            StartStopButton.Content = "Record";
            filePath = null;
            PathDisplay.Text = "The file will save on desktop";
        }
    }

    private async void ChoosePath_Click(object? sender, RoutedEventArgs e)
    {
        var options = new FilePickerSaveOptions
        {
            Title = "Select new location",
            SuggestedFileName = $"Recording_{DateTime.Now:yyMMdd_HHmmss}.wav",
            FileTypeChoices =
            [
                new FilePickerFileType("WAV file") { Patterns = new[] { "*.wav" } }
            ]
        };

        var result = await this.StorageProvider.SaveFilePickerAsync(options);
        if (result is not null)
        {
            filePath = result.Path.LocalPath;
            PathDisplay.Text = filePath;
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