using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public class OperatorNode : ITimeEvalNode
    {
        public string Term { get; set; }
        public Func<DateTime, DateTime, DateTimeOperation, DateTime> Evaluate { get; set; }
        public DateTime GetCurrentValue(DateTime now)
        {
            return this.Evaluate(now, now, DateTimeOperation.Additive);
        }
        public OperatorInfo Info { get; set; }
        protected List<ITimeEvalNode> Children { get; set; }

        public OperatorNode(OperatorInfo info, int childrenSize)
        {
            this.Info = info;
            this.Children = new List<ITimeEvalNode>(childrenSize);
        }
    }
}
