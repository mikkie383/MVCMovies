using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcMovie.Models
{
    public class Movie
    {
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Display(Name ="Image URL")]
        public string ImageUrl { get; set; }

        //navigation
        public Genre Genre { get; set; }

        [Display(Name ="Image File")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}