using Murder.Core.Geometry;
using System.Globalization;
using System.Numerics;
using System.Xml;

namespace Murder.Utilities
{
    internal static class XmlHelper
    {
        public static XmlDocument LoadXML(string filename)
        {
            XmlDocument xml = new XmlDocument();
            using (var stream = File.OpenRead(filename))
                xml.Load(stream);
            return xml;
        }

        public static bool HasAttr(this XmlElement xml, string attributeName)
        {
            return xml.Attributes[attributeName] != null;
        }

        public static string Attr(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return xml.Attributes[attributeName]!.InnerText;
        }

        public static string Attr(this XmlElement xml, string attributeName, string defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return xml.Attributes[attributeName]!.InnerText;
        }

        public static int AttrInt(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToInt32(xml.Attributes[attributeName]!.InnerText);
        }

        public static int AttrInt(this XmlElement xml, string attributeName, int defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return Convert.ToInt32(xml.Attributes[attributeName]!.InnerText);
        }

        public static float AttrFloat(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToSingle(xml.Attributes[attributeName]!.InnerText, CultureInfo.InvariantCulture);
        }

        public static float AttrFloat(this XmlElement xml, string attributeName, float defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return Convert.ToSingle(xml.Attributes[attributeName]!.InnerText, CultureInfo.InvariantCulture);
        }

        public static Vector2 AttrVector2(this XmlElement xml, string xAttributeName, string yAttributeName)
        {
            return new Vector2(xml.AttrFloat(xAttributeName), xml.AttrFloat(yAttributeName));
        }

        public static Vector2 AttrVector2(this XmlElement xml, string xAttributeName, string yAttributeName, Vector2 defaultValue)
        {
            return new Vector2(xml.AttrFloat(xAttributeName, defaultValue.X), xml.AttrFloat(yAttributeName, defaultValue.Y));
        }

        public static bool AttrBool(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToBoolean(xml.Attributes[attributeName]!.InnerText);
        }

        public static bool AttrBool(this XmlElement xml, string attributeName, bool defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return AttrBool(xml, attributeName);
        }

        public static char AttrChar(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToChar(xml.Attributes[attributeName]!.InnerText);
        }

        public static char AttrChar(this XmlElement xml, string attributeName, char defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return AttrChar(xml, attributeName);
        }

        public static T AttrEnum<T>(this XmlElement xml, string attributeName) where T : struct
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            if (Enum.IsDefined(typeof(T), xml.Attributes[attributeName]!.InnerText))
                return (T)Enum.Parse(typeof(T), xml.Attributes[attributeName]!.InnerText);
            else
                throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        public static T AttrEnum<T>(this XmlElement xml, string attributeName, T defaultValue) where T : struct
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return xml.AttrEnum<T>(attributeName);
        }

        public static Vector2 Position(this XmlElement xml)
        {
            return new Vector2(xml.AttrFloat("x"), xml.AttrFloat("y"));
        }

        public static Vector2 Position(this XmlElement xml, Vector2 defaultPosition)
        {
            return new Vector2(xml.AttrFloat("x", defaultPosition.X), xml.AttrFloat("y", defaultPosition.Y));
        }

        public static int X(this XmlElement xml)
        {
            return xml.AttrInt("x");
        }

        public static int X(this XmlElement xml, int defaultX)
        {
            return xml.AttrInt("x", defaultX);
        }

        public static int Y(this XmlElement xml)
        {
            return xml.AttrInt("y");
        }

        public static int Y(this XmlElement xml, int defaultY)
        {
            return xml.AttrInt("y", defaultY);
        }

        public static int Width(this XmlElement xml)
        {
            return xml.AttrInt("width");
        }

        public static int Width(this XmlElement xml, int defaultWidth)
        {
            return xml.AttrInt("width", defaultWidth);
        }

        public static int Height(this XmlElement xml)
        {
            return xml.AttrInt("height");
        }

        public static int Height(this XmlElement xml, int defaultHeight)
        {
            return xml.AttrInt("height", defaultHeight);
        }

        public static Rectangle Rect(this XmlElement xml)
        {
            return new Rectangle(xml.X(), xml.Y(), xml.Width(), xml.Height());
        }

        public static int ID(this XmlElement xml)
        {
            return xml.AttrInt("id");
        }

        public static int InnerInt(this XmlElement xml)
        {
            return Convert.ToInt32(xml.InnerText);
        }

        public static float InnerFloat(this XmlElement xml)
        {
            return Convert.ToSingle(xml.InnerText, CultureInfo.InvariantCulture);
        }

        public static bool InnerBool(this XmlElement xml)
        {
            return Convert.ToBoolean(xml.InnerText);
        }

        public static T InnerEnum<T>(this XmlElement xml) where T : struct
        {
            if (Enum.IsDefined(typeof(T), xml.InnerText))
                return (T)Enum.Parse(typeof(T), xml.InnerText);
            else
                throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        public static bool HasChild(this XmlElement xml, string childName)
        {
            return xml[childName] != null;
        }

        public static string ChildText(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName]!.InnerText;
        }

        public static string ChildText(this XmlElement xml, string childName, string defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName]!.InnerText;
            else
                return defaultValue;
        }

        public static int ChildInt(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName]!.InnerInt();
        }

        public static int ChildInt(this XmlElement xml, string childName, int defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName]!.InnerInt();
            else
                return defaultValue;
        }

        public static float ChildFloat(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName]!.InnerFloat();
        }

        public static float ChildFloat(this XmlElement xml, string childName, float defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName]!.InnerFloat();
            else
                return defaultValue;
        }

        public static bool ChildBool(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName]!.InnerBool();
        }

        public static bool ChildBool(this XmlElement xml, string childName, bool defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName]!.InnerBool();
            else
                return defaultValue;
        }

        public static T ChildEnum<T>(this XmlElement xml, string childName) where T : struct
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            if (Enum.IsDefined(typeof(T), xml[childName]!.InnerText))
                return (T)Enum.Parse(typeof(T), xml[childName]!.InnerText);
            else
                throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        public static T ChildEnum<T>(this XmlElement xml, string childName, T defaultValue) where T : struct
        {
            if (xml.HasChild(childName))
            {
                if (Enum.IsDefined(typeof(T), xml[childName]!.InnerText))
                    return (T)Enum.Parse(typeof(T), xml[childName]!.InnerText);
                else
                    throw new Exception("The attribute value cannot be converted to the enum type.");
            }
            else
                return defaultValue;
        }

        public static Vector2 ChildPosition(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName]!.Position();
        }

        public static Vector2 ChildPosition(this XmlElement xml, string childName, Vector2 defaultValue)
        {
            if (xml.HasChild(childName))
                return xml[childName]!.Position(defaultValue);
            else
                return defaultValue;
        }

    }
}
