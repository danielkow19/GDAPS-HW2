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
        private SpriteFont arial36;
        private SpriteFont arial12;

        // GameObject fields
        private Player player;
        private List<Collectable> collectables;

        // Game-logic fields
        private States state;
        private int level;
        private double timer;
        private Random rand;
        private int playerSpeed;

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
            playerSpeed = 3;
            windowWidth = GraphicsDevice.Viewport.Width;
            windowHeight = GraphicsDevice.Viewport.Height;
            kbState = new KeyboardState();
            previousKbState = new KeyboardState();

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

            // Load all the assets
            playerTexture = this.Content.Load<Texture2D>("spaceship");
            collectableTexture = this.Content.Load<Texture2D>("diamond");
            arial36 = this.Content.Load<SpriteFont>("arial36");
            arial12 = this.Content.Load<SpriteFont>("arial12");

            // Adjusting this makes the player sprite/hitbox any needed size
            playerSizeScale = 1;
            player = new Player(playerTexture, 0, 0,
                (int)(playerTexture.Width * playerSizeScale),
                (int)(playerTexture.Height * playerSizeScale));

            // Adjusting this makes the collectable sprite/hitbox any needed size
            collectableSizeScale = 1.5f;
            collectables = new List<Collectable>();
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

            // Store last frame's keyboard state as well as the current frame's
            previousKbState = kbState;
            kbState = Keyboard.GetState();

            // Updates based on the current gamestate
            switch (state)
            {
                case States.Menu:
                    if (SingleKeyPress(Keys.Enter))
                    {
                        state = States.Game;
                        ResetGame();
                    }
                    break;
                case States.Game:
                    // Lower the timer based on the time it took to process the previous frame
                    timer -= gameTime.ElapsedGameTime.TotalSeconds;

                    // Personal tweak: Speed boost that costs points
                    if (kbState.IsKeyDown(Keys.Space))
                    {
                        playerSpeed = 6;
                        player.TotalScore -= 1;
                        player.LevelScore -= 1;
                    }
                    else
                    {
                        playerSpeed = 3;
                    }

                    // Personal tweak 2: Suction ability that costs time
                    if (kbState.IsKeyDown(Keys.LeftShift))
                    {
                        for (int i = 0; i < collectables.Count; i++)
                        {
                            if (collectables[i].Active)
                            {
                                // If the collectable is active, move it towards the player
                                if (collectables[i].X < player.X) { collectables[i].X += 1; }
                                if (collectables[i].X > player.X) { collectables[i].X -= 1; }
                                if (collectables[i].Y < player.Y) { collectables[i].Y += 1; }
                                if (collectables[i].Y > player.Y) { collectables[i].Y -= 1; }
                            }

                            // Effectively increases the amount the timer decreases by 25%
                            timer -= gameTime.ElapsedGameTime.TotalSeconds / 4;
                        }
                    }

                    // Process movement
                    MovePlayer();
                    ScreenWrap(player);

                    bool anyCollectables = false;

                    for (int i = 0; i < collectables.Count; i++)
                    {
                        // Check to see if the player picked up anything
                        if (collectables[i].CheckCollision(player))
                        {
                            collectables[i].Active = false;
                            player.TotalScore += 500;
                            player.LevelScore += 500;
                        }
                        
                        // Checks if at least one pickup is stil active
                        if (collectables[i].Active) { anyCollectables = true; }
                    }

                    // Checks if the player has run out of time
                    if (timer <= 0) { state = States.GameOver; }

                    // Checks if the player has beaten this level and is ready to go on to the next
                    if (!anyCollectables) { NextLevel(); }

                    break;
                case States.GameOver:
                    if (SingleKeyPress(Keys.Enter))
                    {
                        state = States.Menu;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            switch (state)
            {
                case States.Menu:
                    GraphicsDevice.Clear(Color.Black);

                    spriteBatch.DrawString(arial36, "SPACE AGE SCAVENGER",
                        new Vector2(40, 40), Color.White);

                    spriteBatch.DrawString(arial12, "WASD to move\nSpace to dash (costs points)\n" +
                        "Left Shift to attract objects (costs time)\nEnter to start",
                        new Vector2(40, 200), Color.White);

                    break;
                case States.Game:
                    GraphicsDevice.Clear(Color.CornflowerBlue);

                    // Draw all the collectables
                    for (int i = 0; i < collectables.Count; i++)
                    {
                        collectables[i].Draw(spriteBatch);
                    }

                    player.Draw(spriteBatch);

                    spriteBatch.DrawString(arial12, "Level " + level + "\n" +
                        "Score: " + player.LevelScore + "\n" +
                        "Time: " + String.Format("{0:0.00}", timer),
                        new Vector2(10, 10), Color.Black);

                    break;
                case States.GameOver:
                    GraphicsDevice.Clear(Color.Black);

                    spriteBatch.DrawString(arial36, "GAME OVER",
                        new Vector2(40, 40), Color.White);

                    spriteBatch.DrawString(arial12, "Level reached: " + level + "\n" +
                        "Final score: " + player.TotalScore + "\n" +
                        "Press enter to return to menu",
                        new Vector2(40, 200), Color.White);

                    break;
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Sets up a new level, centering the player and randomizing collectables
        /// </summary>
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

        /// <summary>
        /// Resets the game and makes level 1
        /// </summary>
        private void ResetGame()
        {
            level = 0;
            player.TotalScore = 0;
            NextLevel();
        }

        /// <summary>
        /// Moves an object to the oposite edge of a screen if over half of its hitbox is over the edge
        /// </summary>
        /// <param name="objToWrap">The object to wrap around the screen</param>
        private void ScreenWrap(GameObject objToWrap)
        {
            // Horizontal wrapping
            if (player.X > windowWidth - player.Position.Width / 2)
            {
                player.X = 0 - player.Position.Width / 2;
            }
            else if (player.X < 0 - player.Position.Width / 2)
            {
                player.X = windowWidth - player.Position.Width / 2;
            }

            // Vertical wrapping
            if (player.Y > windowHeight - player.Position.Height / 2)
            {
                player.Y = 0 - player.Position.Height / 2;
            }
            else if (player.Y < 0 - player.Position.Height / 2)
            {
                player.Y = windowHeight - player.Position.Height / 2;
            }
        }

        /// <summary>
        /// Checks if this is the first frame a key has been pressed
        /// </summary>
        /// <param name="key">The key to check</param>
        private bool SingleKeyPress (Keys key)
        {
            // Was it up last frame AND is it down this frame
            return previousKbState.IsKeyUp(key) && kbState.IsKeyDown(key);
        }

        /// <summary>
        /// Moves the player according to the key(s) pressed
        /// </summary>
        private void MovePlayer()
        {
            if (kbState.IsKeyDown(Keys.W)) { player.Y -= playerSpeed; }
            if (kbState.IsKeyDown(Keys.S)) { player.Y += playerSpeed; }
            if (kbState.IsKeyDown(Keys.A)) { player.X -= playerSpeed; }
            if (kbState.IsKeyDown(Keys.D)) { player.X += playerSpeed; }
        }
    }
}
