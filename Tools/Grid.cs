using System;
using System.Collections.Generic;
using System.Numerics;

namespace UntitledGame;

public struct IntVector2(int x, int y)
{
    public int X = x;
    public int Y = y;

    public static IntVector2 operator +(IntVector2 a, IntVector2 b) =>
        new IntVector2(b.X + a.X, b.Y + a.Y);
    

    public static IntVector2 operator -(IntVector2 a, IntVector2 b) =>
        new IntVector2(b.X - a.X, b.Y - a.Y);

    public static implicit operator String(IntVector2 vector) =>
        $"[{vector.X}, {vector.Y}]";
}

public sealed class Grid
{
    private readonly Entity?[,] _container;
    private readonly Dictionary<Entity, IntVector2> _subscribedEntities;

    public Vector2 CellSize { get; private set; }

    public Grid(IntVector2 gridSize)
    {
        _container = new Entity[gridSize.X, gridSize.Y];
        _subscribedEntities = new Dictionary<Entity, IntVector2>();

        CellSize = new Vector2(10, 10);
    }

    public void SubscribeEntity(Entity entity, IntVector2 cell)
    {
        try
        {
            _subscribedEntities.Add(entity, cell);
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
                for (int x = 0; x < _container.GetLength(1); x++)
                {
                    var currentEntity = _container[y, x];

                    if (currentEntity == entity)
                        _container[y, x] = null;
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
        
        var newPosition = _subscribedEntities[entity] + direction;

        if (newPosition.X >= _container.GetLength(0) || newPosition.Y >= _container.GetLength(1))
            throw new OverflowException($"A movement of {direction} would cause the entity to go out of bounds of the grid from {_subscribedEntities[entity]}.");

        _subscribedEntities[entity] = newPosition;
    }

    public Vector2 GetGlobalPositionFromGrid(Entity entity)
    {
        if (!_subscribedEntities.TryGetValue(entity, out var cell))
            throw new KeyNotFoundException("The entity given was not subscribed to the list.");

        var halfCellSize = CellSize * 0.5f;

        var globalCellTopLeftX = cell.X * CellSize.X;
        var globalCellTopLeftY = cell.Y * CellSize.Y;

        return new Vector2(globalCellTopLeftX, globalCellTopLeftY) + halfCellSize;
    }
}