using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameResources", menuName = "Config/GameResources")]
public class ResourceCollection : ConfigBase
{
    [SerializeField] private List<GameResourcesElement> _gameResourceCollection = new List<GameResourcesElement>();

    public List<GameResourcesElement> GetCollection() => _gameResourceCollection;

    public GameResourcesElement GetElementByName(string name)
    {
        for (int i = 0; i < _gameResourceCollection.Count; i++)
        {
            if (_gameResourceCollection[i].ResourceName == name)
                return _gameResourceCollection[i];
        }

        Debug.LogWarning("GameResource element not found");
        return null;
    }
}

[System.Serializable]
public class GameResourcesElement
{
    public Sprite ResourceIcon;
    public string ResourceName;
}