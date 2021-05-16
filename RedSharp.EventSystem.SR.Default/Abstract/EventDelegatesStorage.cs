using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RedSharp.EventSystem.Abstract
{
    public abstract class EventDelegatesStorage<TItem> where TItem : class
    {
        private const byte DecreaseTryingMax = 8;

        private TItem[] _buffer;
        private int _count;
        private int _firstEmptyPosition;
        private byte _decreaseTrying;

        public EventDelegatesStorage()
        {
            _buffer = new TItem[1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FindFirstEmptyPosition()
        {
            while (_firstEmptyPosition < _buffer.Length && _buffer[_firstEmptyPosition] != null)
                _firstEmptyPosition++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncreaseBuffer()
        {
            Array.Resize(ref _buffer, _buffer.Length * 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool NeedToDecreaseBuffer()
        {
            return _buffer.Length > 1 && _count <= _buffer.Length / 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryDecreaseBuffer()
        {
            if (_decreaseTrying < DecreaseTryingMax)
            {
                _decreaseTrying++;
            }
            else
            {
                _decreaseTrying = 0;

                var newBuffer = new TItem[_buffer.Length / 2];

                for (int x = 0, y = 0; x < _buffer.Length; x++)
                {
                    if (_buffer[x] != null)
                    {
                        newBuffer[y] = _buffer[x];

                        y++;
                    }
                }

                _firstEmptyPosition = _count;
                _buffer = newBuffer;
            }
        }

        protected int Count => _count;

        protected TItem[] Buffer => _buffer;

        protected void Add(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_firstEmptyPosition == _buffer.Length)
                IncreaseBuffer();

            _buffer[_firstEmptyPosition] = item;
            _count++;

            FindFirstEmptyPosition();
        }

        protected bool Contain(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            for (int i = 0; i < _buffer.Length; i++)
                if (_buffer[i] == item)
                    return true;

            return false;
        }

        protected bool Remove(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var newEmptyPosition = -1;
            var result = false;

            for (int i = 0; i < _buffer.Length; i++)
            {
                if (_buffer[i] == item)
                {
                    _buffer[i] = null;
                    _count--;

                    newEmptyPosition = i;

                    result = true;

                    break;
                }
            }

            if (newEmptyPosition != -1 && newEmptyPosition < _firstEmptyPosition)
                _firstEmptyPosition = newEmptyPosition;

            if (result && NeedToDecreaseBuffer())
                TryDecreaseBuffer();

            return result;
        }
    }
}
