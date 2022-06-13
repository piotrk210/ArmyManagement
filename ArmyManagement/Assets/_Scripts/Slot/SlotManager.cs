using System;
using System.Collections.Generic;
using Assets._Scripts.Data;
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

        #region ButtonsMethods

        private void DeleteUnit()
        {
            foreach (var slotIndex in selectedSlots)
            {
                if (slotIndex == null) continue;
                AddNoneUnitInProperlyArmy((int)slotIndex);
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
            bool ignoreTheNearest = false;


            if (firstUnitType != secondUnitType)
            {
                if (firstUnitType == UnitType.BigWarriorLeft && secondUnitIndex - firstUnitIndex is 2 or 3 &&
                    !IsUnitsInOtherArmy(firstUnitIndex,secondUnitIndex)||
                    secondUnitType == UnitType.BigWarriorLeft && firstUnitIndex - secondUnitIndex is 2 or 3 &&
                    !IsUnitsInOtherArmy(firstUnitIndex,secondUnitIndex) 
                )
                {
                    isStartLookingPlaceOnRight = false;
                }

                if (secondUnitType == UnitType.BigWarriorLeft && secondUnitIndex - firstUnitIndex == 1 &&
                    !IsUnitsInOtherArmy(firstUnitIndex, secondUnitIndex) ||
                    firstUnitType == UnitType.BigWarriorLeft && firstUnitIndex - secondUnitIndex == 1 &&
                    !IsUnitsInOtherArmy(firstUnitIndex, secondUnitIndex))
                {
                    ignoreTheNearest = true;
                }
                if (isStartLookingPlaceOnRight && IsUnitBigAndOnLastArmySlot(firstUnitIndex) && 
                    !IsUnitsInOtherArmy(firstUnitIndex,secondUnitIndex) && firstUnitIndex > secondUnitIndex)
                {
                    firstUnitIndex++;

                }
                if(isStartLookingPlaceOnRight && IsUnitBigAndOnLastArmySlot(secondUnitIndex) && 
                   !IsUnitsInOtherArmy(firstUnitIndex,secondUnitIndex) && secondUnitIndex > firstUnitIndex) 
                {
                    secondUnitIndex++;
                }
                if (secondUnitType == UnitType.BigWarriorLeft)
                {
                    AddSmallUnitInProperlyArmy(secondUnitIndex);
                        
                    AddBigUnitInProperlyArmy(firstUnitIndex, isStartLookingPlaceOnRight, ignoreTheNearest);
                }
                else
                {
                    AddSmallUnitInProperlyArmy(firstUnitIndex);
                        
                    AddBigUnitInProperlyArmy(secondUnitIndex, isStartLookingPlaceOnRight, ignoreTheNearest);
                }
            }
            DeselectAllSlots();
        }

        private void CreateSmallUnit()
        {
            foreach (var slotIndex in selectedSlots)
            {
                if (slotIndex == null) continue;
                AddSmallUnitInProperlyArmy((int)slotIndex);
            }
            DeselectAllSlots();
        }

        private void CreateBigUnit()
        {
            if (selectedSlots[1] == selectedSlots[0] - 1)
            {
                for (int i = 0; i < selectedSlots.Length; i++)
                {
                    if (selectedSlots[i] == null || Slots[(int) selectedSlots[i]].UnitType == UnitType.BigWarriorLeft) 
                        continue;
                    AddBigUnitInProperlyArmy((int)selectedSlots[i]);
                }
            }
            else
            {
                for (int i = selectedSlots.Length-1; i >= 0; i--)
                {
                    if (selectedSlots[i] == null) continue;
                    AddBigUnitInProperlyArmy((int)selectedSlots[i]);
                }
            }
            DeselectAllSlots();
        }

        #endregion

        
        private bool IsUnitsInOtherArmy(int firstIndex, int secondIndex)
        {
            if (firstIndex < ArmyManager.SlotsNumber && secondIndex < ArmyManager.SlotsNumber ||
                firstIndex >= ArmyManager.SlotsNumber && secondIndex >= ArmyManager.SlotsNumber) return false;
            return true;
        }

        private bool IsUnitBigAndOnLastArmySlot(int index)
        {
            if (Slots[index].UnitType == UnitType.BigWarriorLeft && index is ArmyManager.SlotsNumber - 2 or 
                ArmyManager.SlotsNumber - 3 or ArmyManager.SlotsNumber - 4 or ArmyManager.SlotsNumber * 2 - 2 
                or ArmyManager.SlotsNumber * 2 - 3 or ArmyManager.SlotsNumber * 2 - 4 ) return true;
            return false;
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
                if (selectedSlots[0] != null) Slots[(int)selectedSlots[0]].Deselect();
                selectedSlots[0] = selectedSlots[1];
                selectedSlots[1] = index;
            }

            switchButton.interactable = IsSwitchPossible();
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
                    Slots[(int)selectedSlots[i]].Deselect();
                }
            }
            switchButton.interactable = IsSwitchPossible();
        }
        
        #endregion

        private void AddBigUnitInProperlyArmy(int index, bool shouldLookForOnRight = true, bool ignoreTheNearest = false)
        {
            if (index < ArmyManager.SlotsNumber) topArmyManager.TryAddBigUnit(index, shouldLookForOnRight, ignoreTheNearest );
            else bottomArmyManager.TryAddBigUnit(index - ArmyManager.SlotsNumber, shouldLookForOnRight, ignoreTheNearest);
        }
        
        private void AddSmallUnitInProperlyArmy(int index)
        {
            if (index < ArmyManager.SlotsNumber)  topArmyManager.TryAddSmallUnit(index);
            else bottomArmyManager.TryAddSmallUnit(index - ArmyManager.SlotsNumber);
        }
        
        private void AddNoneUnitInProperlyArmy(int index)
        {
            if (index < ArmyManager.SlotsNumber)  topArmyManager.TryAddNoneUnit(index);
            else bottomArmyManager.TryAddNoneUnit(index - ArmyManager.SlotsNumber);
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
