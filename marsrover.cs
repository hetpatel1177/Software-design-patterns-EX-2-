using System;
using System.Collections.Generic;

abstract class Direction
{
    public abstract void Move(Rover rover, Grid grid);
    public abstract Direction Left();
    public abstract Direction Right();
    public abstract string Name { get; }
}

class North : Direction
{
    public override void Move(Rover rover, Grid grid)
    {
        if (grid.IsValid(rover.X, rover.Y + 1)) rover.Y++;
    }
    public override Direction Left() => new West();
    public override Direction Right() => new East();
    public override string Name => "North";
}

class South : Direction
{
    public override void Move(Rover rover, Grid grid)
    {
        if (grid.IsValid(rover.X, rover.Y - 1)) rover.Y--;
    }
    public override Direction Left() => new East();
    public override Direction Right() => new West();
    public override string Name => "South";
}

class East : Direction
{
    public override void Move(Rover rover, Grid grid)
    {
        if (grid.IsValid(rover.X + 1, rover.Y)) rover.X++;
    }
    public override Direction Left() => new North();
    public override Direction Right() => new South();
    public override string Name => "East";
}

class West : Direction
{
    public override void Move(Rover rover, Grid grid)
    {
        if (grid.IsValid(rover.X - 1, rover.Y)) rover.X--;
    }
    public override Direction Left() => new South();
    public override Direction Right() => new North();
    public override string Name => "West";
}

interface ICommand
{
    void Execute(Rover rover, Grid grid);
}

class MoveCommand : ICommand
{
    public void Execute(Rover rover, Grid grid) => rover.Direction.Move(rover, grid);
}

class LeftCommand : ICommand
{
    public void Execute(Rover rover, Grid grid) => rover.Direction = rover.Direction.Left();
}

class RightCommand : ICommand
{
    public void Execute(Rover rover, Grid grid) => rover.Direction = rover.Direction.Right();
}

class Rover
{
    public int X { get; set; }
    public int Y { get; set; }
    public Direction Direction { get; set; }

    public Rover(int x, int y, Direction direction)
    {
        X = x; Y = y; Direction = direction;
    }

    public void Execute(ICommand cmd, Grid grid) => cmd.Execute(this, grid);

    public string Status(Grid grid)
    {
        string obstacleMsg = grid.HasObstacle(X, Y) ? "Obstacle detected." : "No Obstacles detected.";
        return $"Rover is at ({X}, {Y}) facing {Direction.Name}. {obstacleMsg}";
    }
}

class Grid
{
    private readonly int width, height;
    private readonly HashSet<string> obstacles;

    public Grid(int width, int height, HashSet<string> obstacles)
    {
        this.width = width;
        this.height = height;
        this.obstacles = obstacles;
    }

    public bool IsValid(int x, int y)
    {
        return x >= 0 && x < width &&
               y >= 0 && y < height &&
               !obstacles.Contains($"{x},{y}");
    }

    public bool HasObstacle(int x, int y) => obstacles.Contains($"{x},{y}");
}

class Program
{
    static void Main()
    {
        // Grid size
        Console.Write("Enter grid width: ");
        int width = int.Parse(Console.ReadLine());
        Console.Write("Enter grid height: ");
        int height = int.Parse(Console.ReadLine());

        // Starting position
        Console.Write("Enter starting X: ");
        int startX = int.Parse(Console.ReadLine());
        Console.Write("Enter starting Y: ");
        int startY = int.Parse(Console.ReadLine());
        Console.Write("Enter starting direction (N/S/E/W): ");
        string dirInput = Console.ReadLine().ToUpper();

        Direction startDir = dirInput switch
        {
            "N" => new North(),
            "S" => new South(),
            "E" => new East(),
            "W" => new West(),
            _ => throw new ArgumentException("Invalid direction")
        };

        // Obstacles
        Console.Write("Enter number of obstacles: ");
        int obsCount = int.Parse(Console.ReadLine());
        var obstacles = new HashSet<string>();
        for (int i = 0; i < obsCount; i++)
        {
            Console.Write($"Enter obstacle {i + 1} (x y): ");
            var parts = Console.ReadLine().Split();
            obstacles.Add($"{parts[0]},{parts[1]}");
        }

        var grid = new Grid(width, height, obstacles);
        var rover = new Rover(startX, startY, startDir);

        // Commands
        Console.Write("Enter commands (M/L/R without spaces): ");
        string commandStr = Console.ReadLine().ToUpper();
        var commands = new List<ICommand>();

        foreach (char c in commandStr)
        {
            commands.Add(c switch
            {
                'M' => new MoveCommand(),
                'L' => new LeftCommand(),
                'R' => new RightCommand(),
                _ => throw new ArgumentException("Invalid command")
            });
        }

        // Execute
        foreach (var cmd in commands)
        {
            rover.Execute(cmd, grid);
        }

        // Output
        Console.WriteLine($"Final Position: ({rover.X}, {rover.Y}, {rover.Direction.Name[0]})");
        Console.WriteLine($"Status Report: {rover.Status(grid)}");
    }
}
