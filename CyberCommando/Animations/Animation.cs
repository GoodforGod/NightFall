using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Animations
{
    class Animation
    {
        private List<Frame> FrameList = new List<Frame>();

        public TimeSpan TimeIntoAnimation { get; set; }
        public int SingleAnimationCounter = 0;
        public DateTime SingleAnimationStartTime;
        public SpriteEffects Effect { get; set; }
        public string Name { get; }

        public Animation(string name) { Name = Name; }

        public Animation(string name, SpriteEffects effect)
        {
            Name = name;
            Effect = effect;
        }

        public TimeSpan Duration
        {
            get
            {
                double totalSeconds = 0;
                foreach (var frame in FrameList)
                    totalSeconds += frame.Duration.TotalSeconds;
                return TimeSpan.FromSeconds(totalSeconds);
            }
        }

        public Rectangle CurrentRectangle
        {
            get
            {
                Frame currentFrame = null;
                TimeSpan accumulatedTime = new TimeSpan();

                foreach (var frame in FrameList)
                {
                    if (accumulatedTime + frame.Duration >= TimeIntoAnimation)
                    {
                        currentFrame = frame;
                        break;
                    }
                    else accumulatedTime += frame.Duration;
                }

                if (currentFrame == null)
                    currentFrame = FrameList.LastOrDefault();
                if (currentFrame != null)
                    return currentFrame.Rectangle;
                else
                    return Rectangle.Empty;
            }
        }

        public void AddFrame(Rectangle rectangle, TimeSpan duration)
        {
            FrameList.Add(new Frame()
            {
                Rectangle = rectangle,
                Duration = duration
            });
        }

        public double UpdateSingleAnimationTimeSlapsed(GameTime gameTime)
        {
            if (SingleAnimationCounter == 0)
            {
                SingleAnimationStartTime = DateTime.Now;
                SingleAnimationCounter = 1;
            }
            double secondsIntoAnimation =
                TimeIntoAnimation.TotalSeconds + gameTime.ElapsedGameTime.TotalSeconds;

            double remainder = secondsIntoAnimation % Duration.TotalSeconds;

            if (remainder == double.NaN || Duration.TotalSeconds == 0)
                remainder = 0;

            TimeIntoAnimation = TimeSpan.FromSeconds(remainder);

            var TimeElapsed = (DateTime.Now - SingleAnimationStartTime).TotalMilliseconds - Duration.TotalMilliseconds / 100;

            if (TimeElapsed > Duration.TotalMilliseconds)
            {
                SingleAnimationCounter = 0;
                TimeIntoAnimation = TimeSpan.FromSeconds(0);
                return -1;
            }
            else return TimeElapsed;
        }

        public bool UpdateSingleAnimationIsEnded(GameTime gameTime)
        {
            if (SingleAnimationCounter == 0)
            {
                SingleAnimationStartTime = DateTime.Now;
                SingleAnimationCounter = 1;
            }
            double secondsIntoAnimation =
                TimeIntoAnimation.TotalSeconds + gameTime.ElapsedGameTime.TotalSeconds;

            double remainder = secondsIntoAnimation % Duration.TotalSeconds;

            TimeIntoAnimation = TimeSpan.FromSeconds(remainder);

            if ((DateTime.Now - SingleAnimationStartTime).TotalMilliseconds - Duration.TotalMilliseconds / 100 > Duration.TotalMilliseconds)
            {
                SingleAnimationCounter = 0;
                TimeIntoAnimation = TimeSpan.FromSeconds(0);
                return false;
            }
            else return true;
        }

        public void UpdateCycleAnimation(GameTime gameTime)
        {
            double secondsIntoAnimation =
                TimeIntoAnimation.TotalSeconds + gameTime.ElapsedGameTime.TotalSeconds;

            double remainder = secondsIntoAnimation % Duration.TotalSeconds;

            TimeIntoAnimation = TimeSpan.FromSeconds(remainder);
        }
    }
}
