namespace AudioSniff.Core;

public interface IAudioRecorder
{
    void Start(string filePath);
    void Stop();
    bool IsRecording { get; }
}
