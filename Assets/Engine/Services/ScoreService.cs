#region

using System;

#endregion

namespace MergeGridGame.Services
{
    public class ScoreService : IScoreService
    {
        public const int ScoreToWin = 40;
        private const string TotalScoreKey = "TotalScore";

        private readonly ISaveService _saveService;

        public event Action<int, int> OnScoreChanged;
        public event Action OnLevelCompleted;

        public int CurrentRoundScore { get; private set; }
        public int TotalScore { get; private set; }

        public ScoreService(ISaveService saveService)
        {
            _saveService = saveService;
            TotalScore = _saveService.LoadInt(TotalScoreKey, 0);
            CurrentRoundScore = 0;
        }

        public void AddScore(int amount)
        {
            if (amount <= 0) return;

            CurrentRoundScore += amount;
            TotalScore += amount;

            _saveService.SaveInt(TotalScoreKey, TotalScore);

            OnScoreChanged?.Invoke(CurrentRoundScore, TotalScore);
            if (CurrentRoundScore >= ScoreToWin)
            {
                _saveService.SaveAll();
                OnLevelCompleted?.Invoke();
            }
        }

        public void ResetRoundScore()
        {
            CurrentRoundScore = 0;
            OnScoreChanged?.Invoke(CurrentRoundScore, TotalScore);
        }
    }
}
