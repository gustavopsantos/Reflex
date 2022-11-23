using Reflex.Sample.Application;
using Reflex.Scripts.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Reflex.Sample.Infrastructure
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private Text _progressText;
        [SerializeField] private Button _resetButton;
        [Inject] private readonly ICollectionStorage _collectionStorage;

        private void Start()
        {
            _resetButton.onClick.AddListener(Reset);
        }

        private void Update()
        {
            _progressText.text = $"{_collectionStorage.Count()}/4";
        }

        private void Reset()
        {
            _collectionStorage.Clear();
            SceneManager.LoadScene("Reflex.Sample");
        }
    }
}