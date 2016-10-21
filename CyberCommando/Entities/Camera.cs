using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Entities
{
    class Camera
    {
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }

        private float zoom;
        public float Zoom {
            get { return zoom; }
            set
            {
                if (value < 0.01f)
                    zoom = 0.01f;
                else zoom = value;
            }
        }

        public float RotationAngle { get; set; }

        public Camera(Viewport viewPort)
        {
            Origin = new Vector2(viewPort.Width * 0.5f, viewPort.Height * 0.5f);
            Zoom = 1.0f;
            RotationAngle = 0.0f;
            Position = Vector2.Zero;
        }

        public void Move(Vector2 amount) { Position += amount; }

        /*
         * to cahnge coordinate system when you zoom in the camera.
         * in the function “get_transformation”, change:
         * “new Vector3(-Position.X, -Position.Y, 0))”
         * to “new Vector3(-Position.X * Zoom, -Position.Y * Zoom, 0))”
         */

        public Matrix GetViewMatrix(Vector2 parallax)
        {
            // Thanks to o KB o for this solution
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0)) *
                                       Matrix.CreateTranslation(new Vector3(-Origin, .0f)) *
                                       Matrix.CreateRotationZ(RotationAngle) *
                                       Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                       Matrix.CreateTranslation(new Vector3(Origin, .0f));
        } 
    }
}
