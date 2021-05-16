using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RedSharp.EventSystem.Abstract
{
    public abstract class WeakEventDelegatesStorage<TItem> where TItem : class
    {
        private const byte DecreaseTryingMax = 8;

        private WeakReference<TItem>[] _buffer;
        private bool[] _isAlive;
        private int _count;
        private int _firstEmptyPosition;
        private byte _decreaseTrying;

        public WeakEventDelegatesStorage()
        {
            _buffer = new WeakReference<TItem>[1];
            _isAlive = new bool[1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FindFirstEmptyPosition()
        {
            while (_firstEmptyPosition < _buffer.Length && _isAlive[_firstEmptyPosition])
                _firstEmptyPosition++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncreaseBuffer()
        {
            Array.Resize(ref _buffer, _buffer.Length * 2);
            Array.Resize(ref _isAlive, _buffer.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool NeedToDecreaseBuffer()
        {
            return _buffer.Length > 1 && _count <= _buffer.Length / 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void TryDecreaseBuffer()
        {
            if (_decreaseTrying < DecreaseTryingMax)
            {
                _decreaseTrying++;
            }
            else
            {
                _decreaseTrying = 0;

                var newBuffer = new WeakReference<TItem>[_buffer.Length / 2];
                var newIsAlive = new bool[newBuffer.Length];

                for (int x = 0, y = 0; x < _buffer.Length; x++)
                {
                    if (_isAlive[x])
                    {
                        newBuffer[y] = _buffer[x];
                        newIsAlive[y] = true;

                        y++;
                    }
                }

                _firstEmptyPosition = _count;
                _buffer = newBuffer;
                _isAlive = newIsAlive;
            }
        }

        protected int Count => _count;

        protected WeakReference<TItem>[] Buffer => _buffer;

        protected bool[] IsAlive => _isAlive;

        protected void Add(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_firstEmptyPosition == _buffer.Length)
                IncreaseBuffer();

            if (_buffer[_firstEmptyPosition] == null)
                _buffer[_firstEmptyPosition] = new WeakReference<TItem>(item);
            else
                _buffer[_firstEmptyPosition].SetTarget(item);

            _isAlive[_firstEmptyPosition] = true;
            _count++;

            FindFirstEmptyPosition();
        }

        protected bool Contain(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            bool result = false;

            int newEmptyPosition = -1;
            TItem tempItem;

            for (int i = 0; i < _buffer.Length; i++)
            {
                if (!_isAlive[i])
                    continue;

                if (!_buffer[i].TryGetTarget(out tempItem))
                {
                    _isAlive[i] = false;

                    if (newEmptyPosition == -1)
                        newEmptyPosition = i;

                    _count--;
                }
                else if(tempItem == item)
                {
                    result = true;

                    break;
                }
            }

            if (newEmptyPosition != -1 && newEmptyPosition < _firstEmptyPosition)
                _firstEmptyPosition = newEmptyPosition;

            return result;
        }

        protected bool Remove(TItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            bool result = false;

            int newEmptyPosition = -1;
            TItem tempItem;

            for (int i = 0; i < _buffer.Length; i++)
            {
                if (!_isAlive[i])
                    continue;

                if (!_buffer[i].TryGetTarget(out tempItem))
                {
                    _isAlive[i] = false;

                    if (newEmptyPosition == -1)
                        newEmptyPosition = i;

                    _count--;
                }
                else if (tempItem == item)
                {
                    _isAlive[i] = false;

                    if (newEmptyPosition == -1)
                        newEmptyPosition = i;

                    _count--;

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
