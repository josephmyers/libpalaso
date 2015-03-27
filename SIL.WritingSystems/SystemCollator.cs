using System;
using System.Globalization;

namespace SIL.WritingSystems
{
	public class SystemCollator : ICollator
	{
		private readonly CultureInfo _cultureInfo;

		public SystemCollator(string cultureId)
		{
			_cultureInfo = null;
			if (!string.IsNullOrEmpty(cultureId))
				_cultureInfo = GetCultureInfo(cultureId);
			if (_cultureInfo == null)
				_cultureInfo = CultureInfo.InvariantCulture;
		}

		public SortKey GetSortKey(string source)
		{
			return _cultureInfo.CompareInfo.GetSortKey(source);
		}

		public int Compare(string x, string y)
		{
			int order = _cultureInfo.CompareInfo.Compare(x, y);

			if (order != 0)
			{
				return order;
			}
			if (_cultureInfo == CultureInfo.InvariantCulture)
			{
				// bugfix WS-33997.  Khmer (invariant culture) strings when compared return "same",
				// when in fact they are different strings.  In this case, use an ordinal compare.
				if (x != null && x.GetHashCode() == y.GetHashCode())
				{
					return 0;
				}
				return String.CompareOrdinal(x, y);
			}
			return 0;
		}

		public int Compare(object x, object y)
		{
			return Compare((string)x, (string)y);
		}

		/// <summary>
		/// Validates the specified culture ID.
		/// </summary>
		public static bool ValidateCultureId(string cultureId, out string message)
		{
			try
			{
				if (!String.IsNullOrEmpty(cultureId))
					GetCultureInfo(cultureId);
			}
			catch (Exception e)
			{
				message = String.Format("Error while validating sorting rules: {0}", e.Message);
				return false;
			}
			message = null;
			return true;
		}

		private static CultureInfo GetCultureInfo(string cultureId)
		{
			CultureInfo ci;
			try
			{
				ci = CultureInfo.GetCultureInfo(cultureId);
			}
			catch (ArgumentException e)
			{
				if (e is ArgumentNullException || e is ArgumentOutOfRangeException)
				{
					throw;
				}
				ci = TryGetCultureInfoByIetfLanguageTag(cultureId);
			}
			return ci;
		}

		private static CultureInfo TryGetCultureInfoByIetfLanguageTag(string ietfLanguageTag)
		{
			CultureInfo ci = null;
			try
			{
				ci = CultureInfo.GetCultureInfoByIetfLanguageTag(ietfLanguageTag);
			}
			catch (ArgumentException ex)
			{
				if (ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
				{
					throw;
				}
			}
			return ci;
		}
	}
}
