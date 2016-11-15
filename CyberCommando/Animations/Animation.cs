using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Animations
{
    /// <summary>
    /// Indicates current animation state
    /// </summary> 
    [Flags]
    public enum AnimationState
    {
        NONE = 0,
        IDLE = 1,
        WALK = 2,
        JUMP = 4,
        FALL = 8,
        DUCK = 16,
        FIRE = 32
    }

    /// <summary>
    /// Class stores all frames <see cref="Frame"/> for the specific animation, is used for entity
    /// </summary>
    /// <remarks>
    /// TimeSpan for animation duration <see cref="Duration"/> 
    /// TimeSpan for time elapsed into animation <see cref="TimeIntoAnimation"/>
    /// List of Frames in animation <see cref="Frame"/>, 
    /// SpriteEffects for animation <see cref="Effect"/>
    /// Current rectangle of the animation frame <see cref="CurrentRectangle"/>
    /// Method to add animations <see cref="AddFrame(Rectangle, TimeSpan)"/>
    /// </remarks>
    class Animation
    {
        /// <summary>
        /// List of Frames in animation <see cref="Frame"/>
        /// </summary>
        public List<Frame> FrameList        { get; private set; }

        /// <summary>
        ///  TimeSpan for time elapsed into animation
        /// </summary>
        public TimeSpan TimeIntoAnimation   { get; set; }
        public DateTime SingleAnimStartTime { get; set; }

        public bool     SingleAnimFlag      { get; set; }
        /// <summary>
        /// SpriteEffects for animation
        /// </summary>
        public SpriteEffects Effect         { get; set; }

        // Constactors
        public Animation()                              { FrameList = new List<Frame>(); }
        public Animation(SpriteEffects effect) : this() { Effect = effect; }

        /// <summary>
        /// Represents the total duration of the animation in TotalSeconds
        /// </summary>
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

        /// <summary>
        /// Uses <see cref="TimeIntoAnimation"/> to go through all frames and get the Rectangle of the current frame
        /// </summary>
        public Rectangle CurrentRectangle
        {
            get
            {
                Frame currentFrame = null;
                var accumulatedTime = new TimeSpan();

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


        /// <summary>
        /// Gets rectangle of the sprite, and the duration of the frame, creates new frame in animation
        /// </summary>
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
