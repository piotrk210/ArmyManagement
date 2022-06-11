using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "ImageData", menuName = "ScriptableObjects/ImageData", order = 1)]
public class ImagesData : ScriptableObject
{
    [SerializeField] private List<Texture> Sprites = new List<Texture>();

    public Texture GetSprite(UnitType unitType)
    {
        return Sprites[(int) unitType];
    }
}
