using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using CyberCommando.Entities;
using CyberCommando.Animations;
using CyberCommando.Controllers;

namespace CyberCommando.Services
{
    /// <summary>
    /// Helps other ingame entities to use other services
    /// </summary>
    class ServiceLocator
    {
        public Camera Camera { get; private set; }
        public AnimationLoader AniLoader { get; private set; }
        public LayerLoader LayLoader { get; private set; }
        public InputHandler IOHandler { get; private set; }
        public AudioManager AudioManager { get; private set; }
        // Got all services!
        public ServiceLocator(ContentManager content, Viewport viewport, int frameWidth, int frameHeight)
        {
            Camera = new Camera(viewport);
            AniLoader = new AnimationLoader(content.RootDirectory);
            LayLoader = new LayerLoader(content, viewport);
            AudioManager = new AudioManager();
            IOHandler = new InputHandler();

            Camera.Position = new Vector2(0f, frameWidth / 2);
            Camera.Zoom = 1.0f;
        }
    }
}
