using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Slot
{
    public class ArmyManager : MonoBehaviour
    {
        [SerializeField] private List<Slot> armySlots = new List<Slot>();
        [SerializeField] private Slot slotPrefab;
        [SerializeField] public int slotsNumber = 6;

        public Army army;

        public void InitArmyManager(Army army)
        {
            this.army = army;
            CreateSlots();
        }
        
        private void CreateSlots()
        {
            for (int i = 0; i < slotsNumber; i++)
            {
                var slot = Instantiate(slotPrefab, transform);
                slot.Army = army;
                armySlots.Add(slot);
            }
        }

        public void TryAddBigUnit(int index)
        {
            Debug.Log(index);
            Debug.Log(armySlots[index].UnitType + " " + IsAtLeastSlotFree(2));
            if(armySlots[index].UnitType == UnitType.None && !IsAtLeastSlotFree(2)) return;
            if(armySlots[index].UnitType == UnitType.SmallWarrior && !IsAtLeastSlotFree(1)) return;
            if(armySlots[index].UnitType == UnitType.BigWarriorLeft) return;
            
            SlideUnitsApart(index);
            //jeśli na index+1 jest small to go przesuwamy/zostawiamy chyba że będzie na tak dopiero po powyższym warunku
        }
        
        public void TryAddSmallUnit(int index)
        {
            // if(armySlots[index].UnitType == UnitType.None && !IsAtLeastSlotFree(2)) return;
            // if(armySlots[index].UnitType == UnitType.SmallWarrior && !IsAtLeastSlotFree(1)) return;
            
            if(SlotManager.Slots[index].UnitType == UnitType.BigWarriorLeft) SlotManager.Slots[index + 1].SetUnit(UnitType.None);
            SlotManager.Slots[index].SetUnit(UnitType.SmallWarrior);
            //jeśli na index+1 jest small to go przesuwamy/zostawiamy chyba że będzie na tak dopiero po powyższym warunku
        }


        private void SlideUnitsApart(int index, bool shouldLookOnRight = true)
        {
            int? freeSlotIndex = LookForFreeSlotOnRight(index);
            
            Debug.Log("free index " + freeSlotIndex);
            if (freeSlotIndex != null)
            {
                MoveAllUnitsOnRight(index, (int)freeSlotIndex);
            
                armySlots[index].SetUnit(UnitType.BigWarriorLeft);
                armySlots[index+1].SetUnit(UnitType.BigWarriorRight);    
            }
            else
            {
                freeSlotIndex = LookForFreeSlotOnLeft(index);

                Debug.Log("free index " + freeSlotIndex);
                
                if (freeSlotIndex != null)
                {
                    MoveAllUnitsOnLeft(index, (int)freeSlotIndex);
            
                    armySlots[index - 1].SetUnit(UnitType.BigWarriorLeft);
                    armySlots[index].SetUnit(UnitType.BigWarriorRight);
                    //select na index -1 ?
                    //armySlots[index -1].OnSelect();
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

        
        public void TrySwitchUnits(int firstIndex, int secondIndex)
        {
            if (SlotManager.Slots[firstIndex].UnitType != SlotManager.Slots[secondIndex].UnitType)
            {
                
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
                Debug.Log("sdalkfjlkj");
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
