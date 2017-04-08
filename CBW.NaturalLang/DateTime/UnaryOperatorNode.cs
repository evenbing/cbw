using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public class UnaryOperatorNode : OperatorNode
    {
        public UnaryOperatorNode(ITimeEvalNode child, DateTimeOperation operation)
            : this(child, new OperatorInfo() { Operation = operation })
        {
        }

        public UnaryOperatorNode(ITimeEvalNode child, OperatorInfo info)
            : base(info, 1)
        {
            this.Child = child;
            this.Evaluate =
                (DateTime now, DateTime dt, DateTimeOperation op) =>
                    child.Evaluate(now, dt, (DateTimeOperation)((int)this.Info.Operation * (int)op));
        }

        public ITimeEvalNode Child
        {
            get
            {
                if (this.Children.Count == 0)
                {
                    return null;
                }
                else
                {
                    return this.Children[0];
                }
            }
            set
            {
                if (this.Children.Count == 0)
                {
                    this.Children.Add(value);
                }
                else
                {
                    this.Children[0] = value;
                }
            }
        }
    }
}
