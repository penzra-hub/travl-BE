using Microsoft.AspNetCore.Http;

namespace Travl.Domain.Commons
{
    public class EmailVm
    {
        public string? ToEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string[]? CC { get; set; }
        public string[]? BCC { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }
}
