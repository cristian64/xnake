using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Xnake
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Texture2D backgroundTexture;
        Texture2D headTexture;
        Texture2D bodyTexture;
        Texture2D foodTexture;
        SoundEffect biteSound;
        SoundEffect ost;
        SoundEffectInstance ostInstance;
        const int HEAD = int.MaxValue;
        const int FOOD = int.MinValue;
        const int CELLSIZE = 16;
        int[][] board;
        int rows;
        int columns;
        enum Direction { UP, DOWN, LEFT, RIGHT };
        Direction lastDirection;
        Direction nextDirection;
        Random random;
        int totalMilliseconds;
        const int millisecondsSleep = 25;
        int length;

        public Game()
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

            // Initialize.
            totalMilliseconds = 0;
            random = new Random();

            // Create colors for every part of the snake and food.
            font = Content.Load<SpriteFont>("font");
            backgroundTexture = Content.Load<Texture2D>("background");
            headTexture = Content.Load<Texture2D>("head");
            bodyTexture = Content.Load<Texture2D>("body");
            foodTexture = Content.Load<Texture2D>("hamburger");
            biteSound = Content.Load<SoundEffect>("bite");
            ost = Content.Load<SoundEffect>("ost");
            ostInstance = ost.CreateInstance();
            ostInstance.IsLooped = true;
            ostInstance.Play();

            // Create board.
            rows = graphics.PreferredBackBufferHeight / CELLSIZE;
            columns = graphics.PreferredBackBufferWidth / CELLSIZE;
            board = new int[columns][];
            for (int i = 0; i < columns; i++)
                board[i] = new int[rows];

            // Place the snake on the board and some food.
            board[random.Next() % columns][random.Next() % rows] = HEAD;
            lastDirection = Direction.UP;
            board[random.Next() % columns][random.Next() % rows] = FOOD;
            length = 1;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Check keys.
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Left) && lastDirection != Direction.RIGHT)
                nextDirection = Direction.LEFT;
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Right) && lastDirection != Direction.LEFT)
                nextDirection = Direction.RIGHT;
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Down) && lastDirection != Direction.UP)
                nextDirection = Direction.DOWN;
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Up) && lastDirection != Direction.DOWN)
                nextDirection = Direction.UP;
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();

            // Check if some time has passed before updating the snake.
            if (totalMilliseconds >= millisecondsSleep)
            {
                totalMilliseconds -= millisecondsSleep;

                // Move snake according to the last direction.
                int row = 0;
                int column = 0;
                for (int i = 0; i < columns; i++)
                    for (int j = 0; j < rows; j++)
                    {
                        if (board[i][j] == HEAD)
                        {
                            column = i;
                            row = j;
                            board[i][j] = length;
                            switch (nextDirection)
                            {
                                case Direction.UP:
                                    row--;
                                    break;
                                case Direction.DOWN:
                                    row++;
                                    break;
                                case Direction.RIGHT:
                                    column++;
                                    break;
                                case Direction.LEFT:
                                    column--;
                                    break;
                            }
                            lastDirection = nextDirection;
                        }
                        else if (board[i][j] > 0)
                            board[i][j]--;
                    }

                // Find the next position for the head (mind you about the limits of the board).
                if (column < 0)
                    column = columns - 1;
                else if (column >= columns)
                    column = 0;
                if (row < 0)
                    row = rows - 1;
                else if (row >= rows)
                    row = 0;
                // Check whether the head of the snake has touched itself or not.
                if (board[column][row] == 0 || board[column][row] == FOOD)
                {
                    // If the head is now on food, take the food and add food somewhere else.
                    if (board[column][row] == FOOD)
                    {
                        length++;
                        board[random.Next() % columns][random.Next() % rows] = FOOD;
                        biteSound.Play();
                    }
                    board[column][row] = HEAD;
                }
                else
                {
                    // Player is dead. Place the snake on the board and some food again.
                    System.Threading.Thread.Sleep(5000);
                    for (int i = 0; i < columns; i++)
                        for (int j = 0; j < rows; j++)
                            board[i][j] = 0;
                    board[random.Next() % columns][random.Next() % rows] = HEAD;
                    lastDirection = Direction.UP;
                    board[random.Next() % columns][random.Next() % rows] = FOOD;
                    length = 1;
                }
            }
            else
            {
                totalMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw every object.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (board[i][j] == HEAD)
                        spriteBatch.Draw(headTexture, new Rectangle(i * CELLSIZE - 2, j * CELLSIZE - 2, CELLSIZE + 4, CELLSIZE + 4), Color.White);
                    else if (board[i][j] == FOOD)
                        spriteBatch.Draw(foodTexture, new Rectangle(i * CELLSIZE - 2, j * CELLSIZE - 2, CELLSIZE + 4, CELLSIZE + 4), Color.White);
                    else if (board[i][j] > 0)
                    {
                        int shrink = CELLSIZE / 3 > board[i][j] ?  CELLSIZE / 3 - board[i][j] : 0;
                        spriteBatch.Draw(bodyTexture, new Rectangle(i * CELLSIZE + shrink, j * CELLSIZE + shrink, CELLSIZE - 2 * shrink, CELLSIZE - 2 * shrink), Color.White);
                    }
                }
            }
            spriteBatch.DrawString(font, (length - 1).ToString(), new Vector2(20, 0), Color.White * 0.7f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
