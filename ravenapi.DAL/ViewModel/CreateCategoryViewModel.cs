namespace ravenapi.DAL.ViewModels.Category;

public class CreateCategoryViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;   // "income" or "expense"
    public string Icon { get; set; } = string.Empty;
}