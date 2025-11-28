using AudioSniff.Core;
using NAudio.Wave;

namespace AudioSniff.Infrastructure;

public class AudioRecorder : IAudioRecorder
{
    private WasapiLoopbackCapture? capture;
    private WaveFileWriter? wavWriter;

    private bool isRecording;
    public bool IsRecording => isRecording;

    public void Start(string filePath)
    {
        if (isRecording) return;

        capture = new WasapiLoopbackCapture();
        wavWriter = new WaveFileWriter(filePath, capture.WaveFormat);

        capture.DataAvailable += (s, e) => wavWriter.Write(e.Buffer, 0, e.BytesRecorded);
        capture.RecordingStopped += (s, e) =>
        {
            wavWriter?.Dispose();
            capture?.Dispose();
            wavWriter = null;
            capture = null;
            isRecording = false;
        };

        capture.StartRecording();
        isRecording = true;
    }

    public void Stop()
    {
        if (!isRecording) return;
        capture!.StopRecording();
    }
}