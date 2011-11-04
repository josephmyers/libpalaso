﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Palaso.IO;
using Palaso.UI.WindowsForms.ClearShare;
using Palaso.UI.WindowsForms.ImageToolbox;

namespace PalasoUIWindowsForms.Tests.ClearShare
{
	[TestFixture]
	public class CreativeCommonsLicenseTests
	{
		[Test]
		public void RoundTrip_BY()
		{
			var original =new CreativeCommonsLicense(true,true,CreativeCommonsLicense.DerivativeRules.Derivatives);
			var copy = CreativeCommonsLicense.FromLicenseUrl(original.Url);
			Assert.AreEqual(copy.AttributionRequired, true);
			Assert.AreEqual(copy.CommercialUseAllowed, true);
			Assert.AreEqual(copy.DerivativeRule, CreativeCommonsLicense.DerivativeRules.Derivatives);
		}

		[Test]
		public void RoundTrip_BY_NC_ND()
		{
			var original = new CreativeCommonsLicense(true, false, CreativeCommonsLicense.DerivativeRules.NoDerivatives);
			var copy = CreativeCommonsLicense.FromLicenseUrl(original.Url);
			Assert.AreEqual(copy.AttributionRequired, true);
			Assert.AreEqual(copy.CommercialUseAllowed, false);
			Assert.AreEqual(copy.DerivativeRule, CreativeCommonsLicense.DerivativeRules.NoDerivatives);
		}

		[Test]
		public void RoundTrip_BY_SA()
		{
			var original = new CreativeCommonsLicense(true, true, CreativeCommonsLicense.DerivativeRules.DerivativesWithShareAndShareAlike);
			var copy = CreativeCommonsLicense.FromLicenseUrl(original.Url);
			Assert.AreEqual(copy.AttributionRequired, true);
			Assert.AreEqual(copy.CommercialUseAllowed, true);
			Assert.AreEqual(copy.DerivativeRule, CreativeCommonsLicense.DerivativeRules.DerivativesWithShareAndShareAlike);
		}


		[Test]
		public void FromLicenseUrl_VersionRead()
		{
			var original = CreativeCommonsLicense.FromLicenseUrl("http://creativecommons.org/licenses/by-nd/4.3/");
			Assert.AreEqual(original.Version, "4.3");

		}

		[Test]
		public void Url_GivesVersion()
		{
			var original = new CreativeCommonsLicense(true, true, CreativeCommonsLicense.DerivativeRules.Derivatives);
			original.Version = "2.2";
			Assert.AreEqual("http://creativecommons.org/licenses/by/2.2/", original.Url);
		}
	}
}