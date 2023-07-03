using UnityEditor;
using UnityEngine;

namespace EMI.Utils
{
    public class ContextUtils : Editor
    {
        public static SerializedObject EMI_SOURCE_COPY_COMP = null; //tmp value to store the original source component to copy from

        [MenuItem("CONTEXT/Component/EMI/Deep Copy Values")]
        public static void EMI_COPY_COMPONENT(MenuCommand command)
        {
            EMI_SOURCE_COPY_COMP = new SerializedObject(command.context); //save the source component to copy the values from
        }

        [MenuItem("CONTEXT/Component/EMI/Deep Paste Values")]
        public static void EMI_PASTE_COMPONENT(MenuCommand command)
        {
            if (EMI_SOURCE_COPY_COMP == null) //Make sure there is an original component selected.
            {
                if (EditorUtility.DisplayDialog("No Original Component",
                    "You didn't copy the values from any other component. So there is nothing to paste.",
                    "Okay"))
                {
                    return;
                }
            }
            else
            {
                SerializedObject dest = new SerializedObject(command.context);
                EditorUtils.DeepCopyValues((Component)EMI_SOURCE_COPY_COMP.targetObject, (Component)dest.targetObject);
            }
        }
    }
}
