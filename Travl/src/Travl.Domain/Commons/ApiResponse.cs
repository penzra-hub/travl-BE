using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Domain.Commons
{
    public class ApiResponse<T>
    {
        public string ResponseCode { get; set; } = ResponseCodes.FAILURE;
        public bool isSuccess { get; set; } = false;
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
