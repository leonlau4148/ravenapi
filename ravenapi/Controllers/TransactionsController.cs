using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ravenapi.DAL.Data;
using ravenapi.DAL.Entities;
using ravenapi.DAL.ViewModels.Transaction;  // ← added

namespace ravenapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
            .Where(t => t.UserId == CurrentUserId)
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
    public async Task<IActionResult> Create(CreateTransactionViewModel model)  // ← updated
    {
        var transaction = new Transaction
        {
            UserId = CurrentUserId,
            CategoryId = model.CategoryId,   // ← req → model
            Amount = model.Amount,
            Type = model.Type,
            Description = model.Description,
            TransactionDate = model.TransactionDate
        };

        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = transaction.Id }, new
        {
            transaction.Id,
            transaction.Amount,
            transaction.Type,
            transaction.Description,
            transaction.TransactionDate,
            transaction.CategoryId
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateTransactionViewModel model)
    {
        var transaction = await _db.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == CurrentUserId);

        if (transaction is null) return NotFound();

        transaction.CategoryId = model.CategoryId;
        transaction.Amount = model.Amount;
        transaction.Type = model.Type;
        transaction.Description = model.Description;
        transaction.TransactionDate = model.TransactionDate;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            transaction.Id,
            transaction.Amount,
            transaction.Type,
            transaction.Description,
            transaction.TransactionDate,
            transaction.CategoryId
        });
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