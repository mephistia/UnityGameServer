using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


// Do server para cliente
public enum ServerPackets
{
    welcome = 1,
    spawnPlayer,
    playerPosition,
    playerRotation,
    playerVelocity,
    playerDisconnected
}

// Do cliente para server
public enum ClientPackets
{
    welcomeReceived = 1,
    playerMovement,
    playerMouse,
    playerRotation
}
public class Packet : IDisposable
{
    private List<byte> buffer;
    private byte[] readableBuffer;
    private int readPos;

    // Pacote vazio sem id
    public Packet()
    {
        // Inicializa buffer
        buffer = new List<byte>();
        readPos = 0;
    }

    // Cria pacote para envio com ID
    public Packet(int _id)
    {
        buffer = new List<byte>();
        readPos = 0;

        Write(_id); // Escreve o ID no buffer
    }

    // Cria pacote para recebimento com os dados
    public Packet(byte[] _data)
    {
        buffer = new List<byte>();
        readPos = 0;

        SetBytes(_data);
    }

    #region Functions

    // Seta o conteúdo do pacote e prepara para leitura
    public void SetBytes(byte[] _data)
    {
        Write(_data);
        readableBuffer = buffer.ToArray();
    }


    // Insere o tamanho do conteúdo do pacote no começo do buffer
    public void WriteLength()
    {
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
    }


    // Insere int no começo do buffer
    public void InsertInt(int _value)
    {
        buffer.InsertRange(0, BitConverter.GetBytes(_value));
    }

    // Recebe o conteúdo do pacote como Array
    public byte[] ToArray()
    {
        readableBuffer = buffer.ToArray();
        return readableBuffer;
    }

    // Retorna o tamanho do buffer
    public int Length()
    {
        return buffer.Count;
    }

    // Recebe o tamanho de dados que faltam ler
    public int UnreadLength()
    {
        return Length() - readPos;
    }


    // Reseta o pacote para reuso
    public void Reset(bool _shouldReset = true)
    {
        if (_shouldReset)
        {
            buffer.Clear();
            readableBuffer = null;
            readPos = 0;
        }
        else
        {
            readPos -= 4; // "deslê" o último int lido
        }
    }
    #endregion

    // Adicionar dados por tipo
    #region Write Data
    public void Write(byte _value)
    {
        buffer.Add(_value);
    }

    public void Write(byte[] _value)
    {
        buffer.AddRange(_value);
    }


    public void Write(short _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }


    public void Write(int _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }


    public void Write(long _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }


    public void Write(float _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }


    public void Write(bool _value)
    {
        buffer.AddRange(BitConverter.GetBytes(_value));
    }


    public void Write(string _value)
    {
        Write(_value.Length);
        buffer.AddRange(Encoding.ASCII.GetBytes(_value));
    }


    public void Write(Vector3 _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
    }

    public void Write(Vector2 _value)
    {
        Write(_value.x);
        Write(_value.y);
    }

    public void Write(Quaternion _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
        Write(_value.w);
    }

    #endregion

    // Ler dados por tipo
    #region Read Data

    public byte ReadByte(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // Se tem dados não lidos
            byte _value = readableBuffer[readPos];
            if (_moveReadPos)
            {

                readPos++; // aumenta a posição de leitura
            }
            return _value; // retorna o byte
        }
        else
        {
            throw new Exception("Could not read value of type 'byte'!");
        }
    }


    public byte[] ReadBytes(int _length, bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            byte[] _value = buffer.GetRange(readPos, _length).ToArray();
            if (_moveReadPos)
            {
                readPos += _length;
            }
            return _value;
        }
        else
        {
            throw new Exception("Could not read value of type 'byte[]'!");
        }
    }


    public short ReadShort(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            short _value = BitConverter.ToInt16(readableBuffer, readPos); // Converter para bytes
            if (_moveReadPos)
            {

                readPos += 2;
            }
            return _value;
        }
        else
        {
            throw new Exception("Could not read value of type 'short'!");
        }
    }


    public int ReadInt(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            // If there are unread bytes
            int _value = BitConverter.ToInt32(readableBuffer, readPos);
            if (_moveReadPos)
            {

                readPos += 4;
            }
            return _value;
        }
        else
        {
            throw new Exception("Could not read value of type 'int'!");
        }
    }


    public long ReadLong(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            long _value = BitConverter.ToInt64(readableBuffer, readPos);
            if (_moveReadPos)
            {
                readPos += 8;
            }
            return _value;
        }
        else
        {
            throw new Exception("Could not read value of type 'long'!");
        }
    }

    public float ReadFloat(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            float _value = BitConverter.ToSingle(readableBuffer, readPos);
            if (_moveReadPos)
            {
                readPos += 4;
            }
            return _value;
        }
        else
        {
            throw new Exception("Could not read value of type 'float'!");
        }
    }


    public bool ReadBool(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            bool _value = BitConverter.ToBoolean(readableBuffer, readPos);
            if (_moveReadPos)
            {
                readPos++;
            }
            return _value;
        }
        else
        {
            throw new Exception("Could not read value of type 'bool'!");
        }
    }


    public string ReadString(bool _moveReadPos = true)
    {
        try
        {
            int _length = ReadInt();
            string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length);
            if (_moveReadPos && _value.Length > 0)
            {
                readPos += _length;
            }
            return _value;
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }


    public Vector3 ReadVector3(bool _moveReadPos = true)
    {
        return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
    }

    public Vector2 ReadVector2(bool _moveReadPos = true)
    {
        return new Vector2(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
    }


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
