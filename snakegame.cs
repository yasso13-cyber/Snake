using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConsoleSnake
{
    class Program
    {
        // Board dimensions
        const int Width = 40;
        const int Height = 20;

        // Game state variables
        static List<Position> snake = new List<Position>();
        static Position food;
        static Direction currentDirection = Direction.Right;
        static int score = 0;
        static bool gameOver = false;
        static Random random = new Random();

        struct Position
        {
            public int X;
            public int Y;
            public Position(int x, int y) { X = x; Y = y; }
        }

        enum Direction { Up, Down, Left, Right }

        static void Main(string[] args)
        {
            SetupGame();

            // Main Game Loop
            while (!gameOver)
            {
                if (Console.KeyAvailable)
                {
                    HandleInput();
                }

                MoveSnake();
                CheckCollisions();

                if (!gameOver)
                {
                    DrawGame();
                    Thread.Sleep(100); // Controls game speed
                }
            }

            DisplayGameOver();
        }

        static void SetupGame()
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(Width + 2, Height + 4);
            Console.SetBufferSize(Width + 2, Height + 4);

            // Initialize Snake in the middle with 3 segments
            snake.Clear();
            snake.Add(new Position(Width / 2, Height / 2));
            snake.Add(new Position((Width / 2) - 1, Height / 2));
            snake.Add(new Position((Width / 2) - 2, Height / 2));

            currentDirection = Direction.Right;
            score = 0;
            gameOver = false;

            SpawnFood();
            DrawGame();
        }

        static void HandleInput()
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    if (currentDirection != Direction.Down) currentDirection = Direction.Up;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    if (currentDirection != Direction.Up) currentDirection = Direction.Down;
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    if (currentDirection != Direction.Right) currentDirection = Direction.Left;
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    if (currentDirection != Direction.Left) currentDirection = Direction.Right;
                    break;
            }
        }

        static void MoveSnake()
        {
            Position head = snake.First();
            Position newHead = head;

            switch (currentDirection)
            {
                case Direction.Up: newHead.Y--; break;
                case Direction.Down: newHead.Y++; break;
                case Direction.Left: newHead.X--; break;
                case Direction.Right: newHead.X++; break;
            }

            // Insert new head position
            snake.Insert(0, newHead);

            // Check if food is eaten
            if (newHead.X == food.X && newHead.Y == food.Y)
            {
                score += 10;
                SpawnFood();
            }
            else
            {
                // Remove tail segment if food wasn't eaten to keep size consistent
                snake.RemoveAt(snake.Count - 1);
            }
        }

        static void CheckCollisions()
        {
            Position head = snake.First();

            // Wall collisions
            if (head.X < 0 || head.X >= Width || head.Y < 0 || head.Y >= Height)
            {
                gameOver = true;
                return;
            }

            // Self-collision (skipping the head itself)
            for (int i = 1; i < snake.Count; i++)
            {
                if (head.X == snake[i].X && head.Y == snake[i].Y)
                {
                    gameOver = true;
                    return;
                }
            }
        }

        static void SpawnFood()
        {
            while (true)
            {
                int x = random.Next(0, Width);
                int y = random.Next(0, Height);

                // Ensure food does not spawn inside the snake body
                if (!snake.Any(s => s.X == x && s.Y == y))
                {
                    food = new Position(x, y);
                    break;
                }
            }
        }

        static void DrawGame()
        {
            Console.SetCursorPosition(0, 0);

            // Draw Top Border
            Console.WriteLine(new string('#', Width + 2));

            for (int y = 0; y < Height; y++)
            {
                Console.Write("#"); // Left Border

                for (int x = 0; x < Width; x++)
                {
                    if (snake.First().X == x && snake.First().Y == y)
                    {
                        Console.Write("O"); // Snake Head
                    }
                    else if (snake.Skip(1).Any(s => s.X == x && s.Y == y))
                    {
                        Console.Write("o"); // Snake Body
                    }
                    else if (food.X == x && food.Y == y)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("@"); // Food
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(" "); // Empty Space
                    }
                }

                Console.WriteLine("#"); // Right Border
            }

            // Draw Bottom Border
            Console.WriteLine(new string('#', Width + 2));
            Console.WriteLine($"Score: {score}");
        }

        static void DisplayGameOver()
        {
            Console.Clear();
            Console.SetCursorPosition(Width / 4, Height / 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("GAME OVER!");
            Console.ResetColor();
            Console.SetCursorPosition(Width / 4, (Height / 2) + 1);
            Console.WriteLine($"Final Score: {score}");
            Console.SetCursorPosition(Width / 4, (Height / 2) + 2);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
