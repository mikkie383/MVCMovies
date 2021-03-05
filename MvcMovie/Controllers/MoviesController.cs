using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MvcMovieContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MoviesController(MvcMovieContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string searchString, string sortBy)
        {
            var movies = from m in _context.Movie
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "Title":
                        movies = movies.OrderBy(m => m.Title);
                        break;

                    case "ReleaseDate":
                        movies = movies.OrderBy(m => m.ReleaseDate);
                        break;

                    case "Genre":
                        movies = movies.OrderBy(m => m.Genre);
                        break;

                    case "Price":
                        movies = movies.OrderBy(m => m.Price);
                        break;
                }
            }

                return View(await movies.Include(m => m.Genre).ToListAsync());
            
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "GenreId", "GenreName");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,ReleaseDate,GenreId,Price,ImageFile")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                if (movie.ImageFile != null)
                {
                    //save images to wwwroot/images
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(movie.ImageFile.FileName);
                    string extension = Path.GetExtension(movie.ImageFile.FileName);
                    movie.ImageUrl = fileName = fileName + DateTime.Now.ToString("yymmssffff") + extension;
                    string path = Path.Combine(wwwRootPath + "/images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await movie.ImageFile.CopyToAsync(fileStream);
                    }
                }
                //insert record
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "GenreId", "GenreId", movie.GenreId);
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "GenreId", "GenreName", movie.GenreId);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,ReleaseDate,GenreId,Price,ImageFile")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (movie.ImageFile != null)
                    {
                        //save images to wwwroot/images
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(movie.ImageFile.FileName);
                        string extension = Path.GetExtension(movie.ImageFile.FileName);
                        movie.ImageUrl = fileName = fileName + DateTime.Now.ToString("yymmssffff") + extension;
                        string path = Path.Combine(wwwRootPath + "/images/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await movie.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    //update record
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
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
            //ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "GenreId", "GenreId", movie.GenreId);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);

            if (movie.ImageFile != null)
            {
                //delete image from wwwroot/images
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", movie.ImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            //delete the record
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.MovieId == id);
        }
    }
}
