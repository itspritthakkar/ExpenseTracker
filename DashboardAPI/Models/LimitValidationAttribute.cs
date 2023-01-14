using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DashboardAPI.Models
{
    public class LimitValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)

        {
            var _context = (ApplicationDbContext?)validationContext.GetService(typeof(ApplicationDbContext));
            var transaction = (Transaction)validationContext.ObjectInstance;
            var category = _context!.Categories.Find(transaction.CategoryId);
            if (category == null)
            {
                return new ValidationResult("Category not found");
            }
            if (category!.Type == "Expense")
            {
                string Message = string.Empty;

                List<Transaction> SelectedTransactions = _context!.Transactions.Include(x => x.Category).ToList();

                //Total Income
                double TotalIncome = SelectedTransactions
                    .Where(i => i.Category!.Type == "Income")
                    .Sum(j => j.Amount);

                //Total Expense
                double TotalExpense = SelectedTransactions
                    .Where(i => i.Category!.Type == "Expense")
                    .Sum(j => j.Amount);

                double Balance = TotalIncome - TotalExpense;

                Transaction? untrackedTransaction = _context.Transactions.AsNoTracking().Where(s => s.TransactionId == transaction.TransactionId).FirstOrDefault();
                if (transaction.CategoryId != 0)
                {
                    if(untrackedTransaction != null)
                    {
                        Balance -= untrackedTransaction.Amount;
                    }
                }

                if (Balance == 0)
                {
                    Message = $"Please add some income, available balance: {Balance}";

                    return new ValidationResult(Message);
                }

                if ((double?)value > Balance)
                {
                    Message = $"Expense amount more than available balance: {Balance}";

                    return new ValidationResult(Message);
                }

                DateTime StartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime EndDate = DateTime.Today;
                double transactionSum = _context!.Transactions.Where(i => i.CategoryId == transaction.CategoryId).Where(y => y.Date >= StartDate && y.Date <= EndDate).Sum(j => j.Amount);
                double categoryLimit = category.Limit - transactionSum;
                if (untrackedTransaction != null)
                {
                    categoryLimit += untrackedTransaction.Amount;
                }
                
                if ((double?)value > categoryLimit)
                {
                    Message = $"Expense amount more than available category limit: {categoryLimit}";

                    return new ValidationResult(Message);
                }
            }

            return ValidationResult.Success;

        }
    }
}
