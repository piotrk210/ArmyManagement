using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Slot
{
    public class ArmyManager : MonoBehaviour
    {
        public const int SlotsNumber = 6;
        
        [SerializeField] private readonly List<Slot> armySlots = new List<Slot>();
        [SerializeField] private Slot slotPrefab;


        public void InitArmyManager()
        {
            CreateSlots();
        }
        
        private void CreateSlots()
        {
            for (int i = 0; i < SlotsNumber; i++)
            {
                var slot = Instantiate(slotPrefab, transform);
                armySlots.Add(slot);
            }
        }

        public void TryAddBigUnit(int index, bool shouldLookOnRight = true)
        {
            if(armySlots[index].UnitType == UnitType.None && !IsAtLeastSlotFree(2) ||
            armySlots[index].UnitType == UnitType.SmallWarrior && !IsAtLeastSlotFree(1) ||
            armySlots[index].UnitType == UnitType.BigWarriorLeft) return;
            
            SlideUnitsApart(index, shouldLookOnRight);
        }
        
        public void TryAddSmallUnit(int index)
        {
            if(SlotManager.Slots[index].UnitType == UnitType.BigWarriorLeft) SlotManager.Slots[index + 1].SetUnit(UnitType.None);
            if(SlotManager.Slots[index].UnitType == UnitType.BigWarriorRight) SlotManager.Slots[index - 1].SetUnit(UnitType.None);
            
            SlotManager.Slots[index].SetUnit(UnitType.SmallWarrior);
        }


        private void SlideUnitsApart(int index, bool shouldLookOnRight = true)
        {
            Debug.Log(shouldLookOnRight);
            int? freeSlotIndex;
            if (shouldLookOnRight)
            {
                freeSlotIndex = LookForFreeSlotOnRight(index);
            
                Debug.Log("free index " + freeSlotIndex);
                if (freeSlotIndex != null)
                {
                    MoveAllUnitsOnRight(index, (int)freeSlotIndex);
            
                    armySlots[index].SetUnit(UnitType.BigWarriorLeft);
                    armySlots[index+1].SetUnit(UnitType.BigWarriorRight); 
                    return;
                }
            }
            {
                freeSlotIndex = LookForFreeSlotOnLeft(index);

                Debug.Log("free index " + freeSlotIndex);
                
                if (freeSlotIndex != null)
                {
                    MoveAllUnitsOnLeft(index, (int)freeSlotIndex);
            
                    armySlots[index - 1].SetUnit(UnitType.BigWarriorLeft);
                    armySlots[index].SetUnit(UnitType.BigWarriorRight);
                    return;
                    //select na index -1 ?
                    //armySlots[index -1].OnSelect();
                } 
            }
            if (!shouldLookOnRight)
            {
                freeSlotIndex = LookForFreeSlotOnRight(index);
            
                Debug.Log("free index " + freeSlotIndex);
                if (freeSlotIndex != null)
                {
                    MoveAllUnitsOnRight(index, (int)freeSlotIndex);
            
                    armySlots[index].SetUnit(UnitType.BigWarriorLeft);
                    armySlots[index+1].SetUnit(UnitType.BigWarriorRight);  
                    return;
                }
            }
        }

        private int? LookForFreeSlotOnRight(int index)
        {
            for (int i = index + 1; i < armySlots.Count; i++)
            {
                Debug.Log(" szuka po prawo" + i);
                if (armySlots[i].UnitType == UnitType.None)
                {
                    return i;
                }
            }
            return null;
        }
        
        private int? LookForFreeSlotOnLeft(int index)
        {
            for (int i = index -1; i >= 0; i--)
            {
                Debug.Log(" szuka po lewo " + i);
                if (armySlots[i].UnitType == UnitType.None)
                {
                    return i;
                }
            }
            return null;
        }

        private void MoveAllUnitsOnRight(int index, int freeSlotIndex)
        {
            for (int j = freeSlotIndex - 1; j > index; j--)
            {
                Debug.Log("przesuwa jednostek na " + j);
                MoveUnitRight(j);
            }
        }
        
        private void MoveAllUnitsOnLeft(int index, int freeSlotIndex)
        {
            for (int j = freeSlotIndex + 1; j < index; j++)
            {
                Debug.Log("przesuwa jednostke z " + j);
                MoveUnitLeft(j);
            }
        }

        public bool IsAtLeastSlotFree(int freeSlotsNeeded)
        {
            int freeSlots = 0;
            foreach (var slot in armySlots)
            {
                if (slot.UnitType == UnitType.None) freeSlots++;
            }
            return freeSlots >= freeSlotsNeeded;
        }
        
        private void MoveUnitLeft(int index)
        {
            if (armySlots[index].UnitType == UnitType.SmallWarrior && index >= 1)
            {
                armySlots[index].SetUnit(UnitType.None);
                armySlots[index - 1].SetUnit(UnitType.SmallWarrior);
            } else if (armySlots[index].UnitType == UnitType.BigWarriorRight && index >= 2)
            {
                armySlots[index].SetUnit(UnitType.None);
                armySlots[index - 2].SetUnit(UnitType.BigWarriorLeft);
                armySlots[index - 1].SetUnit(UnitType.BigWarriorRight);
            }  
        }

        private void MoveUnitRight(int index)
        {
            if (armySlots[index].UnitType == UnitType.SmallWarrior && index < armySlots.Count - 1)
            {
                Debug.Log("przesuwa na " + (index + 1));
                armySlots[index].SetUnit(UnitType.None);
                armySlots[index + 1].SetUnit(UnitType.SmallWarrior);
            } else if (armySlots[index].UnitType == UnitType.BigWarriorLeft && index < armySlots.Count - 2)
            {
                armySlots[index].SetUnit(UnitType.None);
                armySlots[index + 1].SetUnit(UnitType.BigWarriorLeft);
                armySlots[index + 2].SetUnit(UnitType.BigWarriorRight);
            }            
        }
    }
}
