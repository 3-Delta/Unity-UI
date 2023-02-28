#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using UnityEngine;

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using scg = global::System.Collections.Generic;

namespace Logic.Hotfix.Fixed.Pbf
{
    #region Messages
    public sealed class PackageBody<T> : pb::IMessage where T : pb::IMessage, new()
    {
        private static readonly pb::MessageParser<PackageBody<T>> _parser = new pb::MessageParser<PackageBody<T>>(() => new PackageBody<T>());
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static pb::MessageParser<PackageBody<T>> Parser { get { return _parser; } }

        /// <summary>Field number for the "split" field.</summary>
        public const int SplitFieldNumber = 1;
        private global::Logic.Hotfix.Fixed.Pbf.PackageSplit split_;
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public global::Logic.Hotfix.Fixed.Pbf.PackageSplit Split
        {
            get { return split_; }
            set
            {
                split_ = value;
            }
        }

        /// <summary>Field number for the "body" field.</summary>
        public const int BodyFieldNumber = 2;
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
            if (split_ != null)
            {
                output.WriteRawTag(10);
                output.WriteMessage(Split);
            }
            if (body_ != null)
            {
                output.WriteRawTag(18);
                output.WriteMessage(Body);
            }
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public int CalculateSize()
        {
            int size = 0;
            if (split_ != null)
            {
                size += 1 + pb::CodedOutputStream.ComputeMessageSize(Split);
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
                            if (split_ == null)
                            {
                                split_ = new global::Logic.Hotfix.Fixed.Pbf.PackageSplit();
                            }
                            input.ReadMessage(split_);
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

