using Services.Storage;

namespace Gameplay.Environments
{
    public class MazeDataService
    {
        public MazeData MazeData { get; private set; }
        public MazeConfig MazeConfig { get; private set; }

        public MazeDataService(MazeConfig mazeConfig)
        {
            MazeConfig = mazeConfig;
            MazeData = new MazeData(mazeConfig.MazeWidth, mazeConfig.MazeHeight, mazeConfig.NumberOfExits,
                mazeConfig.Complexity, mazeConfig.IsRandomSeed, mazeConfig.Seed);
            LoadMazeData();
        }
        
        public void SaveMazeData()
        {
            StorageService.SaveData(StorageConstants.MAZE_DATA, MazeData);
        }

        private void LoadMazeData()
        {
            MazeData = StorageService.LoadData(StorageConstants.MAZE_DATA, MazeData);
        }
    }
}