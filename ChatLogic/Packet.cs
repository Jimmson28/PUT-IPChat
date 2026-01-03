using System.Text;

namespace ChatLogic
{
    public class Packet : IDisposable
    {

        private List<byte> bufferList = null;
        private byte[] bufferArray = null;
        private int bufferIndex = 0;
        private bool disposed = false;

        public Packet() 
        { 
            bufferList = new List<byte>();
            bufferIndex = 0;
        }

        public Packet(byte[] data) 
        {
            bufferList = new List<byte>();
            bufferIndex = 0;
            writeBytes(data);
            bufferArray = bufferList.ToArray();
        }

        public byte[] getBytesArray()
        {
            bufferArray = bufferList.ToArray();
            return bufferArray;
        }
        
        public void writeBytes(byte[] value)
        {
            bufferList.AddRange(value);
        }

        public void writeInt(int value)
        {
            bufferList.AddRange(BitConverter.GetBytes(value));
        }

        public void writeFloat(float value)
        {
            bufferList.AddRange(BitConverter.GetBytes(value));
        }

        public void writeBool(bool value)
        {
            bufferList.AddRange(BitConverter.GetBytes(value));
        }

        public void writeString(string value)
        {
            writeInt(value.Length);
            bufferList.AddRange(Encoding.ASCII.GetBytes(value));
        }

        public byte[] readBytes(int length, bool moveReadPosition)
        { 
            if(bufferList.Count > bufferIndex)
            {
                byte[] value = bufferList.GetRange(bufferIndex, length).ToArray();
                if (moveReadPosition)
                {
                    bufferIndex += length;
                }
                return value;
            }
            else
            {
                throw new Exception("Error in readBytes()");
            }
        }

        public int readInt(bool moveReadPosition = true) 
        {
            if (bufferList.Count > bufferIndex)
            {
                int value = BitConverter.ToInt32(bufferArray, bufferIndex);
                if (moveReadPosition)
                {
                    bufferIndex += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Error in readInt()");
            }
        }

        public float readFloat(bool moveReadPosition = true)
        {
            if (bufferList.Count > bufferIndex)
            {
                float value = BitConverter.ToSingle(bufferArray, bufferIndex);
                if (moveReadPosition)
                {
                    bufferIndex += 4;
                }
                return value;
            }
            else
            {
                throw new Exception("Error in readFloat()");
            }
        }

        public bool readBoolean(bool moveReadPosition = true)
        {
            if (bufferList.Count > bufferIndex)
            {
                bool value = BitConverter.ToBoolean(bufferArray, bufferIndex);
                if (moveReadPosition)
                {
                    bufferIndex += 1;
                }
                return value;
            }
            else
            {
                throw new Exception("Error in readBoolean()");
            }
        }

        public string readString(bool moveReadPosition = true)
        {
            try
            {
                int length = readInt();
                string value = Encoding.ASCII.GetString(bufferArray, bufferIndex, length);
                bufferIndex += length;
                return value;
            }
            catch
            {
                throw new Exception("Error in readString()");
            }
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing)
            {
                bufferList.Clear();
                bufferList = null;
                bufferArray = null;
                bufferIndex = 0;
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
