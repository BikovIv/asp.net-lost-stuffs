using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LostStuffs.Entities
{
    public class LostStuff : BaseEntity
    {


       // [Required]
       // [MinLength(10,ErrorMessage = "The title must be 8 characters long.")]
        public string Name { get; set; }

        public string Description { get; set; }
        public int Price { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }

       // [Required]
        //[StringLength(10 , ErrorMessage = "The phone number must be 10 characters long.")]
        public string  PhoneNumber { get; set; }
        public string UserId { get; set; }

        public virtual List<Comment> Comments { get; set; }
    }
}