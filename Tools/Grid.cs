using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Foster.Framework;

namespace UntitledGame;

public struct IntVector2(int x, int y)
{
    public int X = x;
    public int Y = y;

    public static IntVector2 Up => new(0, -1);
    public static IntVector2 Down => new(0, 1);
    public static IntVector2 Left => new(-1,0);
    public static IntVector2 Right => new(1,0);

    public static IntVector2 operator +(IntVector2 a, IntVector2 b) =>
        new IntVector2(b.X + a.X, b.Y + a.Y);
    

    public static IntVector2 operator -(IntVector2 a, IntVector2 b) =>
        new IntVector2(b.X - a.X, b.Y - a.Y);

    public static implicit operator String(IntVector2 vector) =>
        $"[{vector.X}, {vector.Y}]";
}

public sealed class Grid
{
    public class CellOccupiedException : Exception
    {
        public CellOccupiedException() { }
        public CellOccupiedException(string message) : base(message) { }
    }

    private readonly Entity?[,] _container;
    private readonly Dictionary<Entity, IntVector2> _subscribedEntities;

    public Vector2 CellSize { get; private set; }

    public Grid(IntVector2 gridSize, Window gameWindow)
    {
        _container = new Entity[gridSize.X, gridSize.Y];
        _subscribedEntities = new Dictionary<Entity, IntVector2>();

        CellSize = new Vector2(
            gameWindow.WidthInPixels / gridSize.X,
            gameWindow.HeightInPixels / gridSize.Y
        );
    }

    public bool IsCellInGrid(IntVector2 cell)
    {
        return (
            cell.X >= 0 &&
            cell.Y >= 0 &&
            cell.X < _container.GetLength(0) &&
            cell.Y < _container.GetLength(1)
        );
    }

    public void SubscribeEntity(Entity entity, IntVector2 cell)
    {
        try
        {
            _subscribedEntities.Add(entity, cell);
            
            if (CellOccupied(cell))
                throw new CellOccupiedException("Cannot subscribe an entity to a cell that is already occupied.");

            _container[cell.X, cell.Y] = entity;
        }
        catch (ArgumentException err)
        {
            Console.Error.WriteLine("An entity was being subscribed to a grid when an entity with the given position already exists:");
            Console.Error.WriteLine(err);
        }
        catch (Exception err)
        {
            Console.Error.WriteLine("An unknown error occured while trying to subscribe an entity to the grid:");
            Console.Error.WriteLine(err);
        }
    }

    public void UnsubscribeEntity(Entity entity)
    {
        try
        {
            _subscribedEntities.Remove(entity);
            
            for (int y = 0; y < _container.GetLength(0); y++)
            {
                for (int x = 0; x < _container.GetLength(1); x++)
                {
                    if (_container[y,x] == entity)
                        _container[y,x] = null;
                }
            }
        }
        catch (Exception err)
        {
            Console.Error.WriteLine("An unknown exception occured while trying to unsubscribe an entity from the grid:");
            Console.Error.WriteLine(err);
        }
    }

    public void MoveEntity(Entity entity, IntVector2 direction)
    {
        if (!_subscribedEntities.ContainsKey(entity))
            throw new KeyNotFoundException($"Entity given to move is not contained in the grid.");
        
        var oldPosition = _subscribedEntities[entity];
        var newPosition = oldPosition + direction;

        if (newPosition.X >= _container.GetLength(0) || newPosition.Y >= _container.GetLength(1))
            throw new ArgumentException($"A movement of {direction} would cause the entity to go out of bounds of the grid from {_subscribedEntities[entity]}.");

        if (_container[newPosition.X, newPosition.Y] != null)
            throw new CellOccupiedException("An entity was moved into a cell that is already occupied.");

        _container[oldPosition.X, oldPosition.Y] = null;
        _subscribedEntities[entity] = newPosition;
        _container[newPosition.X, newPosition.Y] = entity;
    }

    public Vector2 GetGlobalPositionFromGrid(Entity entity)
    {
        if (!_subscribedEntities.TryGetValue(entity, out var cell))
            throw new KeyNotFoundException("The entity given was not subscribed to the list.");

        return GetGlobalPositionFromGrid(cell);
    }

    public Vector2 GetGlobalPositionFromGrid(IntVector2 cell)
    {
        var halfCellSize = CellSize * 0.5f;

        var globalCellTopLeftX = cell.X * CellSize.X;
        var globalCellTopLeftY = cell.Y * CellSize.Y;

        return new Vector2(globalCellTopLeftX + halfCellSize.X, globalCellTopLeftY + halfCellSize.Y);
    }

    public IntVector2 GetGridPosition(Entity entity)
    {
        if (!_subscribedEntities.TryGetValue(entity, out var cell))
            throw new KeyNotFoundException("The entity given was not subscribed to the list.");

        return cell;
    }

    public bool CellOccupied(IntVector2 cell)
    {
        return _container[cell.X, cell.Y] != null;
    }
}