namespace Gameplay.Environments
{
    public class MazeData
    {
        public int MazeWidth;
        public int MazeHeight;
        public int NumberOfExits;
        public float Complexity;
        public bool IsRandomSeed;
        public int Seed;

        public MazeData(int mazeWidth, int mazeHeight, int numberOfExits, float complexity, bool isRandomSeed, int seed)
        {
            MazeWidth = mazeWidth;
            MazeHeight = mazeHeight;
            NumberOfExits = numberOfExits;
            Complexity = complexity;
            IsRandomSeed = isRandomSeed;
            Seed = seed;
        }
    }
}