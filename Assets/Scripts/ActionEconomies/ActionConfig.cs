using Assets.Scripts.Actions;
using System.Collections.Generic;

namespace Assets.Scripts.ActionEconomies
{
    [System.Serializable]
    public struct ActionConfig
    {
        public ActionType actionType;
        public List<ActionCost> actionCosts;
        public int order;
    }
}