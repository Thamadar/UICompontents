using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WPF.Services
{
    public class ViewLocatorService
    { 
        private static readonly Lazy<ViewLocatorService> _instance = new Lazy<ViewLocatorService>(() => new ViewLocatorService());


    }
}
