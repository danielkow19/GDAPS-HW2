using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public enum States { Menu, Game, GameOver };

namespace First_MonoGame_Game
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // Pre-generated fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Texture/Drawing Fields
        private Texture2D playerTexture;
        private float playerSizeScale;
        private Texture2D collectableTexture;
        private float collectableSizeScale;
        //private SpriteFont INSERT TITLE/GAMEOVER FONT NAME HERE
        //private SpriteFont INSERT SMALLER FONT NAME HERE

        // GameObject fields
        private Player player;
        private List<Collectable> collectables;

        // Game-logic fields
        private States state;
        private int level;
        private double timer;
        private Random rand;

        // Input fields
        private KeyboardState kbState;
        private KeyboardState previousKbState;

        // Window fields
        private int windowWidth;
        private int windowHeight;

        // Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            state = States.Menu;
            level = 0; // Calling NextLevel will increment this at the start, so it must start at 0
            timer = 10;
            rand = new Random();
            windowWidth = GraphicsDevice.Viewport.Width;
            windowHeight = GraphicsDevice.Viewport.Height;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void NextLevel()
        {
            level++;
            timer = 10;

            player.LevelScore = 0;

            // Centers the player on screen
            player.X = windowWidth / 2 + player.Position.Width / 2;
            player.Y = windowHeight / 2 + player.Position.Height / 2;

            collectables.Clear();

            // Start with 5 collectables, increase by 3 every level
            int numPickups = 2 + 3 * level;

            for (int i = 0; i < numPickups; i++)
            {
                // Randomize the item's position such that it's always in view of the screen
                int x = rand.Next(windowWidth - (int)(collectableTexture.Width * collectableSizeScale));
                int y = rand.Next(windowHeight - (int)(collectableTexture.Height * collectableSizeScale));

                // Create the collectable object and add it to the list
                collectables.Add(new Collectable(collectableTexture, x, y,
                    (int)(collectableTexture.Width * collectableSizeScale),
                    (int)(collectableTexture.Height * collectableSizeScale)));
            }
        }
    }
}
