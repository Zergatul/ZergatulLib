using System;
using System.Collections.Generic;
using System.IO;
using Zergatul.IO;

namespace Zergatul.Network.Http.Frames
{
    class SettingsFrame : Frame
    {
        public override FrameType Type => FrameType.SETTINGS;

        public bool ACK
        {
            get => (Flags & 0x01) != 0;
            set
            {
                if (value)
                    Flags |= 0x01;
                else
                    Flags &= 0xFF ^ 0x01;
            }
        }

        public Dictionary<SettingParameter, uint> Parameters;

        public override void ReadPayload(Stream stream, int length)
        {
            if (ACK && length != 0)
            {
                GoAwayWith(ErrorCode.FRAME_SIZE_ERROR);
                return;
            }

            if (Id != 0)
            {
                GoAwayWith(ErrorCode.PROTOCOL_ERROR);
                return;
            }

            if (length % 6 != 0)
            {
                GoAwayWith(ErrorCode.FRAME_SIZE_ERROR);
                return;
            }

            if (length > 0)
            {
                Parameters = new Dictionary<SettingParameter, uint>();
                byte[] buffer = new byte[6];
                for (int i = 0; i < length / 6; i++)
                {
                    StreamHelper.ReadArray(stream, buffer);
                    var identifier = (SettingParameter)BitHelper.ToUInt16(buffer, 0, ByteOrder.BigEndian);
                    uint value = BitHelper.ToUInt32(buffer, 2, ByteOrder.BigEndian);

                    switch (identifier)
                    {
                        case SettingParameter.SETTINGS_ENABLE_PUSH:
                            if (value != 0 && value != 1)
                            {
                                GoAwayWith(ErrorCode.PROTOCOL_ERROR);
                                return;
                            }
                            break;

                        case SettingParameter.SETTINGS_INITIAL_WINDOW_SIZE:
                            if (value >= 0x80000000)
                            {
                                GoAwayWith(ErrorCode.FLOW_CONTROL_ERROR);
                                return;
                            }
                            break;

                        case SettingParameter.SETTINGS_MAX_FRAME_SIZE:
                            if (value < 16384 || value >= 16777216)
                            {
                                GoAwayWith(ErrorCode.PROTOCOL_ERROR);
                                return;
                            }
                            break;
                    }

                    if (Parameters.ContainsKey(identifier))
                        Parameters[identifier] = value;
                    else
                        Parameters.Add(identifier, value);
                }
            }
        }

        public override byte[] GetPayload()
        {
            if (Parameters == null)
                return null;
            if (Parameters.Count == 0)
                return null;

            byte[] payload = new byte[Parameters.Count * 6];
            int index = 0;
            foreach (var kv in Parameters)
            {
                BitHelper.GetBytes((ushort)kv.Key, ByteOrder.BigEndian, payload, index);
                index += 2;
                BitHelper.GetBytes(kv.Value, ByteOrder.BigEndian, payload, index);
                index += 4;
            }
            return payload;
        }
    }
}