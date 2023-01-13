using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DashboardAPI.Models;
using System.Collections;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DashboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CategoryController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<JsonRes> GetCategories()
        {
            var categorySummary = await (from ep in _context.Categories
                                         join e in _context.Transactions on ep.CategoryId equals e.CategoryId into categoriesJoined
                                         from f in categoriesJoined.DefaultIfEmpty()
                                         select new
                                         {
                                             ep.CategoryId,
                                             ep.Name,
                                             ep.Icon,
                                             ep.Type,
                                             ep.Bookmark,
                                             ep.Limit,
                                             ep.CreatedDate,
                                             ep.UpdatedDate,
                                             Expense = (f == null ? 0 : f.Amount)
                                         }).ToListAsync();

            var categorySummary2 = categorySummary
                .GroupBy(j => j.CategoryId)
                .Select(k => new
                {
                    k.First().CategoryId,
                    k.First().Name,
                    k.First().Icon,
                    k.First().Type,
                    k.First().Bookmark,
                    k.First().Limit,
                    k.First().CreatedDate,
                    k.First().UpdatedDate,
                    Expense = k.Sum(j => j.Expense)
                })
                .ToList();

            return new JsonRes { Data = categorySummary2.OrderByDescending(x => x.Bookmark) };
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<JsonRes> GetCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);

            if (category == null || _context.Transactions == null)
            {
                return new JsonRes { Status=400, StatusMessage="Fail", Data="Not Found"};
            }

            var transactions = await _context.Transactions
                .Where(i => i.Category!.CategoryId == id)
                .Include(t => t.Category)
                .ToListAsync();

            double totalExpense = transactions.Sum(j => j.Amount);

            DateTime StartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime EndDate = DateTime.Today;

            int transThisMonth = transactions.Where(y => y.Date >= StartDate && y.Date <= EndDate).Count();
            double sumThisMonth = transactions.Where(y => y.Date >= StartDate && y.Date <= EndDate).Sum(j => j.Amount);

            SortedList result = new()
            {
                { "transactions", transactions },
                { "totalExpense", totalExpense },
                { "category", category },
                { "transThisMonth", transThisMonth },
                { "sumThisMonth", sumThisMonth }
            };

            return new JsonRes { Data=result };
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<JsonRes> PostCategory([FromForm, Bind("CategoryId,Name,Type,Limit,Bookmark,IconFile")] Category category)
        {
            if (category.CategoryId == 0)
            {
                if (category.IconFile != null)
                {
                    //Save image to wwwroot/uploads
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(category.IconFile.FileName);
                    string extension = Path.GetExtension(category.IconFile.FileName);
                    category.Icon = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/uploads/", fileName);
                    using var fileStream = new FileStream(path, FileMode.Create);
                    await category.IconFile.CopyToAsync(fileStream);
                }
                _context.Add(category);
            }
            else
            {
                var dbCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.CategoryId == category.CategoryId);
                var Icon = Request.Form["Icon"]!.ToString();
                if (dbCategory != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;

                    // If Icon value changed, delete cuurent icon file
                    if (dbCategory.Icon != Icon)
                    {
                        string iconPath = Path.Combine(wwwRootPath + "/uploads/", dbCategory.Icon);
                        if (System.IO.File.Exists(iconPath))
                        {
                            System.IO.File.Delete(iconPath);
                        }
                    }

                    // If Icon updated, add values and file to storage
                    if (Icon != "" && category.IconFile != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(category.IconFile.FileName);
                        string extension = Path.GetExtension(category.IconFile.FileName);
                        category.Icon = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/uploads/", fileName);
                        using var fileStream = new FileStream(path, FileMode.Create);
                        await category.IconFile.CopyToAsync(fileStream);
                    }

                    // If Icon removed, asign empty string to database entity
                    if (Icon != "" && category.IconFile == null)
                    {
                        category.Icon = Icon;
                    }
                }

                _context.Update(category);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.CategoryId))
                {
                    return new JsonRes { Status = 400, StatusMessage = "Fail", Data = "Not Found" };
                }
                else
                {
                    throw;
                }
            }

            return new JsonRes { Data = CreatedAtAction("GetCategory", new { id = category.CategoryId }, category) };
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<JsonRes> DeleteCategory(int id)
        {
            if (_context.Categories == null)
            {

                return new JsonRes
                {
                    Status = 404,
                    StatusMessage = "Fail",
                    Data = "Entity set 'ApplicationDbContext.Categories' is null."
                };
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                if (category.Icon != "")
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string iconPath = Path.Combine(wwwRootPath + "/uploads/", category.Icon);
                    if (System.IO.File.Exists(iconPath))
                        System.IO.File.Delete(iconPath);
                }
                _context.Categories.Remove(category);
            }
            else
            {
                return new JsonRes 
                { 
                    Status = 400, 
                    StatusMessage = "Fail", 
                    Data = "Not Found"
                };
            }

            await _context.SaveChangesAsync();

            return new JsonRes{Data = "Category has been deleted successfully."};
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        [HttpPost("Bookmark/{id:int}"), ActionName("Bookmark")]
        public async Task<JsonRes> Bookmark(int id)
        {
            if (_context.Categories == null)
            {
                return new JsonRes
                {
                    Status = 404,
                    StatusMessage = "Fail",
                    Data = "Entity set 'ApplicationDbContext.Categories' is null."
                };
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                // Toggle bookmark
                if (category.Bookmark == 0)
                {
                    category.Bookmark = 1;
                }
                else
                {
                    category.Bookmark = 0;
                }
            }

            await _context.SaveChangesAsync();

            return new JsonRes
            {
                Data = "Category bookmark toggled"
            };
        }
    }
}
