using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.Slot
{
    public class Slot : MonoBehaviour, IPointerClickHandler
    {
        public UnitType UnitType => unitType;
        public Army Army { get; set; }
        [SerializeField] private UnitType unitType = UnitType.None;
        [SerializeField] private Army army;
        [SerializeField] private RawImage borderImage;
        [SerializeField] private RawImage unitIconImage;
        [SerializeField] private ImagesData images;
        public int slotIndex;
        public bool isSelected;

        private readonly Color selectedColor = Color.red;
        private readonly Color unselectedColor = Color.white;
        
        private void Awake()
        {
            SlotManager.Slots.Add(this);
            slotIndex = SlotManager.Slots.IndexOf(this);
            borderImage.color = unselectedColor;
        }

        public void SetUnit(UnitType unitType)
        {
            if(this.unitType == unitType) return;
            
            ///////////////////////// CHECK IS THAT INTERACTION BLOCKER WORKS!!!!!!!!!
            if(unitType == UnitType.BigWarriorRight && isSelected) OnDeselect();

            this.unitType = unitType;
            unitIconImage.texture = images.GetSprite(unitType);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(unitType == UnitType.BigWarriorRight) return;
            if(!isSelected) OnSelect();
            else OnDeselect();
        }

        private void OnSelect()
        {
            borderImage.color = selectedColor;
            isSelected = true;
            SlotManager.OnSelectAction.Invoke(slotIndex);
        }

        public void OnDeselect()
        {
            borderImage.color = unselectedColor;
            isSelected = false;
            SlotManager.OnDeselectAction.Invoke(slotIndex);
        }
    }
}
