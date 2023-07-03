// NOTE: This is a LEGACY file. I'm only keeping this for backwards compatibility (when upgrading). If you're using this
// you should think about replacing this MP_ file with the original vEquipSlot component on your character.
using EMI.Player;
using UnityEngine;
using Mirror;

namespace Invector.vItemManager
{
    public class MP_vEquipSlot : vEquipSlot
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        protected MP_vInventory inv = null;
        protected int slotIndex = -1;
        protected int areaIndex = -1;
        protected bool inited = false;
        public vItem hasItem = null;
        #endregion

        #region Initialization
        protected override void Start()
        {
            if (!inited)
            {
                InitilizateValues();
            }
            base.Start();
        }
        protected virtual void InitilizateValues()
        {
            vEquipArea area = GetComponentsInParent<vEquipArea>(true)[0];
            MP_vInventory inv = GetComponentsInParent<MP_vInventory>(true)[0];
            areaIndex = inv.equipAreas.vToList().FindIndex(x => x.Equals(area));
            slotIndex = area.equipSlots.FindIndex(x => x == this);
            nc = GetComponentInParent<MeleeNetworkCalls>();
            inited = true;
        }
        #endregion

        #region Overrides
        public override void AddItem(vItem item)
        {
            base.AddItem(item);
            if (NetworkServer.active) 
                return;
            else if (nc && nc.hasAuthority && item != null && nc.GetServerItem(areaIndex, slotIndex) != item.id)
            {
                if (slotIndex == -1 || areaIndex == -1)
                    InitilizateValues();
                nc.Cmd_OnAddedToSlot(item.id, areaIndex, slotIndex);
            }
        }

        public override void RemoveItem()
        {
            base.RemoveItem();
            if (NetworkServer.active)
                return;
            else if (nc && nc.hasAuthority && nc.GetServerItem(areaIndex, slotIndex) != -1)
            {
                if (slotIndex == -1 || areaIndex == -1)
                    InitilizateValues();
                nc.Cmd_OnRemovedFromSlot(areaIndex, slotIndex);
            }
        }
        #endregion
    }
}
