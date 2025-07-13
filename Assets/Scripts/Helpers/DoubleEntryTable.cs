using System;

namespace Helpers
{
    public class DoubleEntryTable<RT, CT, VT>
    {
        private readonly RT[] _rows;
        private readonly CT[] _columns;
        private readonly VT[,] _values;

        public DoubleEntryTable (RT[] rows, CT[] columns)
        {
            this._rows = rows;
            this._columns = columns;
            _values = new VT[rows.Length, columns.Length];
        }

        public VT this[RT row, CT column]
        {
            get
            {
                int i = Array.IndexOf(_rows, row);
                int j = Array.IndexOf(_columns, column);

                return _values[i, j];
            }

            set
            {
                int i = Array.IndexOf(_rows, row);
                int j = Array.IndexOf(_columns, column);

                _values[i, j] = value;
            }
        }
    }
}