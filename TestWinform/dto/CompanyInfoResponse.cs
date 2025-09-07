using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWinform.models;

namespace TestWinform.dto
{
    internal class CompanyInfoResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CompanyInfo Data { get; set; }
    }
}
