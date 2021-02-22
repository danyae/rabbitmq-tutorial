using System.ComponentModel.DataAnnotations;

namespace RabbitMq.Tutorial.Store.Options
{
    public class RabbitmqOptions
    {
        [Required]
        public string HostName { get; set; }
        
        [Range(1, 9999)]
        public int Port { get; set; }
    }
}
