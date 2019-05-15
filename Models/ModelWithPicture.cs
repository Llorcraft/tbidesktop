using System;

namespace TbiDesktop.Models
{
    public class ModelWithPicture
    {
        public string id { get; set; } = (new Random()).ToString().Substring(2);
        public string picture { get; set; }
    }
}