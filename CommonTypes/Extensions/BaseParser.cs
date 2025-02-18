using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.Extensions
{
    public abstract class BaseParser<T>
    {
        private VisitAction<T>?[] _visitMap;
        protected void addVisit(int state, VisitAction<T> action)
        {
            _visitMap[state] = action;
        }
        protected void removeVisit(int state)
        {
            _visitMap[state] = default;
        }
        protected virtual void defaultVisit(ReadOnlySpan<char> text, scoped ref VisitArgument<T> argument)
        {
            argument.Position++;
        }
        protected abstract void finalVisit(ReadOnlySpan<char> text, scoped ref VisitArgument<T> argument);
        protected abstract T getDefaultResult();
        protected virtual T doVisit(ReadOnlySpan<char> text)
        {
            VisitArgument<T> argument = new(0, 0, 0, getDefaultResult());
            int state = 0;
            var map = _visitMap.AsSpan();
            var visit = map[0] ?? defaultVisit;
            while (argument.Position < text.Length)
            {
                if (state == argument.State)
                {
                    visit(text, ref argument);
                }
                else
                {
                    state = argument.State;
                    if (state < map.Length)
                        visit = map[state] ?? defaultVisit;
                    else
                        visit = defaultVisit;
                }
            }
            finalVisit(text, ref argument); 
            return argument.Result;
        }
        public T Visit(string value)
        {
            var text = value.AsSpan();
            return doVisit(text);
        }
        public BaseParser() : this(255)
        {
        }
        public BaseParser(int length)
        {
            _visitMap = new VisitAction<T>?[length];
        }
    }
    public ref struct VisitArgument<T>
    {
        public int State;
        public int Begin;
        public int Position;
        public T Result;
        public VisitArgument(int state, int begin, int position, T result)
        {
            State = state;
            Begin = begin;
            Position = position;
            Result = result;
        }
    }
    public delegate void VisitAction<T>(ReadOnlySpan<char> text, scoped ref VisitArgument<T> argument);
}
