using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSystem.Exceptions
{
    public class UnknownEntityTypeException:Exception
    {

        public UnknownEntityTypeException(string typeIdentifier)
            : base($"نوع غير معروف في قاعدة البيانات: '{typeIdentifier}'. تأكد من صحة تنسيق الملف.") { }
    }
}
