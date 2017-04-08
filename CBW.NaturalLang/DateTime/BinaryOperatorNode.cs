using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang
{
    public class BinaryOperatorNode : OperatorNode
    {
        public BinaryOperatorNode(ITimeEvalNode leftChild, ITimeEvalNode rightChild, DateTimeOperation operation)
            : this(leftChild, rightChild, new OperatorInfo() { Operation = operation })
        {
        }

        public BinaryOperatorNode(ITimeEvalNode leftChild, ITimeEvalNode rightChild, OperatorInfo info)
            : base(info, 2)
        {
            this.LeftChild = leftChild;
            this.RightChild = rightChild;
            this.Evaluate =
                (DateTime now, DateTime dt, DateTimeOperation op) =>
                    rightChild.Evaluate(
                    now, leftChild.Evaluate(now, dt, op), (DateTimeOperation)(((int)this.Info.Operation) * ((int)op)));
        }

        public ITimeEvalNode LeftChild
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
        public ITimeEvalNode RightChild
        {
            get
            {
                if (this.Children.Count < 2)
                {
                    return null;
                }
                else
                {
                    return this.Children[1];
                }
            }
            set
            {
                while (this.Children.Count < 2)
                {
                    this.Children.Add(null);
                }
                this.Children[1] = value;
            }
        }
    }
}
