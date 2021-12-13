using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using UnityEngine;

[Serializable]
public class DataTableElemBase : ISerializable
{
    public string id;

    public DataTableElemBase(Dictionary<string, string> data)
    {

    }

    protected DataTableElemBase(SerializationInfo info, StreamingContext context)
    {

    }

    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {

    }

}
