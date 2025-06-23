using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMMS.Application.DataObject.RequestObject
{
    public class VerifyPhoneRequest
    {
        public string IdToken { get; set; }
        public string PhoneNumber { get; set; }
    }
}
