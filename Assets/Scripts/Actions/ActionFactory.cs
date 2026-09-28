using UnityEngine;

namespace Assets.Scripts.Actions
{
    public static class ActionFactory
    {
        public static BaseAction AddAction(GameObject unitObject, ActionType type)
        {
            return type switch
            {
                ActionType.Move => unitObject.AddComponent<MoveAction>(),
                ActionType.Spin => unitObject.AddComponent<SpinAction>(),
                _ => throw new System.ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }
    }
}
