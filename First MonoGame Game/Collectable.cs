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
    // Purpose: Create a collectable Game Object that can sense when it's been collided with
    // and can become inactive
    // Restrictions: "Active" Property can only be set to false from outside this class
    class Collectable : GameObject
    {
        // Fields
        private bool active;

        // Properties
        public bool Active
        {
            get { return active; }

            // There is never a situation where an inactive collectable should be reactivated
            set { if (value == false) { active = value; } }
        }

        /// <summary>
        /// Creates a new instance of a collectable game object
        /// </summary>
        public Collectable (Texture2D texture, int x, int y, int width, int height)
            : base (texture, x, y, width, height)
        {
            active = true;
        }

        /// <summary>
        /// Checks if this is an active collectable that is colliding with another game object
        /// </summary>
        /// <param name="check">The Game Object to check against</param>
        /// <returns>True if colliding, false if not and false if the collectable is inactive</returns>
        public bool CheckCollision (GameObject check)
        {
            if (!active) { return false; }
            return base.Position.Intersects(check.Position);
        }

        /// <summary>
        /// If this Collectable Game Object is active, draws it to the SpriteBatch
        /// </summary>
        /// <param name="sb">The SpriteBatch to draw to</param>
        public override void Draw(SpriteBatch sb)
        {
            if (active) { base.Draw(sb); }
        }
    }
}
