using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaCasinoHacker
{
    public class GtaFingerPackFile
    {
        public string name;
        public ushort screen_width;
        public ushort screen_height;
        public DateTime modified_utc;
        public GtaFingerprintTarget finger;
        public GtaFingerprintChallenge[] challenges;
        
        public void SaveFile(Stream s)
        {
            //Write file headers
            _SaveUInt(s, 1178686535); //GTAF in ASCII
            _SaveUShort(s, 0); //File version 0
            _SaveUShort(s, (ushort)challenges.Length); //Number of challenges
            _SaveUInt(s, (uint)(modified_utc - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
            _SaveUShort(s, screen_width); //Screen width
            _SaveUShort(s, screen_height); //Screen height

            //Write name
            _SaveString(s, name);

            //Write the finger
            _SaveGtaFinger(s, finger);

            //Write each challenege
            foreach (var f in challenges)
                _SaveGtaFinger(s, f);
        }

        public bool LoadFile(Stream s)
        {
            //Read file headers
            if (_ReadUInt(s) != 1178686535)
                return false;
            if (_ReadUShort(s) != 0)
                return false;
            challenges = new GtaFingerprintChallenge[_ReadUShort(s)];
            modified_utc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(_ReadUInt(s));
            screen_width = _ReadUShort(s);
            screen_height = _ReadUShort(s);

            //Read name
            name = _ReadString(s);

            //Read finger
            finger = new GtaFingerprintTarget(_LoadMask(s));

            //Read challeneges
            for (int i = 0; i < challenges.Length; i++)
                challenges[i] = new GtaFingerprintChallenge(_LoadMask(s));

            return true;
        }

        private void _SaveGtaFinger(Stream s, IGtaFingerprint data)
        {
            _SaveMask(s, (ushort)data.width, (ushort)data.height, data.data);
        }

        private void _SaveMask(Stream s, ushort width, ushort height, bool[,] payload)
        {
            //Convert the payload into bytes
            BitArray payloadArray = new BitArray(width * height);
            for(int x = 0; x<width; x++)
            {
                for(int y = 0; y<height; y++)
                {
                    payloadArray.Set((x * width) + y, payload[x, y]);
                }
            }
            byte[] payloadData = new byte[((width * height) / 8) + 1];
            payloadArray.CopyTo(payloadData, 0);

            //Write headers into file
            _SaveUShort(s, width);
            _SaveUShort(s, height);
            _SaveUInt(s, (uint)payloadData.Length);
            long compressedLenLocation = s.Position;
            _SaveUInt(s, 0); //Write space for us to write the compressed length

            //Open compressed stream and write
            using (GZipStream gz = new GZipStream(s, CompressionLevel.Optimal, true))
                gz.Write(payloadData, 0, payloadData.Length);

            //Rewind to write the length of the compressed data, then come back
            uint compressedDataLen = (uint)(s.Position - compressedLenLocation - 4);
            long endLocation = s.Position;
            s.Position = compressedLenLocation;
            _SaveUInt(s, compressedDataLen);
            s.Position = endLocation;
        }

        private void _SaveString(Stream s, string data)
        {
            //Convert to bytes
            byte[] payload = Encoding.UTF8.GetBytes(data);

            //Write length
            _SaveUShort(s, (ushort)payload.Length);
            s.Write(payload, 0, payload.Length);
        }

        private void _SaveUShort(Stream s, ushort value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);
            s.Write(data, 0, data.Length);
        }

        private void _SaveUInt(Stream s, uint value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);
            s.Write(data, 0, data.Length);
        }

        private string _ReadString(Stream s)
        {
            //Read length
            ushort len = _ReadUShort(s);

            //Read
            byte[] buffer = new byte[len];
            s.Read(buffer, 0, buffer.Length);

            //Convert
            return Encoding.UTF8.GetString(buffer);
        }

        private uint _ReadUInt(Stream s)
        {
            byte[] data = new byte[4];
            s.Read(data, 0, data.Length);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

        private ushort _ReadUShort(Stream s)
        {
            byte[] data = new byte[2];
            s.Read(data, 0, data.Length);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return BitConverter.ToUInt16(data, 0);
        }

        private bool[,] _LoadMask(Stream s)
        {
            //Read headers
            ushort width = _ReadUShort(s);
            ushort height = _ReadUShort(s);
            uint uncompressedLength = _ReadUInt(s);
            uint compressedLength = _ReadUInt(s);

            //Open compressed stream and read
            byte[] payloadData = new byte[uncompressedLength];
            byte[] compressedData = new byte[compressedLength];
            s.Read(compressedData, 0, compressedData.Length);
            using (GZipStream gz = new GZipStream(new MemoryStream(compressedData), CompressionMode.Decompress, true))
                gz.Read(payloadData, 0, payloadData.Length);

            //Unpack into bools
            BitArray ba = new BitArray(payloadData);
            bool[,] response = new bool[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    response[x, y] = ba.Get((x * width) + y);
                }
            }

            return response;
        }

        private void _CopyStreams(Stream source, Stream dest, int length)
        {
            int copied = 0;
            byte[] compressedBuffer = new byte[2048];
            int read = source.Read(compressedBuffer, 0, Math.Min(compressedBuffer.Length, length - copied));
            while(read != 0)
            {
                //dest.CopyTo()
            }
        }
    }
}
