using DashboardAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Globalization;

namespace DashboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Dashboard
        [HttpGet("")]
        public async Task<JsonRes> Index()
        {
            DateTime StartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime EndDate = DateTime.Today;
            DateTime LastWeekDate = EndDate.AddDays(-6);

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .ToListAsync();

            SortedList result = new();

            //Total Income
            double TotalIncome = SelectedTransactions
                .Where(i => i.Category!.Type == "Income")
                .Sum(j => j.Amount);
            result.Add("TotalIncome", TotalIncome);

            //Total Expense
            double TotalExpense = SelectedTransactions
                .Where(i => i.Category!.Type == "Expense")
                .Sum(j => j.Amount);
            result.Add("TotalExpense", TotalExpense);

            //Monthly Income
            double MonthlyIncome = SelectedTransactions
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .Where(i => i.Category!.Type == "Income")
                .Sum(j => j.Amount);

            //Monthly Expense
            double MonthlyExpense = SelectedTransactions
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .Where(i => i.Category!.Type == "Expense")
                .Sum(j => j.Amount);

            //Balance
            double Balance = TotalIncome - TotalExpense;
            result.Add("Balance", Balance);
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-IN");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            var BalanceFormatted = String.Format(culture, "{0:C0}", Balance);
            result.Add("BalanceFormatted", BalanceFormatted);
            var BalancePercentage = Math.Floor((MonthlyExpense / MonthlyIncome) * 100);
            if (double.IsInfinity(BalancePercentage))
            {
                result.Add("BalancePercentage", 0);
            }
            else
            {
                result.Add("BalancePercentage", BalancePercentage);
            }

            //Donut Chart Data
            var DoughnutChartData = SelectedTransactions
                .Where(i => i.Category!.Type == "Expense")
                .GroupBy(j => j.Category!.CategoryId)
                .Select(k => new
                {
                    categoryTitle = k.First().Category!.Name,
                    amount = k.Sum(j => j.Amount)
                })
                .OrderByDescending(l => l.amount)
                .ToList();
            result.Add("DoughnutChartData", DoughnutChartData);

            //Income Summary
            List<AreaChart> IncomeSummary = SelectedTransactions
                .Where(y => y.Date >= LastWeekDate && y.Date <= EndDate)
                .Where(i => i.Category!.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new AreaChart()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                })
                .ToList();

            //Expense Summary
            List<AreaChart> ExpenseSummary = SelectedTransactions
                .Where(y => y.Date >= LastWeekDate && y.Date <= EndDate)
                .Where(i => i.Category!.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new AreaChart()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                })
                .ToList();

            var TransLast7Days = IncomeSummary.Count() + ExpenseSummary.Count();
            result.Add("TransLast7Days", TransLast7Days);

            //Combine Income & Expense
            string[] Last7Days = Enumerable.Range(0, 7)
                .Select(i => LastWeekDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            var AreaChartData = from day in Last7Days
                                join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                from income in dayIncomeJoined.DefaultIfEmpty()
                                join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                from expense in expenseJoined.DefaultIfEmpty()
                                select new
                                {
                                    day,
                                    income = income == null ? 0 : income.income,
                                    expense = expense == null ? 0 : expense.expense,
                                };
            result.Add("AreaChartData", AreaChartData);

            return new JsonRes
            {
                Data = result
            };
        }

        [HttpGet("Notifications")]
        public async Task<JsonRes> Notifications()
        {
            var StartDate = DateTime.Today.AddDays(1);
            var LastDate = DateTime.Today.AddDays(30);

            // Get notification category and date
            var SelectedTransactions = await _context.Transactions
            .Include(x => x.Category)
            .Where(k => k.Category!.Type == "Expense")
            .Where(y => y.Date >= StartDate && y.Date <= LastDate)
            .Select(k => new
            {
                categoryName = k.Category!.Name,
                date = k.Date
            })
            .ToListAsync();

            return new JsonRes
            {
                Data = SelectedTransactions
            };
        }

        public class AreaChart
        {
            public string? day;
            public double income;
            public double expense;
        }
    }
}
