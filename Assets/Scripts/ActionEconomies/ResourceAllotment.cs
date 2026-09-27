namespace Assets.Scripts.ActionEconomies
{
    [System.Serializable]
    public struct ResourceAllotment
    {
        public ResourceTypeSO resourceType;
        public int maxAmount;

        public ResourceAllotment(ResourceTypeSO resourceType, int maxAmount)
        {
            this.resourceType = resourceType;
            this.maxAmount = maxAmount;
        }
    }
}