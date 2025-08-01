using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Character;
using UI.Variables;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Gameplay.Environments
{
    [Serializable]
    public sealed class MazeGenerator
    {
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private Tilemap _wallTilemap;
        [SerializeField] private Tilemap _exitTilemap;

        private MazeData _mazeData;
        private MazeConfig _mazeConfig;

        private CellType[,] _maze;
        private System.Random _random;
        private CharacterControl _characterControl;
        private int _mazeWidth;
        private int _mazeHeight;
        private int _totalWidth;
        private int _totalHeight;
        
        private int _pathOffset;

        public void Initialize(MazeDataService mazeDataService, CharacterControl characterControl)
        {
            _mazeConfig = mazeDataService.MazeConfig;
            _mazeData = mazeDataService.MazeData;
            _characterControl = characterControl;

            _mazeWidth = _mazeData.MazeWidth;
            _mazeHeight = _mazeData.MazeHeight;
            _pathOffset = _mazeConfig.TileOffset;
        }

        public void GenerateMaze()
        {
            _mazeWidth = _mazeData.MazeWidth;
            _mazeHeight = _mazeData.MazeHeight;
            _pathOffset = _mazeConfig.TileOffset;
            
            _random = _mazeData.IsRandomSeed
                ? new System.Random(Random.Range(0, int.MaxValue))
                : new System.Random(_mazeData.Seed);

            if (_mazeData.MazeWidth % 2 == 0)
            {
                _mazeWidth++;
            }

            if (_mazeHeight % 2 == 0)
            {
                _mazeHeight++;
            }

            _totalWidth = _mazeWidth + (_pathOffset * 2);
            _totalHeight = _mazeHeight + (_pathOffset * 2);

            CreateMaze();
            AddExits();
            BuildTilemaps();
            SpawnPlayer();
        }

        private void CreateMaze()
        {
            _maze = new CellType[_totalWidth, _totalHeight];
            
            for (var x = 0; x < _totalWidth; x++)
            {
                for (var y = 0; y < _totalHeight; y++)
                {
                    _maze[x, y] = CellType.Wall;
                }
            }

            for (var x = 0; x < _totalWidth; x++)
            {
                for (var y = 0; y < _totalHeight; y++)
                {
                    if (y < _pathOffset || y >= _totalHeight - _pathOffset)
                    {
                        _maze[x, y] = CellType.Path;
                    }
                    else if (x < _pathOffset || x >= _totalWidth - _pathOffset)
                    {
                        _maze[x, y] = CellType.Path;
                    }
                }
            }

            var stack = new List<Vector2Int>();
            var current = new Vector2Int(_pathOffset + 1, _pathOffset + 1);
            _maze[current.x, current.y] = CellType.Path;

            while (true)
            {
                var neighbors = GetUnvisitedNeighbors(current);

                if (neighbors.Count > 0)
                {
                    var next = _random.NextDouble() < _mazeData.Complexity
                        ? neighbors[_random.Next(neighbors.Count)]
                        : neighbors.OrderBy(n => Vector2Int.Distance(n, current)).First();

                    var wall = current + (next - current) / 2;
                    _maze[wall.x, wall.y] = CellType.Path;
                    _maze[next.x, next.y] = CellType.Path;

                    stack.Add(current);
                    current = next;
                }
                else if (stack.Count > 0)
                {
                    current = stack[^1];
                    stack.RemoveAt(stack.Count - 1);
                }
                else
                {
                    break;
                }
            }
        }

        private List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
        {
            var neighbors = new List<Vector2Int>();
            Vector2Int[] directions =
                { Vector2Int.up * 2, Vector2Int.right * 2, Vector2Int.down * 2, Vector2Int.left * 2 };

            foreach (var dir in directions)
            {
                var neighbor = cell + dir;
                if (IsInMazeBounds(neighbor) && _maze[neighbor.x, neighbor.y] == CellType.Wall)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private bool IsInMazeBounds(Vector2Int pos)
        {
            // Check bounds within the maze area (excluding the path border)
            return pos.x > _pathOffset && pos.x < _totalWidth - _pathOffset - 1 && 
                   pos.y > _pathOffset && pos.y < _totalHeight - _pathOffset - 1;
        }

        private void AddExits()
        {
            var possibleExits = new List<Vector2Int>();

            // Check exits on the maze boundaries (not the outer path border)
            for (var x = _pathOffset; x < _totalWidth - _pathOffset; x++)
            {
                if (x > _pathOffset && x < _totalWidth - _pathOffset - 1)
                {
                    if (_maze[x, _pathOffset + 1] == CellType.Path) 
                        possibleExits.Add(new Vector2Int(x, _pathOffset));
                    if (_maze[x, _totalHeight - _pathOffset - 2] == CellType.Path)
                        possibleExits.Add(new Vector2Int(x, _totalHeight - _pathOffset - 1));
                }
            }

            for (var y = _pathOffset; y < _totalHeight - _pathOffset; y++)
            {
                if (y > _pathOffset && y < _totalHeight - _pathOffset - 1)
                {
                    if (_maze[_pathOffset + 1, y] == CellType.Path) 
                        possibleExits.Add(new Vector2Int(_pathOffset, y));
                    if (_maze[_totalWidth - _pathOffset - 2, y] == CellType.Path) 
                        possibleExits.Add(new Vector2Int(_totalWidth - _pathOffset - 1, y));
                }
            }

            var exitsToPlace = Mathf.Min(_mazeData.NumberOfExits, possibleExits.Count);
            for (var i = 0; i < exitsToPlace; i++)
            {
                var index = _random.Next(possibleExits.Count);
                var exitPos = possibleExits[index];
                _maze[exitPos.x, exitPos.y] = CellType.Exit;
                possibleExits.RemoveAt(index);
            }
        }

        private void BuildTilemaps()
        {
            ClearTilemap(_groundTilemap);
            ClearTilemap(_wallTilemap);
            ClearTilemap(_exitTilemap);
            
            for (var x = 0; x < _totalWidth; x++)
            {
                for (var y = 0; y < _totalHeight; y++)
                {
                    var pos = new Vector3Int(x, y, 0);

                    switch (_maze[x, y])
                    {
                        case CellType.Wall:
                            if (_wallTilemap != null && _mazeConfig.WallTile != null)
                                _wallTilemap.SetTile(pos, _mazeConfig.WallTile);
                            break;

                        case CellType.Path:
                            if (_groundTilemap != null && _mazeConfig.GroundTile != null)
                            {
                                _groundTilemap.SetTile(pos, _mazeConfig.GroundTile);
                            }
                            break;

                        case CellType.Exit:
                            if (_exitTilemap != null && _mazeConfig.ExitTile != null)
                                _exitTilemap.SetTile(pos, _mazeConfig.ExitTile);
                            if (_groundTilemap != null && _mazeConfig.GroundTile != null)
                            {
                                _groundTilemap.SetTile(pos, _mazeConfig.GroundTile);
                            }
                            break;
                    }
                }
            }
        }

        private void ClearTilemap(Tilemap tilemap)
        {
            if (tilemap == null) return; // Fixed: was checking != null

            var bounds = tilemap.cellBounds;
            var total = bounds.size.x * bounds.size.y * bounds.size.z;
            tilemap.SetTilesBlock(bounds, new TileBase[total]);
        }

        private void SpawnPlayer()
        {
            if (_characterControl == null)
            {
                return;
            }

            var center = new Vector2Int(_totalWidth / 2, _totalHeight / 2);

            if (_maze[center.x, center.y] != CellType.Path)
            {
                var minDistance = float.MaxValue;
                var nearestPath = center;

                for (var x = _pathOffset + 1; x < _totalWidth - _pathOffset - 1; x++)
                {
                    for (var y = _pathOffset + 1; y < _totalHeight - _pathOffset - 1; y++)
                    {
                        if (_maze[x, y] == CellType.Path)
                        {
                            var distance = Vector2Int.Distance(new Vector2Int(x, y), center);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                nearestPath = new Vector2Int(x, y);
                            }
                        }
                    }
                }

                center = nearestPath;
            }

            var worldPosition = _groundTilemap.CellToWorld(new Vector3Int(center.x, center.y, 0));
            worldPosition += _groundTilemap.tileAnchor;
            _characterControl.transform.position = worldPosition;
            _characterControl.Initialize(_maze, _totalWidth, _totalHeight, _groundTilemap);
        }
    }
}