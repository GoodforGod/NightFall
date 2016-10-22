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
        public Vector2 Origin { get; set; }
        private Viewport viewPort; 

        public float RotationAngle { get; set; }

        private float _Zoom;
        public float Zoom
        {
            get { return _Zoom; }
            set
            {
                if (value < 0.01f)
                    _Zoom = 0.01f;
                else _Zoom = value;
            }
        }

        private Rectangle _Limits;
        public Rectangle Limits
        {
            get { return _Limits; }
            set
            {
                if (value != null)
                {
                    // Assign limit, should always be bigger then viewport
                    _Limits = new Rectangle
                    {
                        X = value.X,
                        Y = value.Y,
                        Width = System.Math.Max(viewPort.Width, value.Width),
                        Height = System.Math.Max(viewPort.Height, value.Height)
                    };

                    // Validate camera position with new limit
                    Position = Position;
                }
                else _Limits = Rectangle.Empty;
            }
        }

        private Vector2 _Position;
        public Vector2 Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
                // If there's a limit set and the camera is not transformed clamp position to limits
                if (_Limits != null && Zoom == 1.0f && RotationAngle == 0.0f)
                {
                    _Position = new Vector2(MathHelper.Clamp(_Position.X, Limits.X, Limits.X + Limits.Width - viewPort.Width),
                                            MathHelper.Clamp(_Position.Y, Limits.Y, Limits.Y + Limits.Height - viewPort.Height));
                }
            }
        }

        public Camera(Viewport viewPort)
        {
            this.viewPort = viewPort;
            Origin = new Vector2(viewPort.Width * 0.5f, 
                                 viewPort.Height * 0.5f);
            Zoom = 1.0f;
            RotationAngle = 0.0f;
            Position = Vector2.Zero;
        }

        public void LookAt(Vector2 position)
        {
            Position = position - new Vector2(viewPort.Width * 0.5f, 
                                              viewPort.Height * 0.5f);
        }

        public void Move(Vector2 position, bool respectRotation = false)
        {
            //TRUE to move in accordance with the camera’s rotation
            if (respectRotation)
                position = Vector2.Transform(position, Matrix.CreateRotationZ(-RotationAngle));
            Position += position;
        }

        public void Move(Vector2 position) { Position += position; }

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
