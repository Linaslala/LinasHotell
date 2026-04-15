using System;
using System.Collections.Generic;
using System.Text;

namespace LinasHotell.GlobalUtilities.SoftDelete
{
    public interface ISoftDelete
    {
       public bool IsDeleted { get; set; }

        public void Undo()
        {
            IsDeleted = false;
        }
    }
}
