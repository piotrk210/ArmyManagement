using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.Slot
{
    public class Slot : MonoBehaviour, IPointerClickHandler
    {
        public UnitType UnitType => unitType;
        
        [SerializeField] private UnitType unitType = UnitType.None;

        [SerializeField] private RawImage borderImage;
        [SerializeField] private RawImage unitIconImage;
        [SerializeField] private ImagesData images;
        private int slotIndex;
        public bool IsSelected;

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
            if(unitType == UnitType.BigWarriorRight && IsSelected) OnDeselect();

            this.unitType = unitType;
            unitIconImage.texture = images.GetSprite(unitType);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(unitType == UnitType.BigWarriorRight) return;
            if(!IsSelected) OnSelect();
            else OnDeselect();
        }

        private void OnSelect()
        {
            borderImage.color = selectedColor;
            IsSelected = true;
            SlotManager.OnSelectAction.Invoke(slotIndex);
        }

        public void OnDeselect()
        {
            borderImage.color = unselectedColor;
            IsSelected = false;
            SlotManager.OnDeselectAction.Invoke(slotIndex);
        }
    }
}
