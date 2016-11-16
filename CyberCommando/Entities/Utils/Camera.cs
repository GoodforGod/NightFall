using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CyberCommando.Entities.Utils
{
    /// <summary>
    /// Represents world camera 
    /// </summary>
    class Camera
    {
        public Vector2  Origin      { get; set; }
        public Viewport viewport    { get; set; }

        public float RotationAngle  { get; set; }

        /// <summary>
        /// Camera zoom value
        /// </summary>
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

        /// <summary>
        /// Camera limit on scrolling
        /// </summary>
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
                        Width = System.Math.Max(viewport.Width, value.Width),
                        Height = System.Math.Max(viewport.Height, value.Height)
                    };

                    // Validate camera position with new limit
                    Position = Position;
                }
                else _Limits = Rectangle.Empty;
            }
        }

        /// <summary>
        /// Camera's current inworld position
        /// </summary>
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
                    _Position = new Vector2(MathHelper.Clamp(_Position.X, Limits.X, Limits.X + Limits.Width - viewport.Width),
                                            MathHelper.Clamp(_Position.Y, Limits.Y, Limits.Y + Limits.Height - viewport.Height));
                }
            }
        }

        public Camera(Viewport viewPort)
        {
            this.viewport = viewPort;
            Origin = new Vector2(viewPort.Width * 0.5f, 
                                 viewPort.Height * 0.5f);
            Zoom = 1.0f;
            RotationAngle = 0.0f;
            Position = Vector2.Zero;
        }

        /// <summary>
        /// Set camera position to look at specific position
        /// </summary>
        /// <param name="position"></param>
        public void LookAt(Vector2 position)
        {
            Position = position - new Vector2(viewport.Width * 0.5f, 
                                              viewport.Height * 0.5f);
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
         * to change coordinate system when you zoom in the camera.
         * in the function “GetViewMatrixn”, change:
         * “new Vector3(-Position.X, -Position.Y, 0))”
         * to “new Vector3(-Position.X * Zoom, -Position.Y * Zoom, 0))”
         */

         /// <summary>
         /// Calculate camera inworld matrix
         /// </summary>
         /// <param name="parallax"></param>
         /// <returns></returns>
        public Matrix GetViewMatrix(Vector2 parallax)
        {
            // Thanks to o KB o for this solution
            return Matrix.CreateTranslation(new Vector3(-_Position * parallax, 0))
                                               * Matrix.CreateTranslation(new Vector3(-Origin, .0f))
                                               * Matrix.CreateRotationZ(RotationAngle)
                                               * Matrix.CreateScale(new Vector3(_Zoom, _Zoom, 1))
                                               * Matrix.CreateTranslation(new Vector3(Origin, .0f));
        } 
    }
}
