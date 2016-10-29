using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Animations
{
    public enum AnimationState
    {
        NONE,
        IDLE,
        WALK,
        JUMP,
        FALL,
        DUCK,
        FIRE
    }

    class Animation
    {
        public List<Frame> FrameList { get; private set; }

        public TimeSpan TimeIntoAnimation { get; set; }
        public DateTime SingleAnimStartTime { get; set; }

        public bool SingleAnimFlag { get; set; }
        public SpriteEffects Effect { get; set; }

        public Animation() { FrameList = new List<Frame>(); }
        public Animation(SpriteEffects effect) { FrameList = new List<Frame>(); Effect = effect; }

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
    }
}
