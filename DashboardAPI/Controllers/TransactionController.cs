using DashboardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace DashboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Transaction
        [HttpGet]
        public async Task<JsonRes> GetTransactions()
        {
            var transactions = _context.Transactions
                .Include(t => t.Category)
                .OrderByDescending(x => x.TransactionId)
                .Select( c => new {
                    c.TransactionId,
                    c.CategoryId,
                    c.Title,
                    c.Amount,
                    c.FormattedAmount,
                    c.Description,
                    c.Date,
                    categoryName = c.Category!.Name
                });
            return new JsonRes { Data = await transactions.ToListAsync() };
        }

        // GET: api/Transaction/5
        [HttpGet("{id}")]
        public async Task<JsonRes> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return new JsonRes
                {
                    Status = 400,
                    StatusMessage = "Fail",
                    Data = "Not Found"
                };
            }

            SortedList result = new()
            {
                { "selectCategories", new SelectList(_context.Categories, "CategoryId", "Name", transaction.CategoryId)},
                { "transaction", transaction}
            };
            return new JsonRes { Data = result };
        }

        [HttpPost]
        public async Task<JsonRes> PostTransaction(Transaction transaction)
        {
            if (transaction.TransactionId == 0)
            {
                _context.Add(transaction);
            }
            else
            {
                _context.ChangeTracker.Clear();
                _context.Entry(transaction).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(transaction.TransactionId))
                {
                    return new JsonRes
                    {
                        Status = 400,
                        StatusMessage = "Fail",
                        Data = "Not Found"
                    };
                }
                else
                {
                    throw;
                }
            }
            SortedList result = new()
            {
                { "transaction", CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction) }
            };
            return new JsonRes { Data = result };
        }

        // DELETE: api/Transaction/5
        [HttpDelete("{id}")]
        public async Task<JsonRes> DeleteTransaction(int id)
        {
            if (_context.Transactions == null)
            {
                return new JsonRes
                {
                    Status = 404,
                    StatusMessage = "Fail",
                    Data = "Entity set 'ApplicationDbContext.Transaction'  is null."
                };
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();

            return new JsonRes
            {
                Data = "Transaction has been deleted successfully."
            };
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }
    }
}
