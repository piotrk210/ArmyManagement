using System.Collections.Generic;
using Assets._Scripts.Data;
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
            
            SlideBigUnitApart(index, shouldLookOnRight);
        }
        
        public void TryAddSmallUnit(int index)
        {
            if(armySlots[index].UnitType == UnitType.BigWarriorLeft) armySlots[index + 1].SetUnit(UnitType.None);
            if(armySlots[index].UnitType == UnitType.BigWarriorRight) armySlots[index - 1].SetUnit(UnitType.None);
            
            armySlots[index].SetUnit(UnitType.SmallWarrior);
        }
        
        public void TryAddNoneUnit(int index)
        {
            if (armySlots[index].UnitType == UnitType.BigWarriorLeft)
            {
                armySlots[index + 1].SetUnit(UnitType.None);
            }
            armySlots[index].SetUnit(UnitType.None);
        }

        private void SlideBigUnitApart(int index, bool shouldFirstLookOnRight = true)
        {
            int? freeSlotIndex;
            if (shouldFirstLookOnRight)
            {
                freeSlotIndex = LookForFreeSlotOnRight(index);
                
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

                if (freeSlotIndex != null)
                {
                    MoveAllUnitsOnLeft(index, (int)freeSlotIndex);
            
                    armySlots[index - 1].SetUnit(UnitType.BigWarriorLeft);
                    armySlots[index].SetUnit(UnitType.BigWarriorRight);
                    return;
                } 
            }
            if (!shouldFirstLookOnRight)
            {
                freeSlotIndex = LookForFreeSlotOnRight(index);
                
                if (freeSlotIndex != null)
                {
                    MoveAllUnitsOnRight(index, (int)freeSlotIndex);
            
                    armySlots[index].SetUnit(UnitType.BigWarriorLeft);
                    armySlots[index+1].SetUnit(UnitType.BigWarriorRight);  
                }
            }
        }

        private int? LookForFreeSlotOnRight(int index)
        {
            for (int i = index + 1; i < armySlots.Count; i++)
            {
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
                MoveUnitRight(j);
            }
        }
        
        private void MoveAllUnitsOnLeft(int index, int freeSlotIndex)
        {
            for (int j = freeSlotIndex + 1; j < index; j++)
            {
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
