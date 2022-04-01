using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Enums.TigerGraph
{
    public enum OperationCode
    {
        /// <summary>
        /// If the vertex/edge does not exist, use the payload value to initialize the attribute; 
        /// but if the vertex/edge already exists, do not change this attribute.
        /// </summary>
        IgnoreIfExists = 1,
        /// <summary>
        /// Add the payload value to the existing value.
        /// </summary>
        Add = 2,
        /// <summary>
        ///  Update to the logical AND of the payload value and the existing value.
        /// </summary>
        And = 3,
        /// <summary>
        /// Update to the logical OR of the payload value and the existing value.
        /// </summary>
        Or = 4,
        /// <summary>
        /// Update to the higher value between the payload value and the existing value.
        /// </summary>
        Max = 5,
        /// <summary>
        /// Update to the lower value between the payload value and the existing value.
        /// </summary>
        Min = 6
    }
}
