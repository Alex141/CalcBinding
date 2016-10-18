using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace CalcBinding.PathAnalysis
{
    public abstract class PathToken
    {
        public int Start { get; private set; }

        public int End { get; private set; }

        public abstract PathTokenId Id { get; }

        protected PathToken (int start, int end)
        {
            Start = start;
            End = end;
        }

        public override bool Equals(object obj)
        {
            var token = obj as PathToken;

            if (obj == null)
                return false;

            return (GetType() == token.GetType() && Start == token.Start && End == token.End && Id.Equals(token.Id));
        }
    }

    public class PropertyPathToken : PathToken
    {
        public IEnumerable<string> Properties { get; private set; }

        private PathTokenId id;
        public override PathTokenId Id { get { return id; } }

        public PropertyPathToken(int start, int end, IEnumerable<string> properties):base(start, end)
        {
            Properties = properties.ToList();
            id = new PathTokenId(PathTokenType.Property, String.Join(".", Properties));
        }
    }

    public class StaticPropertyPathToken : PropertyPathToken
    {
        public string Class { get; private set; }
        public string Namespace { get; private set; }

        private PathTokenId id;
        public override PathTokenId Id { get { return id; } }

        public StaticPropertyPathToken(int start, int end, string @namespace, string @class, IEnumerable<string> properties):base(start, end, properties)
        {
            Class = @class;
            Namespace = @namespace;
            id = new PathTokenId(PathTokenType.StaticProperty,String.Format("{0}:{1}.{2}", Namespace, Class, String.Join(".", Properties)));
        }
    }

    public class EnumToken : PathToken
    {
        public Type Enum { get; private set; }
        public string EnumMember { get; private set; }
        public string Namespace { get; private set; }

        private PathTokenId id;
        public override PathTokenId Id { get { return id; } }

        public EnumToken(int start, int end, string @namespace, Type @enum, string enumMember):base(start, end)
        {
            Enum = @enum;
            EnumMember = enumMember;
            Namespace = @namespace;

            id = new PathTokenId(PathTokenType.StaticProperty, String.Format("{0}:{1}.{2}", Namespace, @enum.Name, EnumMember));
        }
    }

    public class MathToken : PathToken
    {
        public string MathMember { get; private set; }

        private PathTokenId id;
        public override PathTokenId Id { get { return id; } }

        public MathToken(int start, int end, string mathMember):base(start, end)
        {
            MathMember = mathMember;
            id = new PathTokenId(PathTokenType.StaticProperty, String.Join(".", "Math", MathMember));
        }
    }

    public class PathTokenId
    {
        public PathTokenType PathType { get; private set; }
        public string Value { get; private set; }

        public PathTokenId(PathTokenType pathType, string value)
        {
            PathType = pathType;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var o = obj as PathTokenId;

            if (o == null)
                return false;

            return (o.PathType == PathType && o.Value == Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ PathType.GetHashCode();
        }
    }

    public enum PathTokenType
    {
        /// <summary>
        /// Math, e.g. Math.Sin, Math.PI
        /// </summary>
        Math,
        /// <summary>
        /// Usual propertyPath, e.g. Name, Caption, Models.Count
        /// </summary>
        Property,
        /// <summary>
        /// Static propertyPatj, e.g. local:MyStaticVM.MyProp
        /// </summary>
        StaticProperty,
        /// <summary>
        /// Enum, e.g. local:MyEnum.MyValue
        /// </summary>
        Enum
    }
}
