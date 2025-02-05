using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomProj.Model
{
    public class Product
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public double FixedPrice { get; set; }
        
         public double DiscountPrice { get; set; }

        public string? Image { get; set; }


        //foreign key relation shown
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }    
        
        
        [Required]
        [Display(Name = "SubCategory")]
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public SubCategory SubCategory { get; set; }


    }
}
