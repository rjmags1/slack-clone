namespace PersistenceService.Utils.GraphQL;

public struct FieldTree
{
    public string FieldName { get; set; }
    public List<FieldTree> Children { get; set; } = new List<FieldTree>();

    public FieldTree(string name)
    {
        FieldName = name;
    }
}

public struct FieldInfo
{
    public FieldTree FieldTree { get; set; }
    public List<string> SubfieldNames { get; set; }
}
