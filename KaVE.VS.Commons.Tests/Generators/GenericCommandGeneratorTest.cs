/*
 * Copyright 2017 University of Zurich
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using JetBrains.DataFlow;
using JetBrains.Threading;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Utils;
using KaVE.Commons.Utils;
using KaVE.VS.Commons.Generators;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Commons.Tests.Generators
{
    internal class GenericCommandGeneratorTest
    {
        private const string TestUUID = "test-uuid";
        private const string TestVersion = "1.2-test";

        private GenericCommandGenerator _sut;

        private IList<IIDEEvent> _publishedEvents;
        private TestDateUtils _testDateUtils;

        [SetUp]
        public void SetUp()
        {
            var ideSess = Mock.Of<IIDESession>();
            Mock.Get(ideSess).SetupGet(r => r.UUID).Returns(TestUUID);

            var rsEnv = Mock.Of<IRSEnv>();
            Mock.Get(rsEnv).SetupGet(r => r.IDESession).Returns(ideSess);
            Mock.Get(rsEnv).SetupGet(r => r.KaVEVersion).Returns(TestVersion);

            _publishedEvents = new List<IIDEEvent>();
            var messageBus = Mock.Of<IMessageBus>();
            Mock.Get(messageBus).Setup(bus => bus.Publish(It.IsAny<IIDEEvent>())).Callback<IIDEEvent>(
                ideEvent => _publishedEvents.Add(ideEvent));

            _testDateUtils = new TestDateUtils();

            var threading = new Invocator(Lifetimes.Define("testlifetime").Lifetime);

            _sut = new GenericCommandGeneratorImpl(rsEnv, messageBus, _testDateUtils, threading);
        }

        [Test]
        public void DefaultCase()
        {
            _sut.Fire("x");

            var actual = _publishedEvents;
            var expected = new List<IIDEEvent>
            {
                new CommandEvent
                {
                    CommandId = "x",
                    IDESessionUUID = TestUUID,
                    KaVEVersion = TestVersion,
                    TriggeredAt = _testDateUtils.Now,
                    TriggeredBy = EventTrigger.Unknown
                }
            };
            Assert.AreEqual(expected, actual);
        }
    }

    public class GenericCommandGeneratorImpl : GenericCommandGenerator
    {
        public GenericCommandGeneratorImpl(IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils,
            IThreading threading) : base(env, messageBus, dateUtils, threading) { }
    }
}