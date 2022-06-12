using System.Collections.Generic;
using Assets._Scripts.Data;
using UnityEngine;

namespace _Scriptable
{
    [CreateAssetMenu(fileName = "ImageData", menuName = "ScriptableObjects/ImageData", order = 1)]
    public class ImagesData : ScriptableObject
    {
        [SerializeField] private List<Texture> Sprites = new List<Texture>();

        public Texture GetSprite(UnitType unitType)
        {
            return Sprites[(int) unitType];
        }
    }
}
