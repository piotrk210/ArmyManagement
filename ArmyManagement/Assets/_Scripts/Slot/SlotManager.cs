using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Slot
{
    public class SlotManager : MonoBehaviour
    {
        [SerializeField] private ArmyManager topArmyManager;
        [SerializeField] private ArmyManager bottomArmyManager;
    
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button switchButton;
        [SerializeField] private Button creatSmallUnitButton;
        [SerializeField] private Button creatBigUnitButton;

        public static List<Slot> Slots = new List<Slot>();
        public static int?[] selectedSlots = new int?[] {null, null};
        
        
        public static Action<int> OnSelectAction;
        public static Action<int> OnDeselectAction;

        private int currentSelectedSlotsNumber;

        #region UnityEventMehtods

        private void Awake()
        {
            AssignButtonMethods();
            topArmyManager.InitArmyManager();
            bottomArmyManager.InitArmyManager();
            switchButton.interactable = IsSwitchPossible();
        }

        private void OnDestroy()
        {
            UnAssignButtonMethods();
        }        

        #endregion

        
        private void AssignButtonMethods()
        {
            OnSelectAction += SelectMethod;
            OnDeselectAction += DeselectMethod;
            deleteButton.onClick.AddListener(DeleteUnit);
            switchButton.onClick.AddListener(SwitchUnits);
            creatSmallUnitButton.onClick.AddListener(CreateSmallUnit);
            creatBigUnitButton.onClick.AddListener(CreateBigUnit);

        }
    
        private void UnAssignButtonMethods()
        {
            OnSelectAction -= SelectMethod;
            OnDeselectAction -= DeselectMethod;
            deleteButton.onClick.RemoveAllListeners();
            switchButton.onClick.RemoveAllListeners();
            creatSmallUnitButton.onClick.RemoveAllListeners();
            creatBigUnitButton.onClick.RemoveAllListeners();
        }

        private void DeleteUnit()
        {
            foreach (var slotIndex in selectedSlots)
            {
                if (slotIndex == null) continue;
                if (Slots[(int) slotIndex].UnitType == UnitType.BigWarriorLeft)
                {
                    Slots[(int) slotIndex + 1].SetUnit(UnitType.None);
                }
                Slots[(int) slotIndex].SetUnit(UnitType.None);
            }
            DeselectAllSlots();
        }

        private void SwitchUnits()
        {
            int firstUnitIndex = (int)selectedSlots[0];
            int secondUnitIndex = (int)selectedSlots[1];
            UnitType firstUnitType = Slots[(int) selectedSlots[0]].UnitType;
            UnitType secondUnitType = Slots[(int) selectedSlots[1]].UnitType;
            bool isStartLookingPlaceOnRight = true;

            if (firstUnitType != secondUnitType)
            {
                if (firstUnitType == UnitType.BigWarriorLeft && secondUnitIndex - firstUnitIndex == 2 &&
                    !IsUnitsInOtherArmy(firstUnitIndex,secondUnitIndex)||
                    secondUnitType == UnitType.BigWarriorLeft && firstUnitIndex - secondUnitIndex == 2 &&
                    !IsUnitsInOtherArmy(firstUnitIndex,secondUnitIndex))
                {
                    Debug.Log("wyjątek spamuje dużą jednostkę na lewo");
                    isStartLookingPlaceOnRight = false;
                }
                if (IsUnitBigAndOnLastArmySlot(firstUnitIndex))
                {
                    firstUnitIndex++;
                    Debug.Log("wyjątek mała jednostak na prawą częśc duzej");
                }
                if(IsUnitBigAndOnLastArmySlot(secondUnitIndex)) 
                {
                    secondUnitIndex++;
                    Debug.Log("wyjątek mała jednostak na prawą częśc duzej");
                }
                if (secondUnitType == UnitType.BigWarriorLeft)
                {
                    if (secondUnitIndex < ArmyManager.SlotsNumber)  topArmyManager.TryAddSmallUnit(secondUnitIndex);
                    else bottomArmyManager.TryAddSmallUnit(secondUnitIndex);
                        
                    if (firstUnitIndex < ArmyManager.SlotsNumber)  topArmyManager.TryAddBigUnit(firstUnitIndex, isStartLookingPlaceOnRight);
                    else bottomArmyManager.TryAddBigUnit(firstUnitIndex- ArmyManager.SlotsNumber, isStartLookingPlaceOnRight);
                }
                else
                {
                    if (firstUnitIndex < ArmyManager.SlotsNumber)  topArmyManager.TryAddSmallUnit(firstUnitIndex);
                    else bottomArmyManager.TryAddSmallUnit(firstUnitIndex);
                        
                    if (secondUnitIndex < ArmyManager.SlotsNumber)  topArmyManager.TryAddBigUnit(secondUnitIndex, isStartLookingPlaceOnRight);
                    else bottomArmyManager.TryAddBigUnit(secondUnitIndex- ArmyManager.SlotsNumber, isStartLookingPlaceOnRight);
                }
            }
            
            
            DeselectAllSlots();
        }

        
        private bool IsUnitsInOtherArmy(int firstIndex, int secondIndex)
        {
            if (firstIndex < ArmyManager.SlotsNumber && secondIndex < ArmyManager.SlotsNumber ||
                firstIndex >= ArmyManager.SlotsNumber && secondIndex >= ArmyManager.SlotsNumber) return false;
            return true;
        }

        private bool IsUnitBigAndOnLastArmySlot(int index)
        {
            if (Slots[index].UnitType == UnitType.BigWarriorLeft && index == ArmyManager.SlotsNumber - 2 ||
                Slots[index].UnitType == UnitType.BigWarriorLeft && index == Slots.Count - 2) return true;
            return false;
        }

        private void CreateSmallUnit()
        {
            foreach (var slotIndex in selectedSlots)
            {
                if (slotIndex == null) continue;
                if(Slots[(int) slotIndex].UnitType == UnitType.BigWarriorLeft) Slots[(int) slotIndex + 1].SetUnit(UnitType.None);
                Slots[(int) slotIndex].SetUnit(UnitType.SmallWarrior);
            }
            DeselectAllSlots();
        }

        private void CreateBigUnit()
        {
            if (selectedSlots[0] == selectedSlots[1] - 1)
            {
                Debug.Log("sloty obok siebie w złej kolejności");
                for (int i = selectedSlots.Length-1; i >= 0; i--)
                {
                    if (selectedSlots[i] == null) continue;
                    if (selectedSlots[i] < ArmyManager.SlotsNumber) topArmyManager.TryAddBigUnit((int) selectedSlots[i]);
                    else bottomArmyManager.TryAddBigUnit((int) selectedSlots[i] - ArmyManager.SlotsNumber);
                }
            }
            else
            {
                for (int i = 0; i < selectedSlots.Length; i++)
                {
                    if (selectedSlots[i] == null || Slots[(int) selectedSlots[i]].UnitType == UnitType.BigWarriorLeft) continue;
                    if (selectedSlots[i] < ArmyManager.SlotsNumber) topArmyManager.TryAddBigUnit((int) selectedSlots[i]);
                    else bottomArmyManager.TryAddBigUnit((int) selectedSlots[i] - ArmyManager.SlotsNumber);
                }    
            }
            DeselectAllSlots();
        }
        

        #region SelectMethods

        private void SelectMethod(int index)
        {
            currentSelectedSlotsNumber = SelectedSlotsNumber();
            if (currentSelectedSlotsNumber == 1)
            {
                selectedSlots[0] = index;
            }
            else if (currentSelectedSlotsNumber == 2)
            {
                selectedSlots[1] = index;
            }
            else
            {
                if (selectedSlots[0] != null) Slots[(int)selectedSlots[0]].OnDeselect();
                selectedSlots[0] = selectedSlots[1];
                selectedSlots[1] = index;
            }

            switchButton.interactable = IsSwitchPossible();
            //Debug.Log("Select First: "+ selectedSlots[0] + " Second: "+ selectedSlots[1]);
        }
        
        private void DeselectMethod(int index)
        {
            currentSelectedSlotsNumber = SelectedSlotsNumber();
            if (currentSelectedSlotsNumber == 0)
            {
                selectedSlots[0] = null;
            }
            else if (currentSelectedSlotsNumber == 1)
            {
                if(index != selectedSlots[1]) selectedSlots[0] = selectedSlots[1];
                selectedSlots[1] = null;
            }
            switchButton.interactable = IsSwitchPossible();
            //Debug.Log("Deselect First: "+ selectedSlots[0] + " Second: "+ selectedSlots[1]);
        }
        
        private static int SelectedSlotsNumber()
        {
            int selectedUnitsNumber = 0;
            foreach (var slot in Slots)
            {
                if (slot.IsSelected && slot.UnitType != UnitType.BigWarriorRight) selectedUnitsNumber++;
            }
            
            return selectedUnitsNumber;
        }

        private void DeselectAllSlots()
        {
            for (int i = selectedSlots.Length - 1; i >= 0; i--)
            {
                if (selectedSlots[i] != null)
                {
                    Slots[(int)selectedSlots[i]].OnDeselect();
                }
            }
            switchButton.interactable = IsSwitchPossible();
        }
        
        #endregion

        private void AddBigUnitInProperlyArmy(int index)
        {
            if (index < ArmyManager.SlotsNumber) topArmyManager.TryAddBigUnit(index);
            else bottomArmyManager.TryAddBigUnit(index - ArmyManager.SlotsNumber);
        }
        
        private bool IsSwitchPossible()
        {
            if (selectedSlots[0] == null || selectedSlots[1] == null) return false;
            if (Slots[(int) selectedSlots[0]].UnitType == UnitType.None ||
                Slots[(int) selectedSlots[1]].UnitType == UnitType.None) return false;
            if (selectedSlots[0] < 6 && selectedSlots[1] < 6 ||
                selectedSlots[0] >= 6 && selectedSlots[1] >= 6) return true;
            if (Slots[(int) selectedSlots[0]].UnitType == Slots[(int) selectedSlots[1]].UnitType) return true;
            foreach (var slot in selectedSlots)
            {
                if (Slots[(int) slot].UnitType == UnitType.SmallWarrior)
                {
                    if (slot < ArmyManager.SlotsNumber)  return topArmyManager.IsAtLeastSlotFree(1);
                    else return bottomArmyManager.IsAtLeastSlotFree(1);
                }
            }
            return false;
        }
    }
}
