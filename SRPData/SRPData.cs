namespace SRPSimulator
{
    using timeType = System.Int32;
    using powerType = System.Single;
    using freqType = System.Single;
    using xType = System.Single;
    using fType = System.Single;
    using ddpType = System.Int32;

    public class SRPData(timeType time, powerType power, freqType freq,
        xType x, fType f, ddpType ddp)
    {
        public SRPData() : this(0, 0, 0, 0, 0, 0) { }

        public static int Size { 
            get => sizeof(timeType) + sizeof(powerType) + sizeof(freqType) +
                sizeof(xType) + sizeof(fType) + sizeof(ddpType); 
        }

        public bool Serialize(Span<byte> destination)
        {
            int cursor = 0;
            return
                BitConverter.TryWriteBytes(destination, time) &&
                BitConverter.TryWriteBytes(destination.Slice(cursor = sizeof(timeType)), power) &&
                BitConverter.TryWriteBytes(destination.Slice(cursor += sizeof(powerType)), freq) &&
                BitConverter.TryWriteBytes(destination.Slice(cursor += sizeof(freqType)), x) &&
                BitConverter.TryWriteBytes(destination.Slice(cursor += sizeof(xType)), f) &&
                BitConverter.TryWriteBytes(destination.Slice(cursor += sizeof(fType)), ddp);
        }

        static public SRPData? Deserialize(Span<byte> source)
        {
            var data = new SRPData();
            Deserialize(source, ref data);
            return data;
        }

        static public void Deserialize(Span<byte> source, ref SRPData data)
        {
            int cursor = 0;
            data.Time = BitConverter.ToInt32(source);
            data.Power = BitConverter.ToSingle(source.Slice(cursor = sizeof(timeType)));
            data.Freq = BitConverter.ToSingle(source.Slice(cursor += sizeof(powerType)));
            data.X = BitConverter.ToSingle(source.Slice(cursor += sizeof(freqType)));
            data.F = BitConverter.ToSingle(source.Slice(cursor += sizeof(xType)));
            data.DDP = BitConverter.ToInt32(source.Slice(cursor += sizeof(fType)));
        }

        public void Deconstruct(out timeType timeOut, out powerType powerOut, out freqType freqOut,
            out xType xOut, out fType fOut, out ddpType ddpOut)
        {
            timeOut = time;
            powerOut = power;
            freqOut = freq;
            xOut = x;
            fOut = f;
            ddpOut = ddp;
        }

        public timeType Time { get => time; set => time = value; }
        public powerType Power { get => power; set => power = value; }
        public freqType Freq { get => freq; set => freq = value; }
        public xType X { get => x; set => x = value; }
        public fType F { get => f; set => f = value; }
        public ddpType DDP { get => ddp; set => ddp = value; }
    }
}
