﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Palaso.Annotations;
using Palaso.TestUtilities;

namespace Palaso.Tests.Annotations
{
	[TestFixture]
	public class AnnotatableCloneableTests : CloneableTests<Annotatable>
	{
		public override Annotatable CreateNewCloneable()
		{
			return new Annotatable();
		}

		public override string ExceptionList
		{
			get { return ""; }
		}

		protected override List<ValuesToSet> DefaultValuesForTypes
		{
			get
			{
				return new List<ValuesToSet>
							 {
								 new ValuesToSet(new Annotation{IsOn = false}, new Annotation{IsOn = true})
							 };
			}
		}
	}

	[TestFixture]
	public class AnnotationCloneableTests : CloneableTests<Annotation>
	{
		public override Annotation CreateNewCloneable()
		{
			return new Annotation();
		}

		public override string ExceptionList
		{
			get { return ""; }
		}

		protected override List<ValuesToSet> DefaultValuesForTypes
		{
			get
			{
				return new List<ValuesToSet>
							 {
								 new ValuesToSet(42, 7)
							 };
			}
		}
	}
}
