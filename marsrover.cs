using System;
using System.Collections.Generic;

// Composite Pattern Base ------------------
abstract class GridComponent
{
    public abstract bool IsOccupied(int x, int y);
    public abstract void Display();
}

// Leaf --------------------------
class Obstacle : GridComponent
{
    private readonly int x, y;
    public Obstacle(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public override bool IsOccupied(int px, int py) => (px == x && py == y);
    public override void Display()
    {
        Console.WriteLine($"Obstacle at ({x}, {y})");
    }
}

// Composite ------------------------
class Grid : GridComponent
{
    private readonly int width, height;
    private readonly List<GridComponent> components = new List<GridComponent>();
    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void Add(GridComponent component) => components.Add(component);
    public override bool IsOccupied(int x, int y)
    {
        foreach (var c in components)
        {
            if (c.IsOccupied(x, y)) return true;
        }
        return false;
    }

    public bool IsInside(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;
    public bool IsValid(int x, int y) => IsInside(x, y) && !IsOccupied(x, y);
    public override void Display()
    {
        Console.WriteLine($"Grid: {width} x {height}");
        foreach (var c in components) c.Display();
    }
}

// Directions -----------------
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

// Command Pattern ------------------------
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

// Rover ----------------------
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
        string obstacleMsg = grid.IsOccupied(X, Y) ? "Obstacle detected." : "No Obstacles detected.";
        return $"Rover is at ({X}, {Y}) facing {Direction.Name}. {obstacleMsg}";
    }
}

// Program -----------------------
class Program
{
    static void Main()
    {
        Console.Write("Enter grid width: ");
        int width = int.Parse(Console.ReadLine());
        Console.Write("Enter grid height: ");
        int height = int.Parse(Console.ReadLine());

        var grid = new Grid(width, height);

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

        Console.Write("Enter number of obstacles: ");
        int obsCount = int.Parse(Console.ReadLine());
        for (int i = 0; i < obsCount; i++)
        {
            Console.Write($"Enter obstacle {i + 1} (x y): ");
            var parts = Console.ReadLine().Split();
            int ox = int.Parse(parts[0]);
            int oy = int.Parse(parts[1]);
            grid.Add(new Obstacle(ox, oy));
        }

        var rover = new Rover(startX, startY, startDir);

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

        foreach (var cmd in commands)
        {
            rover.Execute(cmd, grid);
        }

        Console.WriteLine($"Final Position: ({rover.X}, {rover.Y}, {rover.Direction.Name[0]})");
        Console.WriteLine($"Status Report: {rover.Status(grid)}");
    }
}
