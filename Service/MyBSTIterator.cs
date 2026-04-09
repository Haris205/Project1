public class MyBSTIterator<T> : IMyIterator<T> where T : IComparable<T>
    {
        private readonly T[] _ordered;
        private int _index;

        public MyBSTIterator(MyBST<T> tree)
        {
            _ordered = new T[tree.Count];
            _index = -1;

            int i = 0;
            foreach (var value in tree)
            {
                _ordered[i++] = value;
            }
        }

        public bool HasNext() => _index < _ordered.Length - 1;

        public T Next()
        {
            _index += 1;
            return _ordered[_index]!;
        }

        public void Reset()
        {
            _index = -1;
        }
    }