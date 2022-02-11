using System;
using System.Net.Http;
using AareonTechnicalTest.JsonConfiguration;

namespace AareonTechnicalTest.Tests
{
	public class ObjectJsonContent: StringContent
	{
		public ObjectJsonContent(object entity)
			: base(entity?.Serialise(), null, mediaType: "application/json")
		{ }
	}
}

