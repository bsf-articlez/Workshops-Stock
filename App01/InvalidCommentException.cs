using System;
using System.Collections.Generic;
using System.Text;

namespace App01
{
    public class InvalidCommentException : Exception
    {
        // field (class-level vars)

        // ctor
        public InvalidCommentException(int lineNumber)
        {
            LineNumber = lineNumber;
        }
        
        // readonly auto-implementd properties
        public int LineNumber { get; }

        // methods
        public override string Message => $"Found invalid comment at line {LineNumber}";
    }
}
