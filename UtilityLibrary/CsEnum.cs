#region + Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion


// projname: TestEnum.External
// itemname: CsEnum
// username: jeffs
// created:  7/7/2019 9:56:29 PM


namespace UtilityLibrary
{
	// this is the base / abstract class for the enum
	// T is this class
	// U is the enum value (the number attached to a regular enum)
	// V is the value associated with the the enum
	public abstract class CsEnumBase<T, U, V> :
		IComparable<CsEnumBase<T, U, V>>
		where T : CsEnumBase<T, U, V>
	{
		static CsEnumBase()
		{
			// the static constructor causes
			// early creation of the object
			Count = 0;
		}

		// constructor
		protected CsEnumBase(Enum m, V v)
		{
			Enum = m;
			Value = v;
			Ordinal = Count++;
		}

		public CsEnumBase(){}


	#region > Admin Private fields

		// the list of members
		private static readonly List<T> members = new List<T>();

	#endregion

	#region > Admin Private Properties

		// the enum associated with this class
		private Enum Enum { get; }

	#endregion

	#region > Admin Public Properties
		// number of members
		public static int Count { get; private set; }

		// ordinal number of this member (zero based)
		public int Ordinal { get; }

		// a name of this member
		public string Name => Enum.ToString();

		// the value attached to the enum
		public U EnumValue => (U) (IConvertible) Enum;

		// the value of this member - this value is
		// returned from the implicit conversion
		public V Value { get; }

		public Enum E => Enum;

	#endregion

	#region > Admin Operator

		public static implicit operator V (CsEnumBase<T, U, V> m)
		{
			return m.Value;
		}

	#endregion

	#region > Admin Functions

		public void Add(T t)
		{
			members.Add(t);
		}

		// compare
		public int CompareTo(CsEnumBase<T, U, V> other)
		{
			if (other.GetType() != typeof(T)) { return -1; }

			return Ordinal.CompareTo(other.Ordinal);
		}

		// determine if the name provided is a member
		public static bool IsMember(string name, bool caseSensitive)
		{
			return Find(name, caseSensitive) != null;
		}

		// finds and returns a member
		public static T Find (string name, bool caseSensitive = false)
		{
			if (caseSensitive)
			{
				return members.Find(s => s.ToString().Equals(name));
			}
			else
			{
				return members.Find(s => s.ToString().ToLower().Equals(name.ToLower()));
			}
		}

		// allow enumeration over the members
		public static IEnumerable Members()
		{
			foreach (T m in members)
			{
				yield return m;
			}
		}

		// get the members as an array
		public static T[] Values()
		{
			return members.ToArray();
		}

		// same as Find but throws an exception 
		public static T ValueOf(string name)
		{
			T m = Find(name, true);

			if (m != null) return m;

			throw new InvalidEnumArgumentException();
		}

	#endregion

	#region > Admin Overrides

		public override bool Equals(object obj)
		{
			if (obj != null && obj.GetType() != typeof(T)) return false;

			return ((T) obj).Ordinal == Ordinal ;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	#endregion
	}

	#region > emum extension
	public static class EnumEx
	{
		public static dynamic Value(this Enum e)
		{
			switch (e.GetTypeCode())
			{
			case TypeCode.Byte:
				{
					return (byte) (IConvertible) e;
				}
			case TypeCode.Int16:
				{
					return (short) (IConvertible) e;
				}
			case TypeCode.Int32:
				{
					return (int) (IConvertible) e;
				}
			case TypeCode.Int64:
				{
					return (long) (IConvertible) e;
				}
			case TypeCode.UInt16:
				{
					return (ushort) (IConvertible) e;
				}
			case TypeCode.UInt32:
				{
					return (uint) (IConvertible) e;
				}
			case TypeCode.UInt64:
				{
					return (ulong) (IConvertible) e;
				}
			case TypeCode.SByte:
				{
					return (sbyte) (IConvertible) e;
				}
			}

			return 0;
		}

		public static string Name(this Enum e)
		{
			return e.ToString(); 
		}
	}

	#endregion

}