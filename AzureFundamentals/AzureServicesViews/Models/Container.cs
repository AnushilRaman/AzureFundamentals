using System.ComponentModel.DataAnnotations;

namespace AzureServicesViews.Models
{
    public class Container
    {
        [Required]
        public string Name { get; set; }
    }
}
