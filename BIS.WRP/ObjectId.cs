namespace BIS.WRP
{
    public struct ObjectId
    {
        public bool IsObject => ((Id >> 31) & 1) > 0;
        public short ObjId => (short) (Id & 0b111_1111_1111);
        public short ObjX => (short) ((Id >> 11) & 0b11_1111_1111);
        public short ObjZ => (short) ((Id >> 21) & 0b11_1111_1111);

        public int Id { get; private set; }

        public static implicit operator int(ObjectId d)
        {
            return d.Id;
        }

        public static implicit operator ObjectId(int d)
        {
            ObjectId o = new ObjectId
            {
                Id = d
            };
            return o;
        }
    }
}