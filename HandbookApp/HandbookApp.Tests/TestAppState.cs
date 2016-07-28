using System;
using System.Collections.Immutable;
using NUnit.Framework;
using HandbookApp.States;

namespace HandbookApp.Tests
{
    [TestFixture]
    public class TestAppState
    {
        [Test]
        public void HasBooks()
        {
            AppState item = new AppState();

            Assert.IsInstanceOf<ImmutableDictionary<string, Book>>(item.Books);
        }
    }
}
