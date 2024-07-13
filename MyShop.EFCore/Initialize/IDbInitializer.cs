using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.EFCore.Initialize
{
    public interface IDbInitializer
    {
        void Initialize();
    }
}
