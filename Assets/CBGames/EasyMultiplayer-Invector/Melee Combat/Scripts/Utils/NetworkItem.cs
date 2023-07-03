using Invector.vItemManager;
using System.Collections.Generic;
using System.IO;

namespace EMI.Utils
{
    #region Helper Class
    [System.Serializable]
    public class NetworkAttribute
    {
        public vItemAttributes name = 0;
        public int value = 0;
        public bool isOpen;
        public bool isBool;
        public string displayFormat;

        public NetworkAttribute(vItemAttribute attribute)
        {
            name = attribute.name;
            value = attribute.value;
            isOpen = attribute.isOpen;
            isBool = attribute.isBool;
            displayFormat = attribute.displayFormat;
        }
    }
    #endregion

    [System.Serializable]
    public class NetworkItem
    {
        #region Properties
        public int id = 0;
        public string description = "Item Description";
        public byte type;
        public bool stackable = true;
        public int amount;
        public List<NetworkAttribute> attributes = new List<NetworkAttribute>();
        public bool isInEquipArea;
        public bool destroyAfterUse = true;
        public bool canBeUsed = true;
        public bool canBeDroped = true;
        public bool canBeDestroyed = true;
        public string EnableAnim = "LowBack";
        public string DisableAnim = "LowBack";
        public float enableDelayTime = 0.5f;
        public float disableDelayTime = 0.5f;
        public string customHandler;
        public bool twoHandWeapon;
        #endregion

        #region Constructors
        public NetworkItem() { }

        public NetworkItem(vItem item)
        {
            id = item.id;
            description = item.description;
            type = (byte)item.type;
            stackable = item.stackable;
            amount = item.amount;
            isInEquipArea = item.isInEquipArea;
            destroyAfterUse = item.destroyAfterUse;
            canBeUsed = item.canBeUsed;
            canBeDroped = item.canBeDroped;
            canBeDestroyed = item.canBeDestroyed;
            EnableAnim = item.EnableAnim;
            DisableAnim = item.DisableAnim;
            enableDelayTime = item.enableDelayTime;
            disableDelayTime = item.disableDelayTime;
            customHandler = item.customHandler;
            twoHandWeapon = item.twoHandWeapon;
            List<NetworkAttribute> att = new List<NetworkAttribute>();
            foreach(vItemAttribute attribute in item.attributes)
            {
                att.Add(new NetworkAttribute(attribute));
            }
        }
        #endregion

        #region Serialization For Network Calls
        /// <summary>
        /// This will convert the NetworkItem to a byte array
        /// </summary>
        /// <returns></returns>
        public virtual byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(id);
                    writer.Write(description);
                    writer.Write(type);
                    writer.Write(stackable);
                    writer.Write(amount);
                    writer.Write(isInEquipArea);
                    writer.Write(destroyAfterUse);
                    writer.Write(canBeUsed);
                    writer.Write(canBeDroped);
                    writer.Write(canBeDestroyed);
                    writer.Write(EnableAnim);
                    writer.Write(DisableAnim);
                    writer.Write(enableDelayTime);
                    writer.Write(disableDelayTime);
                    writer.Write(customHandler);
                    writer.Write(twoHandWeapon);
                }
                return m.ToArray();
            }
        }

        /// <summary>
        /// This will convert the byte array to a NetworkItem class
        /// </summary>
        /// <param name="data"></param>
        public virtual void Desserialize(byte[] data)
        {
            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    this.id = reader.ReadInt32();
                    this.description = reader.ReadString();
                    this.type = reader.ReadByte();
                    this.stackable = reader.ReadBoolean();
                    this.amount = reader.ReadInt32();
                    this.isInEquipArea = reader.ReadBoolean();
                    this.destroyAfterUse = reader.ReadBoolean();
                    this.canBeUsed = reader.ReadBoolean();
                    this.canBeDroped = reader.ReadBoolean();
                    this.canBeDestroyed = reader.ReadBoolean();
                    this.EnableAnim = reader.ReadString();
                    this.DisableAnim = reader.ReadString();
                    this.enableDelayTime = reader.ReadSingle();
                    this.disableDelayTime = reader.ReadSingle();
                    this.customHandler = reader.ReadString();
                    this.twoHandWeapon = reader.ReadBoolean();
                }
            }
        }
        #endregion

        #region Convert To VItem
        public virtual void AddTo(ref vItem item)
        {
            item.description = this.description;
            item.type = (vItemType)this.type;
            item.stackable = this.stackable;
            item.amount = this.amount;
            item.isInEquipArea = this.isInEquipArea;
            item.destroyAfterUse = this.destroyAfterUse;
            item.canBeUsed = this.canBeUsed;
            item.canBeDroped = this.canBeDroped;
            item.canBeDestroyed = this.canBeDestroyed;
            item.EnableAnim = this.EnableAnim;
            item.DisableAnim = this.DisableAnim;
            item.enableDelayTime = this.enableDelayTime;
            item.disableDelayTime = this.disableDelayTime;
            item.customHandler = this.customHandler;
            item.twoHandWeapon = this.twoHandWeapon;

            List<vItemAttribute> finalAttributes = new List<vItemAttribute>();
            foreach (NetworkAttribute attribute in this.attributes)
            {
                vItemAttribute att = item.attributes.Find(x => x.name == attribute.name);
                if (att == null)
                {
                    att = new vItemAttribute(attribute.name, attribute.value);
                    finalAttributes.Add(att);
                }
                else
                {
                    att.value = attribute.value;
                    att.isOpen = attribute.isOpen;
                    att.isBool = attribute.isBool;
                    finalAttributes.Add(att);
                }
            }
            item.attributes.Clear();
            item.attributes.AddRange(finalAttributes);
        }
        #endregion
    }
}
