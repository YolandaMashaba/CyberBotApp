using System;
using System.IO;
using System.Media;
using System.Windows.Media.Imaging;

namespace CyberBotApp
{
    public class AudioImageHandler
    {
        private SoundPlayer soundPlayer;

        public BitmapImage GetAiLogoImage()
        {
            try
            {
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AILOGO33.png");
                if (File.Exists(imagePath))
                {
                    return new BitmapImage(new Uri(imagePath, UriKind.Absolute));
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public void PlayWelcomeAudio()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greetingai.wav");
                if (File.Exists(audioPath))
                {
                    soundPlayer = new SoundPlayer(audioPath);
                    soundPlayer.Play();
                }
            }
            catch
            {
                // Silent failure
            }
        }
    }
}