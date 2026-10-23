#region 

using UnityEngine;

#endregion

namespace MergeGridGame.Factories
{
    public class UIFactory
    {
        private readonly Transform _uiRootContainer;

        public UIFactory(Transform uiRootContainer)
        {
            _uiRootContainer = uiRootContainer;
        }

        public GameObject CreateUIElement(GameObject prefab)
        {
            return Object.Instantiate(prefab, _uiRootContainer, false);
        }

        public T CreateUIElement<T>(T prefab) where T : Object
        {
            return Object.Instantiate(prefab, _uiRootContainer, false);
        }
    }
}
