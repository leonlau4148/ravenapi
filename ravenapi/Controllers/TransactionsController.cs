using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ravenapi.DAL.Data;
using ravenapi.DAL.Entities;

namespace ravenapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]                          // ← entire controller requires JWT
public class TransactionsController : BaseController
{
    private readonly ApplicationDbContext _db;

    public TransactionsController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _db.Transactions
            .Where(t => t.UserId == CurrentUserId)   // ← always scope to user
            .Include(t => t.Category)
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => new
            {
                t.Id,
                t.Amount,
                t.Type,
                t.Description,
                t.TransactionDate,
                Category = t.Category.Name
            })
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionRequest req)
    {
        var transaction = new Transaction
        {
            UserId = CurrentUserId,
            CategoryId = req.CategoryId,
            Amount = req.Amount,
            Type = req.Type,
            Description = req.Description,
            TransactionDate = req.TransactionDate
        };

        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = transaction.Id }, transaction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var transaction = await _db.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == CurrentUserId);

        if (transaction is null) return NotFound();

        _db.Transactions.Remove(transaction);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record CreateTransactionRequest(
    int CategoryId,
    decimal Amount,
    string Type,
    string Description,
    DateOnly TransactionDate
);