using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using AStar;

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
        SpriteFont font2;
        Texture2D backgroundTexture;
        Texture2D headTexture;
        Texture2D bodyTexture;
        Texture2D foodTexture;
        Texture2D astarTexture;
        SoundEffect biteSound;
        SoundEffect ost;
        SoundEffect deadSound;
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
        int partialMilliseconds;
        int millisecondsSleep;
        int totalTime;
        int lastSecond;
        const int SLOW = 100;
        const int NORMAL = 35;
        const int FAST = 10;
        int length;
        int score;
        int foods;
        bool paused;
        bool justPaused;
        bool dead;
        bool automatic;
        ArrayList path;

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
            partialMilliseconds = 0;
            millisecondsSleep = NORMAL;
            random = new Random();
            totalTime = 0;
            lastSecond = 0;

            // Create colors for every part of the snake and food.
            font = Content.Load<SpriteFont>("font");
            font2 = Content.Load<SpriteFont>("font2");
            backgroundTexture = Content.Load<Texture2D>("background");
            headTexture = Content.Load<Texture2D>("head");
            bodyTexture = Content.Load<Texture2D>("body");
            foodTexture = Content.Load<Texture2D>("hamburger");
            astarTexture = Content.Load<Texture2D>("astar");
            biteSound = Content.Load<SoundEffect>("bite");
            ost = Content.Load<SoundEffect>("ost");
            deadSound = Content.Load<SoundEffect>("dead");
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
            board[17][15] = HEAD;
            lastDirection = Direction.UP;
            length = 1;
            score = 0;
            foods = 0;
            paused = false;
            justPaused = false;
            dead = false;
            automatic = false;
            path = new ArrayList();
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
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.F1))
            {
                if (!automatic)
                {
                    path = new ArrayList();
                }
                automatic = true;
            }
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Left) && lastDirection != Direction.RIGHT)
            {
                nextDirection = Direction.LEFT;
                automatic = false;
            }
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Right) && lastDirection != Direction.LEFT)
            {
                nextDirection = Direction.RIGHT;
                automatic = false;
            }
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Down) && lastDirection != Direction.UP)
            {
                nextDirection = Direction.DOWN;
                automatic = false;
            }
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Up) && lastDirection != Direction.DOWN)
            {
                nextDirection = Direction.UP;
                automatic = false;
            }
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();
            else if (dead && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Enter))
            {
                dead = false;
                for (int i = 0; i < columns; i++)
                    for (int j = 0; j < rows; j++)
                        board[i][j] = 0;
                board[17][15] = HEAD;
                length = 1;
                score = 0;
                foods = 0;
            }
            else if (!dead && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Space))
            {
                if (!justPaused)
                {
                    paused = !paused;
                    justPaused = true;
                    ostInstance.Volume = paused ? 0.2f : 1.0f;
                }
            }
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D1) || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.NumPad1))
                millisecondsSleep = SLOW;
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D2) || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.NumPad2))
                millisecondsSleep = NORMAL;
            else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D3) || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.NumPad3))
                millisecondsSleep = FAST;

            if (Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Space))
                justPaused = false;

            if (!paused && !dead)
            {
                // Check if some time has passed before updating the snake.
                if (partialMilliseconds >= millisecondsSleep)
                {
                    partialMilliseconds -= millisecondsSleep;

                    // Execute A* to find a path
                    if (automatic && foods > 0)
                    {
                        // Check where the head is.
                        int headColumn = 0;
                        int headRow = 0;
                        for (int i = 0; i < columns; i++)
                            for (int j = 0; j < rows; j++)
                                if (board[i][j] == HEAD)
                                {
                                    headColumn = i;
                                    headRow = j;
                                }

                        // If there is no  path, it finds it.
                        if (path == null || path.Count == 0)
                        {
                            Node startNode = null;
                            Node endNode = null;
                            Node[][] nodes = new Node[columns][];
                            for (int i = 0; i < columns; i++)
                            {
                                nodes[i] = new Node[rows];
                                for (int j = 0; j < rows; j++)
                                {
                                    nodes[i][j] = new Node(i, j);
                                    if (board[i][j] == FOOD) // TODO improve ramdonly the endNode
                                        endNode = nodes[i][j];
                                    else if (board[i][j] == HEAD)
                                        startNode = nodes[i][j];
                                    else if (board[i][j] > Math.Abs(i - headColumn) + Math.Abs(j - headRow))
                                        nodes[i][j].Transitable = false;
                                }
                            }

                            AStar.AStar astar = new AStar.AStar(nodes, startNode, endNode);
                            path = astar.FindPath();
                            if (path != null && path.Count > 0)
                                path.RemoveAt(0);
                        }

                        // If there is a path, it takes the next move.
                        if (path != null && path.Count > 0)
                        {
                            Node node = (Node)path[0];
                            path.RemoveAt(0);

                            if (node.Column < headColumn)
                                nextDirection = Direction.LEFT;
                            else if (node.Column > headColumn)
                                nextDirection = Direction.RIGHT;
                            else if (node.Row < headRow)
                                nextDirection = Direction.UP;
                            else
                                nextDirection = Direction.DOWN;
                        }
                    }

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
                            score += 100 / millisecondsSleep;
                            biteSound.Play();
                            foods--;
                        }
                    }
                    else
                    {
                        // Player is dead.
                        dead = true;
                        deadSound.Play();
                    }
                    board[column][row] = HEAD;

                    // Every few seconds, check if more food has to be placed.
                    int currentSecond = totalTime / 1000;
                    if (foods == 0 || lastSecond != currentSecond && currentSecond % (3 + length / 20) == 0)
                    {
                        lastSecond = currentSecond;
                        if (score / 50 >= foods)
                        {
                            int rowCandidate = random.Next() % rows;
                            int columnCandidate = random.Next() % columns;
                            if (board[columnCandidate][rowCandidate] == 0)
                            {
                                board[columnCandidate][rowCandidate] = FOOD;
                                foods++;
                            }
                        }
                    }
                }
                else
                {
                    partialMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            totalTime += gameTime.ElapsedGameTime.Milliseconds;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw every object.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);
            if (automatic && path != null)
                foreach (Node i in path)
                {
                    spriteBatch.Draw(astarTexture, new Rectangle(i.Column * CELLSIZE, i.Row * CELLSIZE, CELLSIZE, CELLSIZE), Color.White * 0.7f);
                }
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (board[i][j] == HEAD)
                    {
                        float rotation = 0.0f;
                        Vector2 origin = new Vector2(0.0f);
                        if (lastDirection == Direction.DOWN)
                        {
                            rotation = (float)Math.PI;
                            origin = new Vector2(headTexture.Width, headTexture.Height);
                        }
                        else if (lastDirection == Direction.LEFT)
                        {
                            rotation = -(float)Math.PI / 2;
                            origin = new Vector2(headTexture.Width, 0.0f);
                        }
                        else if (lastDirection == Direction.RIGHT)
                        {
                            rotation = (float)Math.PI / 2;
                            origin = new Vector2(0.0f, headTexture.Height);
                        }
                        spriteBatch.Draw(headTexture, new Rectangle(i * CELLSIZE - 2, j * CELLSIZE - 2, CELLSIZE + 4, CELLSIZE + 4), null, Color.White, rotation, origin, SpriteEffects.None, 0.0f);
                    }
                    else if (board[i][j] == FOOD)
                        spriteBatch.Draw(foodTexture, new Rectangle(i * CELLSIZE - 2, j * CELLSIZE - 2, CELLSIZE + 4, CELLSIZE + 4), Color.White);
                    else if (board[i][j] > 0)
                    {
                        int shrink = CELLSIZE / 3 - board[i][j] - 1;
                        shrink = shrink > 0 ? shrink : 0;
                        spriteBatch.Draw(bodyTexture, new Rectangle(i * CELLSIZE + shrink, j * CELLSIZE + shrink, CELLSIZE - 2 * shrink, CELLSIZE - 2 * shrink), Color.White);
                    }
                }
            }
            spriteBatch.DrawString(font, score.ToString(), new Vector2(20, 0), Color.White * 0.7f);
            float alpha = (totalTime < 5000) ? 1.0f : (1.0f - (totalTime - 5000) / 5000.0f);
            if (alpha > 0)
            {
                spriteBatch.DrawString(font2, "Press arrows to change direction", new Vector2(20, 360), Color.White * alpha);
                spriteBatch.DrawString(font2, "Press [Esc] to exit", new Vector2(20, 380), Color.White * alpha);
                spriteBatch.DrawString(font2, "Press [Space] to pause the game", new Vector2(20, 400), Color.White * alpha);
                spriteBatch.DrawString(font2, "Press [1], [2] or [3] to change speed", new Vector2(20, 420), Color.White * alpha);
                spriteBatch.DrawString(font2, "Press [F1] to active Artificial Intelligence", new Vector2(20, 440), Color.White * alpha);
            }
            if (dead)
            {
                spriteBatch.DrawString(font, "Game Over!", new Vector2(265, 180), Color.Red * 0.7f);
                spriteBatch.DrawString(font2, "Press [Enter] to restart", new Vector2(320, 250), Color.Red * 0.7f);
            }
            if (paused)
            {
                spriteBatch.DrawString(font, "Paused", new Vector2(320, 180), Color.Blue * 0.7f);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
