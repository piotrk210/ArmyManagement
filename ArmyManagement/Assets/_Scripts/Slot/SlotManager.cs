using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Slot
{
    public class SlotManager : MonoBehaviour
    {
        private const int MAXNumberUnitsSelected = 2;
        
        [SerializeField] private ArmyManager topArmyManager;
        [SerializeField] private ArmyManager bottomArmyManager;
    
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button switchButton;
        [SerializeField] private Button creatSmallUnitButton;
        [SerializeField] private Button creatBigUnitButton;

        public static List<Slot> Slots = new List<Slot>();
        public static int?[] selectedSlots = new int?[MAXNumberUnitsSelected] {null, null};
        
        
        public static Action<int> OnSelectAction;
        public static Action<int> OnDeselectAction;

        public int currentSelectedSlotsNumber;

        private void Awake()
        {
            AssignButtonMethods();
            topArmyManager.InitArmyManager(Army.Top);
            bottomArmyManager.InitArmyManager(Army.Bottom);
            switchButton.interactable = IsSwitchPossible();
        }

        private void OnDestroy()
        {
            UnAssignButtonMethods();
        }
        
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
            Debug.Log("switch");
            int firstUnitIndex = (int)selectedSlots[0];
            int secondUnitIndex = (int)selectedSlots[1];
            UnitType firstUnitType = Slots[(int) selectedSlots[0]].UnitType;
            UnitType secondUnitType = Slots[(int) selectedSlots[1]].UnitType;

            if (firstUnitType != secondUnitType)
            {
                    if (secondUnitType == UnitType.BigWarriorLeft)
                    {
                        if (secondUnitIndex < topArmyManager.slotsNumber)  topArmyManager.TryAddSmallUnit(secondUnitIndex);
                        else bottomArmyManager.TryAddSmallUnit(secondUnitIndex);
                        
                        if (firstUnitIndex < topArmyManager.slotsNumber)  topArmyManager.TryAddBigUnit(firstUnitIndex);
                        else bottomArmyManager.TryAddBigUnit(firstUnitIndex- topArmyManager.slotsNumber);
                    }
                    else
                    {
                        if (firstUnitIndex < topArmyManager.slotsNumber)  topArmyManager.TryAddSmallUnit(firstUnitIndex);
                        else bottomArmyManager.TryAddSmallUnit(firstUnitIndex);
                        
                        if (secondUnitIndex < topArmyManager.slotsNumber)  topArmyManager.TryAddBigUnit(secondUnitIndex);
                        else bottomArmyManager.TryAddBigUnit(secondUnitIndex- topArmyManager.slotsNumber);
                    }
            }
            
            
            DeselectAllSlots();
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
                    Debug.Log(selectedSlots[i]);
                    if (selectedSlots[i] < topArmyManager.slotsNumber) topArmyManager.TryAddBigUnit((int) selectedSlots[i]);
                    else bottomArmyManager.TryAddBigUnit((int) selectedSlots[i] - topArmyManager.slotsNumber);
                }
            }
            else
            {
                for (int i = 0; i < selectedSlots.Length; i++)
                {
                    //Debug.Log(Slots[(int) selectedSlots[i]].UnitType == UnitType.BigWarriorRight);
                    if (selectedSlots[i] == null || Slots[(int) selectedSlots[i]].UnitType == UnitType.BigWarriorLeft) continue;
                    Debug.Log(selectedSlots[i]);
                    if (selectedSlots[i] < topArmyManager.slotsNumber) topArmyManager.TryAddBigUnit((int) selectedSlots[i]);
                    else bottomArmyManager.TryAddBigUnit((int) selectedSlots[i] - topArmyManager.slotsNumber);
                }    
            }
            DeselectAllSlots();
        }
        

        private void SelectMethod(int index)
        {
            //Debug.Log("select "+index);
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
            //Debug.Log("deselect "+index);
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
            else if (currentSelectedSlotsNumber == 2)
            {
                //Debug.Log("nic");
            }
            switchButton.interactable = IsSwitchPossible();
            //Debug.Log("Deselect First: "+ selectedSlots[0] + " Second: "+ selectedSlots[1]);
        }
        
        private static int SelectedSlotsNumber()
        {
            int selectedUnitsNumber = 0;
            foreach (var slot in Slots)
            {
                if (slot.isSelected && slot.UnitType != UnitType.BigWarriorRight) selectedUnitsNumber++;
            }
            
            return selectedUnitsNumber;
        }

        private void DeselectAllSlots()
        {
            // Debug.Log(selectedSlots[0] +" i " + selectedSlots[1]);
            for (int i = MAXNumberUnitsSelected - 1; i >= 0; i--)
            {
                if (selectedSlots[i] != null)
                {
                    Slots[(int)selectedSlots[i]].OnDeselect();
                }
            }
            switchButton.interactable = IsSwitchPossible();
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
                    if (slot < topArmyManager.slotsNumber)  return topArmyManager.IsAtLeastSlotFree(1);
                    else return bottomArmyManager.IsAtLeastSlotFree(1);
                }
            }

            Debug.Log("eloleoeoleo");
            return false;
        }
    }
}
