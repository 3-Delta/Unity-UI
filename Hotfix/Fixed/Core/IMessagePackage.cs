#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;

namespace Logic.Hotfix.Fixed.Pbf
{
    #region Messages
    public sealed class Package<T> : pb::IMessage where T : pb::IMessage, new()
    {
        private static readonly pb::MessageParser<Package<T>> _parser = new pb::MessageParser<Package<T>>(() => new Package<T>());
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<Package<T>> Parser { get { return _parser; } }

        /// <summary>Field number for the "header" field.</summary>
        public const int HeaderFieldNumber = 1;
        private global::Logic.Hotfix.Fixed.Pbf.PackageHeader header_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public global::Logic.Hotfix.Fixed.Pbf.PackageHeader Header
        {
            get { return header_; }
            set
            {
                header_ = value;
            }
        }

        /// <summary>Field number for the "body" field.</summary>
        public const int BufferFieldNumber = 2;
        private T body_;
        /// <summary>
        /// 数据部分
        /// </summary>
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public T Body
        {
            get { return body_; }
            set
            {
                body_ = value;
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void WriteTo(pb::CodedOutputStream output)
        {
            if (header_ != null)
            {
                output.WriteRawTag(10);
                output.WriteMessage(Header);
            }
            if (body_ != null)
            {
                output.WriteRawTag(18); // 18不知道计算出来的？但是发现如果Buffer字段是IMessage的话，那么这个值始终是18
                output.WriteMessage(Body);
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize()
        {
            int size = 0;
            if (header_ != null)
            {
                size += 1 + pb::CodedOutputStream.ComputeMessageSize(Header);
            }
            if (body_ != null)
            {
                size += 1 + pb::CodedOutputStream.ComputeMessageSize(Body);
            }
            return size;
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public void MergeFrom(pb::CodedInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch (tag)
                {
                    default:
                        input.SkipLastField();
                        break;
                    case 10:
                        {
                            if (header_ == null)
                            {
                                header_ = new global::Logic.Hotfix.Fixed.Pbf.PackageHeader();
                            }
                            input.ReadMessage(header_);
                            break;
                        }
                    case 18:
                        {
                            if (body_ == null)
                            {
                                body_ = new T();
                            }
                            input.ReadMessage(body_);
                            break;
                        }
                }
            }
        }

    }
    #endregion
}

#endregion Designer generated code

