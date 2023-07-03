namespace EMI.UI
{
    public partial class UIValueStorage {
        public static UIValueStorage instance = null;
        partial void UpdateName();

        public void CallUpdateName()
        {
            UpdateName();
        }
    }
}
