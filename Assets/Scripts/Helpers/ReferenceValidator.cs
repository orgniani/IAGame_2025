using UnityEngine;

namespace Helpers
{
    public class ReferenceValidator
    {
        public static bool Validate(Object reference, string referenceName, MonoBehaviour context)
        {
            if (reference != null) return true;

            Debug.LogError($"[Validator] {context.name}: {referenceName} is null!\nDisabling component to avoid errors.");
            context.enabled = false;
            return false;
        }

        public static bool Validate(object reference, string referenceName, object context)
        {
            if (reference != null) return true;

            Debug.LogError($"[Validator] {referenceName} is null! Context: {context?.GetType().Name ?? "Unknown"}");
            return false;
        }
    }
}