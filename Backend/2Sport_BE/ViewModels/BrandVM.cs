﻿using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.ViewModels
{
    public class BrandDTO
    {
        public string BrandName { get; set; }

    }
    public class BrandVM : BrandDTO
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public bool? Status { get; set; }
        public string Logo { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BrandCM : BrandDTO
    {
        public IFormFile? LogoImage { get; set; }
    }

    public class BrandUM : BrandDTO
    {
        public IFormFile? LogoImage { get; set; }
    }

}
