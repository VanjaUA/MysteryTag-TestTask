#region
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace MergeGridGame.UI
{
    public class VictoryScreenView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform windowContainer;
        [SerializeField] private TMP_Text currentTimeText;
        [SerializeField] private TMP_Text bestTimeText;
        [SerializeField] private Button nextLevelButton;

        public event Action OnNextLevelClicked;

        private void Awake()
        {
            nextLevelButton.onClick.AddListener(HandleNextLevelClicked);
        }

        private void OnDestroy()
        {
            nextLevelButton.onClick.RemoveListener(HandleNextLevelClicked);
            if (windowContainer != null)
            {
                _ = windowContainer.DOKill();
            }
        }

        public void Setup(string currentTimeFormatted, string bestTimeFormatted)
        {
            currentTimeText.text = $"Time: {currentTimeFormatted}";
            bestTimeText.text = $"Best: {bestTimeFormatted}";

            PlayShowAnimation();
        }

        private void PlayShowAnimation()
        {
            if (windowContainer == null)
            {
                return;
            }

            _ = windowContainer.DOKill();
            windowContainer.localScale = Vector3.zero;
            _ = windowContainer.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        private void HandleNextLevelClicked()
        {
            OnNextLevelClicked?.Invoke();
        }
    }
}