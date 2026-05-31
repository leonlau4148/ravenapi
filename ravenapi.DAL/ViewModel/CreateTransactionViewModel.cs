namespace ravenapi.DAL.ViewModels.Transaction;

public class CreateTransactionViewModel
{
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;        // "income" or "expense"
    public string Description { get; set; } = string.Empty;
    public DateOnly TransactionDate { get; set; }
}