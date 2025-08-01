using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gameplay.Environments
{
    [CreateAssetMenu(fileName = "MazeConfig", menuName = "ScriptableObjects/Maze", order = 0)]
    public class MazeConfig : ScriptableObject
    {
        [SerializeField] private TileBase _groundTile;
        [SerializeField] private TileBase _wallTile;
        [SerializeField] private TileBase _exitTile;

        [SerializeField] private int _mazeWidth = 21;
        [SerializeField] private int _mazeHeight = 21;
        [SerializeField] private int _numberOfExits = 4;
        [SerializeField] private int _tileOffset = 10;

        [Tooltip("0 = simple, 1 = very complex"), Range(0f, 1f)] [SerializeField]
        private float _complexity = 0.75f;

        [SerializeField] private bool _isRandomSeed = true;
        [SerializeField] private int _seed = 77777;

        public TileBase GroundTile => _groundTile;
        public TileBase WallTile => _wallTile;
        public TileBase ExitTile => _exitTile;

        public int MazeWidth => _mazeWidth;
        public int MazeHeight => _mazeHeight;
        public int NumberOfExits => _numberOfExits;
        public int TileOffset => _tileOffset;
        public float Complexity => _complexity;
        public bool IsRandomSeed => _isRandomSeed;
        public int Seed => _seed;
    }
}