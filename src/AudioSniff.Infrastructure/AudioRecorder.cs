using AudioSniff.Core;
using NAudio.Wave;

namespace AudioSniff.Infrastructure;

public class AudioRecorder : IAudioRecorder
{
    private WasapiLoopbackCapture? capture;
    private WaveFileWriter? wavWriter;

    private bool isRecording;
    public bool IsRecording => isRecording;

    public event EventHandler<float[]>? SamplesAvailible;

    public void Start(string filePath)
    {
        if (isRecording) return;

        capture = new WasapiLoopbackCapture();
        wavWriter = new WaveFileWriter(filePath, capture.WaveFormat);

        capture.DataAvailable += (s, e) =>
        {
            // Save to file
            wavWriter.Write(e.Buffer, 0, e.BytesRecorded);

            // Float conversion [-1, 1], and invoke event
            int samples = e.BytesRecorded / 2;
            float[] buffer = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                short sample16 = BitConverter.ToInt16(e.Buffer, i * 2);
                buffer[i] = sample16 / 32768f;
            }

            SamplesAvailible?.Invoke(this, buffer);
        };

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