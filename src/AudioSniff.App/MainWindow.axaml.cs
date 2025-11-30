using AudioSniff.App.Components;
using AudioSniff.Core;
using AudioSniff.Infrastructure;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.IO;

namespace AudioSniff.App;

public partial class MainWindow : Window
{
    private IAudioRecorder _recorder = new AudioRecorder();
    private WaveformDisplay _waveformDisplay;

    private static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private string? filePath = null;

    private string GetAutomaticFilePath() => Path.Combine(desktopPath, $"recording-{DateTime.Now.ToString("yyMMddHHmm")}.wav");

    public MainWindow()
    {
        InitializeComponent();

        _waveformDisplay = this.FindControl<WaveformDisplay>("WaveformDisplay");

        _recorder.SamplesAvailible += (s, samples) =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                _waveformDisplay.AddSample(samples);
            });
        };
    }

    private void Recording_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!_recorder.IsRecording)
        {
            if (filePath is null)
            {
                _recorder.Start(GetAutomaticFilePath());
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                _recorder.Start(filePath);
            }
            StartStopButton.Content = "■";
            StartStopButton.Foreground = Avalonia.Media.Brushes.White;
        }
        else
        {
            _recorder.Stop();
            StartStopButton.Content = "⬤";
            StartStopButton.Foreground = Avalonia.Media.Brushes.Red;
            filePath = null;
            PathDisplay.Text = "The file will save on desktop";
        }
    }

    private void OnSampleAvailible(float[] samples)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _waveformDisplay.AddSample(samples);
        });
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

    private void TopBar_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        this.BeginMoveDrag(e);
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