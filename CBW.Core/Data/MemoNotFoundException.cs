using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Core
{
    [Serializable]
    public class MemoNotFoundException : Exception
    {
        public MemoNotFoundException() { }
        public MemoNotFoundException(int id)
            : this(String.Format("Memo id:{0} is not found", id))
        {
        }
        public MemoNotFoundException(string message) : base(message) { }
        public MemoNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected MemoNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
