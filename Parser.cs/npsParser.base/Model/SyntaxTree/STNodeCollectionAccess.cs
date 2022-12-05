﻿using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.syntaxtree
{

    /// <summary>
    /// An expr-node to access sub elements in a collection.
    /// 
    /// The collection may have multiple keys, like 2D-array or 3D-array.
    /// 
    /// </summary>
    class STNodeCollectionAccess : STNodeBase
    {
        public STNodeCollectionAccess()
            : base("array")
        {
        }

    }

}