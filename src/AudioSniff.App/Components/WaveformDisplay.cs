using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;

namespace AudioSniff.App.Components
{
    public class WaveformDisplay : Control
    {
        private Queue<float> samples = new Queue<float>();
        public int MaxSamples { get; set; } = 44100 * 2;

        private DispatcherTimer timer;

        public WaveformDisplay()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            timer.Tick += (s, e) => InvalidateVisual();
            timer.Start();
        }

        public void AddSample(float[] newSamples)
        {
            foreach (var sample in newSamples)
            {
                samples.Enqueue(sample);
                if (samples.Count > MaxSamples) samples.Dequeue();
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var width = Bounds.Width;
            var height = Bounds.Height;
            var bgColor = Color.Parse("#222222");
            var waveformColor = Color.Parse("#00FF00");

            context.DrawRectangle(new SolidColorBrush(bgColor), null, new Avalonia.Rect(0, 0, width, height));
            context.DrawLine(new Pen(new SolidColorBrush(waveformColor), 1),
                             new Avalonia.Point(0, height / 2),
                             new Avalonia.Point(width, height / 2));

            if (samples.Count == 0) return;

            float[] arr = samples.ToArray();
            int pixelCount = (int)width;
            if (pixelCount < 1) return;

            int samplesPerPixel = Math.Max(1, arr.Length / pixelCount);

            double lastX = 0, lastY = height / 2;

            for (int x = 0; x < pixelCount; x++)
            {
                int start = x * samplesPerPixel;
                int end = Math.Min(start + samplesPerPixel, arr.Length);

                float maxSample = 0;
                for (int i = start; i < end; i++)
                {
                    if (Math.Abs(arr[i]) > maxSample) maxSample = Math.Abs(arr[i]);
                }

                double y = height / 2 - Math.Sign(arr[start]) * Math.Pow(maxSample, 0.5) * (height / 2);

                context.DrawLine(new Pen(new SolidColorBrush(waveformColor), 1),
                                 new Avalonia.Point(lastX, lastY),
                                 new Avalonia.Point(x, y));

                lastX = x;
                lastY = y;
            }
        }
    }  
}
