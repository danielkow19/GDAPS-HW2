using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace First_MonoGame_Game
{
    // Author: Daniel Kowalski
    // Purpose: Provide a base for more specific game objects to inherit from
    // Restrictions: On its own, it can only move and be drawn.
    class GameObject
    {
        // Fields
        private Texture2D texture;
        private Rectangle position;

        // Properties
        public Rectangle Position { get { return position; } }
        public int X
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public int Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        /// <summary>
        /// Create a new instance of a Game Object
        /// </summary>
        public GameObject(Texture2D texture, int x, int y, int width, int height)
        {
            this.texture = texture;
            this.position = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Draws the Game Object's texture, adding it to the given SpriteBatch
        /// </summary>
        /// <param name="sb">A SpriteBatch that has begun but not ended</param>
        public virtual void Draw (SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }
    }
}
