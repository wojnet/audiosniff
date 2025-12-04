namespace AudioSniff.Core;

public interface IAudioRecorder
{
    public event EventHandler<float[]>? SamplesAvailible;
    void Start(string filePath);
    void Stop();
    bool IsRecording { get; }
}
