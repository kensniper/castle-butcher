using System;
using System.Collections.Generic;
using System.Text;

namespace UDPClientServerCommons
{
    class BadPackageIdException:ApplicationException
    {
        public BadPackageIdException(ushort packageId):base("Couldnt find package Id "+ packageId.ToString()+" in dictionary list, Communication should be veryfied between client and server")
        {
            this.PackageIdField = packageId;
        }

        public BadPackageIdException()
            : base("Couldnt find package Id in dictionary list, Communication should be veryfied between client and server")
        { }

        public BadPackageIdException(string message)
            : base(message)
        {
        }

        public BadPackageIdException(string message, ushort packageId):base(message)
        {
            this.PackageIdField = packageId;
        }

        private ushort PackageIdField;

        public ushort PackageId
        {
            get { return PackageIdField; }
            set { PackageIdField = value; }
        }

        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public override System.Collections.IDictionary Data
        {
            get
            {
                return base.Data;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
