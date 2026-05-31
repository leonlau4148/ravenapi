namespace ravenapi.DAL.Entities;

public class Budget
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public decimal LimitAmount { get; set; }
    public int Month { get; set; }   // 1–12
    public int Year { get; set; }

    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}