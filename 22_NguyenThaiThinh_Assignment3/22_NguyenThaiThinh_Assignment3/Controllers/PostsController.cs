﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _22_NguyenThaiThinh_Assignment3.Models;
using _22_NguyenThaiThinh_Assignment3.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace _22_NguyenThaiThinh_Assignment3.Controllers
{
    public class PostsController : Controller
    {
        private readonly Ass3_Prn221_Bl5Context _context;
        private readonly IHubContext<SignalRServer> _signalRHub;

        public PostsController(Ass3_Prn221_Bl5Context context, IHubContext<SignalRServer> signalRHub)
        {
            _context = context;
            _signalRHub = signalRHub;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Type") == "1")
            {
                var ass3_Prn221_Bl5Context = _context.Posts.Include(p => p.Author).Include(p => p.Category);
                return View(await ass3_Prn221_Bl5Context.ToListAsync());
            }
            else
            {
                var ass3_Prn221_Bl5Context = _context.Posts.Where(p => p.PublishStatus == true).Include(p => p.Author).Include(p => p.Category);
                return View(await ass3_Prn221_Bl5Context.ToListAsync());
            }
            
        }
        public IActionResult SortByDateDescending()
        {
            if (HttpContext.Session.GetString("Type") == "1")
            {
                var sortedPosts = _context.Posts.OrderByDescending(p => p.CreatedDate).Include(p => p.Author).Include(p => p.Category).ToList();
                return View("Index", sortedPosts);
            }
            else
            {
                var sortedPosts = _context.Posts.Where(p => p.PublishStatus == true).OrderByDescending(p => p.CreatedDate).Include(p => p.Author).Include(p => p.Category).ToList();
                return View("Index", sortedPosts);
            }
            
        }
        public async Task<IActionResult> GenReport(DateTime? startDate, DateTime? endDate)
        {
            if(HttpContext.Session.GetString("Type") == "1")
            {
                var ass3_Prn221_Bl5Context = _context.Posts.Include(p => p.Author).Include(p => p.Category);
                var postsInPeriod = _context.Posts
            .Where(p => p.CreatedDate >= startDate && p.CreatedDate <= endDate)
            .OrderByDescending(p => p.CreatedDate)
            .Include(p => p.Author)
            .Include(p => p.Category)
            .ToList();
                var categoryCounts = postsInPeriod
            .GroupBy(p => p.Category.CategoryName)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .OrderByDescending(c => c.Count)
            .FirstOrDefault();

                // Gửi danh sách bài viết trong khoảng thời gian và thông tin về category được sử dụng nhiều nhất đến view
                ViewBag.Posts = postsInPeriod;
                ViewBag.MostUsedCategory = categoryCounts;
                return View("Index", await ass3_Prn221_Bl5Context.ToListAsync());
            }
            else
            {
                var ass3_Prn221_Bl5Context = _context.Posts.Include(p => p.Author).Include(p => p.Category);
                var postsInPeriod = _context.Posts
            .Where(p => p.CreatedDate >= startDate && p.CreatedDate <= endDate && p.PublishStatus == true)
            .OrderByDescending(p => p.CreatedDate)
            .Include(p => p.Author)
            .Include(p => p.Category)
            .ToList();
                var categoryCounts = postsInPeriod
            .GroupBy(p => p.Category.CategoryName)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .OrderByDescending(c => c.Count)
            .FirstOrDefault();

                // Gửi danh sách bài viết trong khoảng thời gian và thông tin về category được sử dụng nhiều nhất đến view
                ViewBag.Posts = postsInPeriod;
                ViewBag.MostUsedCategory = categoryCounts;
                return View("Index", await ass3_Prn221_Bl5Context.ToListAsync());
            }
            
        }
        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.AppUsers, "UserId", "UserId");
            ViewData["CategoryId"] = new SelectList(_context.PostCategories, "CategoriesId", "CategoriesId");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,AuthorId,CreatedDate,UpdatedDate,Title,Content,PublishStatus,CategoryId")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                await _signalRHub.Clients.All.SendAsync("LoadPosts");
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.AppUsers, "UserId", "UserId", post.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.PostCategories, "CategoriesId", "CategoriesId", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.AppUsers, "UserId", "UserId", post.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.PostCategories, "CategoriesId", "CategoriesId", post.CategoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,AuthorId,CreatedDate,UpdatedDate,Title,Content,PublishStatus,CategoryId")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    await _signalRHub.Clients.All.SendAsync("LoadPosts");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.AppUsers, "UserId", "UserId", post.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.PostCategories, "CategoriesId", "CategoriesId", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'Ass3_Prn221_Bl5Context.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            await _signalRHub.Clients.All.SendAsync("LoadPosts");
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}
