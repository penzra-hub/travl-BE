using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travl.Domain.Commons
{
    public class ResponseCodes
    {
        public const string SUCCESS = "00";
        public const string CREATED = "01";
        public const string UPDATED = "02";
        public const string DELETED = "03";
        public const string BAD_REQUEST = "40";
        public const string UNAUTHORIZED = "41";
        public const string FORBIDDEN = "43";
        public const string NOT_FOUND = "44";
        public const string DELETED_RESOURCE = "97";
        public const string FAILURE = "06";
        public const string SERVER_ERROR = "99";
    }
}
