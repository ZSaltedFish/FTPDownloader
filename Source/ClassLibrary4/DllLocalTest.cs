namespace InDll
{
    public class DllLocalTest
    {
        private int _value;
        public DllLocalTest(int value)
        {
            _value = value;
        }

        public int GetData()
        {
            return _value + 1;
        }
    }
}
