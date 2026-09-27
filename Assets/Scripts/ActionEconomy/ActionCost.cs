[System.Serializable]
public struct ActionCost
{
    public ResourceTypeSO resourceType;
    public int amount;

    public ActionCost(ResourceTypeSO resourceType, int amount)
    {
        this.resourceType = resourceType;
        this.amount = amount;
    }
}
