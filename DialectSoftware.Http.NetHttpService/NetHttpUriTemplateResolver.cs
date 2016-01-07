using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialectSoftware.Http.Services
{
    public interface INetHttpUriTemplateResolver
    {
        String GetTemplate(Uri uri);
    }
}
