﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using CyberCommando.Entities.Utils;
using CyberCommando.Controllers;

namespace CyberCommando.Services
{
    /// <summary>
    /// Singlton which helps other ingame entities to use other services
    /// </summary>
    public class ServiceLocator
    {
        internal GraphicsDevice GraphDev { get; private set; }

        internal Camera             Camera      { get; private set; }
        internal LoadManager        LManager    { get; private set; }
        internal InputHandler       IOHandler   { get; private set; }
        internal AudioManager       AUManager   { get; private set; }
        internal PipelineManager    PLManager   { get; private set; }
        internal LevelManager       LVLManager  { get; private set; }
        internal ScreenManager      SManager    { get; private set; }

        public bool                 IsInitialized { get; private set; }

        private static ServiceLocator _Instance;
        public static ServiceLocator Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ServiceLocator();
                return _Instance;
            }
        }

        private ServiceLocator() { }

        // Got all services!
        public void Initialize(ContentManager content, GraphicsDevice graphdev, int frameWidth, int frameHeight)
        {
            this.GraphDev = graphdev;
            SManager = ScreenManager.Instance;
            LManager = LoadManager.Instance;
            LManager.Initialize(content, GraphDev.Viewport);
            AUManager = AudioManager.Instance;
            IOHandler = InputHandler.Instance;
            PLManager = PipelineManager.Instance;
            PLManager.Initialize(content);
            LVLManager = LevelManager.Instance;
            LVLManager.Initialize();

            Camera = new Camera(GraphDev.Viewport);
            Camera.Position = new Vector2(0f, frameWidth / 2);
            Camera.Zoom = 1.0f;

            IsInitialized = true;
        }

        public void Resize(ResolutionState res, int width, int height)
        {
            Camera.viewport = GraphDev.Viewport;
        }

        public void Unload()
        {
            PLManager.Unload();
            //SManager.UnloadContent();
            LVLManager.Unload();
            AUManager.Unload();
        }
    }
}
