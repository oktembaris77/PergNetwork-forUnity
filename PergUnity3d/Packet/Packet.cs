using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using System.Numerics;
using UnityEngine;

/// <summary>Sent from server to client.</summary>
public enum AlreadyServerPackets // Alınacak Veriler
{
    welcome=0,
    pergRPC
}

/// <summary>Sent from client to server.</summary>
public enum AlreadyClientPackets // Gönderilcek Veriler
{
    pergRPC = 1,//0
    serializedData=2
}

public class Packet : IDisposable
{
    public List<byte> buffer;
    private byte[] readableBuffer;
    private int readPos;

    /// <summary>Creates a new empty packet (without an ID).</summary>
    public Packet()
    {
        buffer = new List<byte>(); // Intitialize buffer
        readPos = 0; // Set readPos to 0
    }

    /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
    /// <param name="_id">The packet ID.</param>
    public Packet(int _id, bool a = false)
    {
        buffer = new List<byte>(); // Intitialize buffer
        readPos = 0; // Set readPos to 0

        if(!a)
        Write(_id); // Write packet id to the buffer (Buffer a paket id'yi yazar)
    }

    /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
    /// <param name="_data">The bytes to add to the packet.</param>
    public Packet(byte[] _data)
    {
        buffer = new List<byte>(); // Intitialize buffer
        readPos = 0; // Set readPos to 0

        SetBytes(_data);
    }

    #region Functions
    /// <summary>Sets the packet's content and prepares it to be read.</summary>
    /// <param name="_data">The bytes to add to the packet.</param>
    public void SetBytes(byte[] _data)
    {
        Write(_data);
        readableBuffer = buffer.ToArray();
    }

    /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
    public void WriteLength()
    {
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); // Insert the byte length of the packet at the very beginning
    }

    /// <summary>Inserts the given int at the start of the buffer.</summary>
    /// <param name="_value">The int to insert.</param>
    public void InsertInt(int index, int _value)
    {
        buffer.InsertRange(index, BitConverter.GetBytes(_value)); // Insert the int at the start of the buffer
    }
    /// <summary>Gets the packet's content in array form.</summary>
    public byte[] ToArray()
    {
        readableBuffer = buffer.ToArray();
        return readableBuffer;
    }

    /// <summary>Gets the length of the packet's content.</summary>
    public int Length()
    {
        return buffer.Count; // Return the length of buffer
    }

    /// <summary>Gets the length of the unread data contained in the packet.</summary>
    public int UnreadLength()
    {
        return Length() - readPos; // Return the remaining length (unread)
    }

    /// <summary>Resets the packet instance to allow it to be reused.</summary>
    /// <param name="_shouldReset">Whether or not to reset the packet.</param>
    public void Reset(bool _shouldReset = true)
    {
        if (_shouldReset)
        {
            buffer.Clear(); // Clear buffer
            readableBuffer = null;
            readPos = 0; // Reset readPos
        }
        else
        {
            readPos -= 4; // "Unread" the last read int
        }
    }

    #endregion

    #region Write Data
    public void WriteObject(object _object, string _type)
    {
        switch(_type)
        {
            case "Int16": //short
                Write((short)_object);
                break;
            case "Int":
            case "Int32": //int
                Write((int)_object);
                break;
            case "Int64": //long
                Write((long)_object);
                break;
            case "Float":
            case "Single": //float
                Write((float)_object);
                break;
            case "String": //string
                Write((string)_object);
                break;
            case "Byte": //byte
                Write((byte)_object);
                break;
            case "Trigger":
            case "Bool":
            case "Boolean": //bool
                Write((bool)_object);
                break;
            case "Vector3": //Vector3
                Write((Vector3)_object);
                break;
            case "Quaternion": //Quaternion
                Write((Quaternion)_object);
                break;
        }
    }
    public void WriteObject(object _object, VarType _varType)
    {
        switch (_varType)
        {
            case VarType._short: //short
                Write((short)_object);
                break;
            case VarType._int: //int
                Write((int)_object);
                break;
            case VarType._long: //long
                Write((long)_object);
                break;
            case VarType._float: //float
                Write((float)_object);
                break;
            case VarType._string: //string
                Write((string)_object);
                break;
            case VarType._byte: //byte
                Write((byte)_object);
                break;
            case VarType._boolean: //bool
                Write((bool)_object);
                break;
            case VarType._Vector3: //Vector3
                Write((Vector3)_object);
                break;
            case VarType._Quaternion: //Quaternion
                Write((Quaternion)_object);
                break;
        }
    }

    /// <summary>Adds a byte to the packet.</summary>
    /// <param name="_value">The byte to add.</param>
    public void Write(byte _value)
    {
        buffer.Add(_value);
    }
    /// <summary>Adds an array of bytes to the packet.</summary>
    /// <param name="_value">The byte array to add.</param>
    public void Write(byte[] _value)
    {
        buffer.AddRange(_value);
    }
    /// <summary>Adds a short to the packet.</summary>
    /// <param name="_value">The short to add.</param>
    public void Write(short _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>Adds an int to the packet.</summary>
    /// <param name="_value">The int to add.</param>
    public void Write(int _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));

        /*// C++ server'ına yollamak için bunu kullan !
        List<byte> bytes = new List<byte>();

        for (int i = 0; i < _value.ToString().ToCharArray().Length; i++)
        {
            int k = System.Convert.ToInt32(_value.ToString().ToCharArray()[i]);
            bytes.Add(byte.Parse(k.ToString()));
        }

        byte[] sendingBytes = bytes.ToArray();

        buffer.AddRange(sendingBytes);
        */
    }
    /// <summary>Adds a long to the packet.</summary>
    /// <param name="_value">The long to add.</param>
    public void Write(long _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>Adds a float to the packet.</summary>
    /// <param name="_value">The float to add.</param>
    public void Write(float _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>Adds a bool to the packet.</summary>
    /// <param name="_value">The bool to add.</param>
    public void Write(bool _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }
    /// <summary>Adds a string to the packet.</summary>
    /// <param name="_value">The string to add.</param>
    public void Write(string _value)
    {
        Write(_value.Length); // Add the length of the string to the packet
        buffer.AddRange(Encoding.ASCII.GetBytes(_value)); // Add the string itself
    }
    /// <summary>Adds a Vector3 to the packet.</summary>
    /// <param name="_value">The Vector3 to add.</param>
    public void Write(Vector3 _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);

    }
    /// <summary>Adds a Quaternion to the packet.</summary>
    /// <param name="_value">The Quaternion to add.</param>
    public void Write(Quaternion _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
        Write(_value.w);
    }
    #endregion

    #region Read Data
    public object ReadObject(string _type)
    {
        object data = -1;
        switch (_type)
        {
            case "Int16": //short
                data = ReadShort();
                break;
            case "Int":
            case "Int32": //int
                data = ReadInt();
                break;
            case "Int64": //long
                data = ReadLong();
                break;
            case "Float":
            case "Single": //float
                data = ReadFloat();
                break;
            case "String": //string
                data = ReadString();
                break;
            case "Byte": //byte
                data = ReadByte();
                break;
            case "Trigger":
            case "Bool":
            case "Boolean": //bool
                data = ReadBool();
                break;
            case "Vector3": //Vector3
                data = ReadVector3();
                break;
            case "Quaternion": //Quaternion
                data = ReadQuaternion();
                break;
        }
        return data;
    }
    public object ReadObject(VarType _varType)
    {
        object data = -1;
        switch (_varType)
        {
            case VarType._short: //short
                data = ReadShort();
                break;
            case VarType._int: //int
                data = ReadInt();
                break;
            case VarType._long: //long
                data = ReadLong();
                break;
            case VarType._float: //float
                data = ReadFloat();
                break;
            case VarType._string: //string
                data = ReadString();
                break;
            case VarType._byte: //byte
                data = ReadByte();
                break;
            case VarType._boolean: //bool
                data = ReadBool();
                break;
            case VarType._Vector3: //Vector3
                data = ReadVector3();
                break;
            case VarType._Quaternion: //Quaternion
                data = ReadQuaternion();
                break;
        }
        return data;
    }

    /// <summary>Reads a byte from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public byte ReadByte(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            byte _value = readableBuffer[readPos]; // Get the byte at readPos' position
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 1; // Increase readPos by 1
            }
            return _value; // Return the byte
        }
        else
        {
            throw new Exception("Could not read value of type 'byte'!");
        }
    }

    /// <summary>Reads an array of bytes from the packet.</summary>
    /// <param name="_length">The length of the byte array.</param>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public byte[] ReadBytes(int _length, bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            byte[] _value = buffer.GetRange(readPos, _length).ToArray(); // Get the bytes at readPos' position with a range of _length
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += _length; // Increase readPos by _length
            }
            return _value; // Return the bytes
        }
        else
        {
            throw new Exception("Could not read value of type 'byte[]'!");
        }
    }

    /// <summary>Reads a short from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public short ReadShort(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            short _value = BitConverter.ToInt16(readableBuffer, readPos); // Convert the bytes to a short
            if (_moveReadPos)
            {
                // If _moveReadPos is true and there are unread bytes
                readPos += 2; // Increase readPos by 2
            }
            return _value; // Return the short
        }
        else
        {
            throw new Exception("Could not read value of type 'short'!");
        }
    }

    /// <summary>Reads an int from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public int ReadInt(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            int _value = BitConverter.ToInt32(readableBuffer, readPos); // Convert the bytes to an int
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 4; // Increase readPos by 4 (neden 4 artıyor: int 4byte tan oluşur.yani 32 bit. zaten verinin byte'ı 4ten büyükse okunabiliyordu Client'de) 11111111 11111111 11111111 11111111 -> max int (4.294.967.295)
            }
            return _value; // Return the int
        }
        else
        {
            throw new Exception("Could not read value of type 'int'!");
        }
    }

    /// <summary>Reads a long from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public long ReadLong(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            long _value = BitConverter.ToInt64(readableBuffer, readPos); // Convert the bytes to a long
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 8; // Increase readPos by 8
            }
            return _value; // Return the long
        }
        else
        {
            throw new Exception("Could not read value of type 'long'!");
        }
    }

    /// <summary>Reads a float from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public float ReadFloat(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            float _value = BitConverter.ToSingle(readableBuffer, readPos); // Convert the bytes to a float
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 4; // Increase readPos by 4
            }
            return _value; // Return the float
        }
        else
        {
            throw new Exception("Could not read value of type 'float'!");
        }
    }

    /// <summary>Reads a bool from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public bool ReadBool(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            bool _value = BitConverter.ToBoolean(readableBuffer, readPos); // Convert the bytes to a bool
            if (_moveReadPos)
            {
                // If _moveReadPos is true
                readPos += 1; // Increase readPos by 1
            }
            return _value; // Return the bool
        }
        else
        {
            throw new Exception("Could not read value of type 'bool'!");
        }
    }

    /// <summary>Reads a string from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public string ReadString(bool _moveReadPos = true)
    {
        try
        {
            int _length = ReadInt(); // Get the length of the string
            string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); // Convert the bytes to a string
            if (_moveReadPos && _value.Length > 0)
            {
                // If _moveReadPos is true string is not empty
                readPos += _length; // Increase readPos by the length of the string
            }
            return _value; // Return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }
    /// <summary>Reads a Vector3 from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public Vector3 ReadVector3(bool _moveReadPos = true)
    {
        return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
    }
    /// <summary>Reads a Quaterniom from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public Quaternion ReadQuaternion(bool _moveReadPos = true)
    {
        return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
    }
    #endregion

    private bool disposed = false;

    protected virtual void Dispose(bool _disposing)
    {
        if (!disposed)
        {
            if (_disposing)
            {
                buffer = null;
                readableBuffer = null;
                readPos = 0;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
