using System;
using System.Collections.Generic;
using System.Text;

namespace core.Services.Services.Text
{
    public interface ITextService
    {
        string StyleName(string name);
        string GenerateSlug(string title);
    }
}
