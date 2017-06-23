﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using dnlib.DotNet;

namespace il2cpp
{
	public class UniqueList<T> : IEnumerable<T>
	{
		private readonly HashSet<T> Set_ = new HashSet<T>();
		private readonly List<T> List_ = new List<T>();

		public int Count => List_.Count;
		public T this[int key] => List_[key];

		public bool Add(T val)
		{
			if (Set_.Contains(val))
				return false;

			Set_.Add(val);
			List_.Add(val);
			return true;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return List_.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	// 方法包装
	public class MethodX : GenericArgs, IEquatable<MethodX>
	{
		// 定义
		public readonly MethodDef Def;
		// 所在类型
		public readonly TypeX DeclType;

		// 返回类型
		public TypeX ReturnType { get; set; }
		// 参数类型列表
		public readonly List<TypeX> Args = new List<TypeX>();

		// 覆盖的方法集合
		private UniqueList<MethodX> OverrideImpls_;
		public UniqueList<MethodX> OverrideImpls => OverrideImpls_ ?? (OverrideImpls_ = new UniqueList<MethodX>());
		public bool HasOverrideImpls => OverrideImpls_ != null && OverrideImpls_.Count > 0;

		// 是否已标记
		public bool IsMarked { get; set; }
		// 是否已跳过
		public bool IsSkipped { get; set; }

		public MethodX(MethodDef metDef, TypeX declType)
		{
			Def = metDef;
			DeclType = declType;
		}

		public override int GetHashCode()
		{
			return Def.GetHashNoDecl() ^
				   DeclType.GetHashCode() ^
				   GenericHashCode();
		}

		public bool Equals(MethodX other)
		{
			Debug.Assert(other != null);

			if (ReferenceEquals(this, other))
				return true;

			return Def.EqualsNoDecl(other.Def) &&
				   DeclType.Equals(other.DeclType) &&
				   GenericEquals(other);
		}

		public override bool Equals(object obj)
		{
			return obj is MethodX other && Equals(other);
		}

		public string ToString(bool hasDeclType)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("{0} {1}{2}{3}",
				ReturnType?.ToString() ?? "<?>",
				hasDeclType ? DeclType.ToString() + "::" : "",
				Def.Name,
				GenericToString());

			sb.Append('(');
			bool isLast = false;
			foreach (var p in Args)
			{
				if (isLast)
					sb.Append(',');
				isLast = true;

				sb.Append(p);
			}
			sb.Append(')');

			return sb.ToString();
		}

		public override string ToString()
		{
			return ToString(false);
		}

		public string PrettyName()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("{0} {1}{2}", ReturnType?.PrettyName() ?? "<?>", Def.Name, GenericToString(true));

			sb.Append('(');
			bool isLast = false;
			foreach (var p in Args)
			{
				if (isLast)
					sb.Append(',');
				isLast = true;

				sb.Append(p.PrettyName());
			}
			sb.Append(')');

			return sb.ToString();
		}
	}
}