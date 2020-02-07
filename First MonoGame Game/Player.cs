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
    // Purpose: Create a Game Object that can keep track of scores. To be used as the player-controlled object
    // Restrictions: Can only access public info from the GameObject class
    class Player : GameObject
    {
        // Fields
        private int levelScore;
        private int totalScore;

        // Properties
        public int LevelScore
        {
            get { return levelScore; }
            set { levelScore = value; }
        }
        public int TotalScore
        {
            get { return totalScore; }
            set { totalScore = value; }
        }

        /// <summary>
        /// Create a new instance of a Player Game Object
        /// </summary>
        public Player (Texture2D texture, int x, int y, int width, int height)
            : base (texture, x, y, width, height)
        {
            levelScore = 0;
            totalScore = 0;
        }
    }
}
